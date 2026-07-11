using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Domain.Abstractions;
using CollegeControlSystem.Domain.Registrations;
namespace CollegeControlSystem.Application.CourseOfferings.GetRoster
{
    internal sealed class GetCourseRosterQueryHandler : IQueryHandler<GetCourseRosterQuery, List<StudentRosterResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetCourseRosterQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<List<StudentRosterResponse>>> Handle(GetCourseRosterQuery request, CancellationToken cancellationToken)
        {
            // Assuming the repository implementation Includes Student and Grade navigation properties
            var registrations = await _unitOfWork.RegistrationRepository.GetByOfferingIdAsync(request.OfferingId, cancellationToken);

            // Filter out dropped or withdrawn students if you don't want them on the active grading roster
            var activeRoster = registrations
                .Where(r => r.Status == RegistrationStatus.Approved || r.Status == RegistrationStatus.Completed)
                .Select(r => new StudentRosterResponse(
                    r.Id,
                    r.Student.AcademicNumber,
                    r.Student.StudentName,
                    r.Status.ToString(),
                    r.IsRetake,
                    r.Grade?.SemesterWorkGrade,
                    r.Grade?.FinalGrade,
                    r.Grade?.LetterGrade
                )).ToList();

            return Result<List<StudentRosterResponse>>.Success(activeRoster);
        }
    }
}
