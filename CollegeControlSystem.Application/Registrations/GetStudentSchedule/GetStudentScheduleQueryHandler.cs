using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Domain.Abstractions;
using CollegeControlSystem.Domain.Registrations;
using CollegeControlSystem.Domain.Students;

namespace CollegeControlSystem.Application.Registrations.GetStudentSchedule
{
    internal sealed class GetStudentScheduleQueryHandler : IQueryHandler<GetStudentScheduleQuery, ScheduleResponse>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetStudentScheduleQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<ScheduleResponse>> Handle(GetStudentScheduleQuery request, CancellationToken cancellationToken)
        {
            // 1. Get Student Info (for the Header)
            var student = await _unitOfWork.StudentRepository.GetByIdAsync(request.StudentId, cancellationToken);
            if (student is null)
            {
                return Result<ScheduleResponse>.Failure(StudentErrors.StudentNotFound);
            }

            // 2. Determine Semester Logic
            // If request params are null, assume we want the current active semester (Logic depends on your system's clock/settings)
            // For this example, we'll fetch ALL active registrations if no filter is provided, 
            // or filter via the Repository if parameters exist.

            List<Registration> registrations;

            if (!string.IsNullOrEmpty( request.Term) && request.Year.HasValue)
            {
                registrations = await _unitOfWork.RegistrationRepository
                    .GetByStudentAndSemesterAsync(request.StudentId, request.Term, request.Year.Value, cancellationToken);
            }
            else
            {
                registrations = await _unitOfWork.RegistrationRepository
                    .GetActiveByStudentIdAsync(request.StudentId, cancellationToken);
            }

            // 3. Map to Response
            var scheduleItems = registrations.Select(r => new ClassScheduleItem(
                r.CourseOffering.CourseId,
                r.CourseOffering.Course.Code.Value, // Value Object
                r.CourseOffering.Course.Title,
                r.CourseOffering.Instructor.FacultyName,
                r.CourseOffering.Course.Credits,
                r.Status.ToString()
            )).ToList();

            // Calculate total credits for this view
            int totalCredits = scheduleItems.Sum(x => x.Credits);

            var response = new ScheduleResponse(
                student.Id,
                student.StudentName,
                request.Term?.ToString() ?? "Current",
                request.Year ?? DateTime.UtcNow.Year,
                totalCredits,
                scheduleItems
            );

            return Result<ScheduleResponse>.Success(response);
        }
    }
}
