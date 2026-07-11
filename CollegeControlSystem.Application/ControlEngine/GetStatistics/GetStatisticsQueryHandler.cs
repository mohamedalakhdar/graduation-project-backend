using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Domain.Abstractions;
using CollegeControlSystem.Domain.CourseOfferings;
using CollegeControlSystem.Domain.Registrations;
using CollegeControlSystem.Domain.Students;
using Microsoft.EntityFrameworkCore;

namespace CollegeControlSystem.Application.ControlEngine.GetStatistics
{
    internal sealed class GetStatisticsQueryHandler
        : IQueryHandler<GetStatisticsQuery, StatisticsResponse>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetStatisticsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<StatisticsResponse>> Handle(
            GetStatisticsQuery request,
            CancellationToken cancellationToken)
        {
            var students = await _unitOfWork.StudentRepository.GetAllAsync(cancellationToken);
            var departments = await _unitOfWork.DepartmentRepository.GetAllAsync(cancellationToken);
            var faculties = await _unitOfWork.FacultyRepository.GetAllAsync(cancellationToken);
            var offerings = await _unitOfWork.CourseOfferingRepository
                .GetAvailableOfferingsAsync(null, null, null, cancellationToken);

            int totalStudents = students.Count;
            int activeStudents = students.Count(s => s.AcademicStatus == AcademicStatus.GoodStanding);
            int warningStudents = students.Count(s => s.AcademicStatus == AcademicStatus.AcademicWarning);
            int dismissedStudents = students.Count(s => s.AcademicStatus == AcademicStatus.Dismissed);
            int graduatedStudents = students.Count(s => s.AcademicStatus == AcademicStatus.Graduated);

            int totalPrograms = departments.Sum(d => d.Programs.Count);
            int totalCourseOfferings = offerings.Count;

            decimal avgCgpa = totalStudents > 0
                ? Math.Round(students.Average(s => s.CGPA), 2)
                : 0.00m;

            var activeRegistrations = students
                .SelectMany(s => s.Registrations)
                .Count(r => r.Status == RegistrationStatus.Approved
                         || r.Status == RegistrationStatus.Pending);

            var response = new StatisticsResponse(
                totalStudents,
                activeStudents,
                warningStudents,
                dismissedStudents,
                graduatedStudents,
                departments.Count,
                totalPrograms,
                faculties.Count,
                totalCourseOfferings,
                avgCgpa,
                activeRegistrations
            );

            return Result<StatisticsResponse>.Success(response);
        }
    }
}
