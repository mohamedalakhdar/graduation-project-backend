using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Domain.Abstractions;
using CollegeControlSystem.Domain.CourseOfferings;
using CollegeControlSystem.Domain.Registrations;

namespace CollegeControlSystem.Application.CourseOfferings.GetOfferingAnalytics
{
    internal sealed class GetOfferingAnalyticsQueryHandler
        : IQueryHandler<GetOfferingAnalyticsQuery, OfferingAnalyticsResponse>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetOfferingAnalyticsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<OfferingAnalyticsResponse>> Handle(
            GetOfferingAnalyticsQuery request,
            CancellationToken cancellationToken)
        {
            var offering = await _unitOfWork.CourseOfferingRepository
                .GetByIdAsync(request.OfferingId, cancellationToken);

            if (offering is null)
                return Result<OfferingAnalyticsResponse>.Failure(CourseOfferingErrors.OfferingNotFound);

            var registrations = await _unitOfWork.RegistrationRepository
                .GetByOfferingIdAsync(request.OfferingId, cancellationToken);

            var enrolledRegs = registrations
                .Where(r => r.Status == RegistrationStatus.Approved
                         || r.Status == RegistrationStatus.Completed
                         || r.Status == RegistrationStatus.Pending)
                .ToList();

            var gradedRegs = enrolledRegs
                .Where(r => r.Status == RegistrationStatus.Completed && r.Grade is not null)
                .ToList();

            var withdrawnRegs = registrations
                .Where(r => r.Status == RegistrationStatus.Withdrawn)
                .ToList();

            int retakeCount = enrolledRegs.Count(r => r.IsRetake);
            int totalGraded = gradedRegs.Count;

            var gradeCounts = new Dictionary<string, int>();
            int passingCount = 0;

            foreach (var reg in gradedRegs)
            {
                string letter = reg.Grade!.LetterGrade ?? "IP";
                gradeCounts.TryGetValue(letter, out int count);
                gradeCounts[letter] = count + 1;

                if (reg.Grade.IsPassing)
                    passingCount++;
            }

            double passRate = totalGraded > 0
                ? Math.Round((double)passingCount / totalGraded * 100, 1)
                : 0.0;

            double avgGradePoints = totalGraded > 0
                ? (double)Math.Round(gradedRegs.Average(r => r.Grade!.GradePoints), 2)
                : 0.0;

            double fillRate = offering.Capacity > 0
                ? Math.Round((double)offering.CurrentEnrolled / offering.Capacity * 100, 1)
                : 0.0;

            var response = new OfferingAnalyticsResponse(
                offering.Id,
                offering.Course?.Code?.Value ?? "N/A",
                offering.Course?.Title ?? "N/A",
                $"{offering.Semester.Term} {offering.Semester.Year}",
                offering.Instructor?.FacultyName ?? "N/A",
                offering.Capacity,
                offering.CurrentEnrolled,
                fillRate,
                retakeCount,
                withdrawnRegs.Count,
                new GradeDistributionDto(totalGraded, gradeCounts),
                passRate,
                avgGradePoints
            );

            return Result<OfferingAnalyticsResponse>.Success(response);
        }
    }
}
