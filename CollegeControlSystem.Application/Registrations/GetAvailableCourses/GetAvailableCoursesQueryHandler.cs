using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Domain.Abstractions;
using CollegeControlSystem.Domain.Shared;
using CollegeControlSystem.Domain.Students;

namespace CollegeControlSystem.Application.Registrations.GetAvailableCourses
{
    internal sealed class GetAvailableCoursesQueryHandler : IQueryHandler<GetAvailableCoursesQuery, List<AvailableCourseResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAvailableCoursesQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<List<AvailableCourseResponse>>> Handle(GetAvailableCoursesQuery request, CancellationToken cancellationToken)
        {
            // 1. Validate Student
            var student = await _unitOfWork.StudentRepository.GetByIdAsync(request.StudentId, cancellationToken);
            if (student is null)
            {
                return Result<List<AvailableCourseResponse>>.Failure(StudentErrors.StudentNotFound);
            }

            // 2. Validate Semester
            var semesterResult = Semester.Create(request.Term, request.Year);
            if (semesterResult.IsFailure)
            {
                return Result<List<AvailableCourseResponse>>.Failure(semesterResult.Error);
            }
            var semester = semesterResult.Value;

            // 3. Fetch all offerings for the requested semester
            // Note: In a more advanced implementation, you could filter this down by the student's ProgramId 
            // or exclude courses they have already passed.
            var offerings = await _unitOfWork.CourseOfferingRepository.GetBySemesterAsync(semester, cancellationToken);

            // 4. Map to DTO
            var response = offerings.Select(o => new AvailableCourseResponse(
                o.Id,
                o.Course?.Code?.Value ?? "N/A",
                o.Course?.Title ?? "Unknown",
                o.Course?.Credits ?? 0,
                o.Instructor?.FacultyName ?? "TBA",
                o.AvailableSeats,
                o.IsFull
            )).ToList();

            return Result<List<AvailableCourseResponse>>.Success(response);
        }
    }
}
