using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Domain.Abstractions;
using CollegeControlSystem.Domain.Identity;
using CollegeControlSystem.Domain.Registrations;
using CollegeControlSystem.Domain.Students;

namespace CollegeControlSystem.Application.Students.GetStudentProfile
{
    internal sealed class GetMyStudentProfileQueryHandler : IQueryHandler<GetMyStudentProfileQuery, StudentResponse>
    {
        private readonly IStudentRepository _studentRepository;

        public GetMyStudentProfileQueryHandler(IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository;
        }

        public async Task<Result<StudentResponse>> Handle(GetMyStudentProfileQuery request, CancellationToken cancellationToken)
        {
            var userId = request.User.FindFirst(Keys.UserIdKey)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Result<StudentResponse>.Failure(StudentErrors.StudentNotFound);

            var student = await _studentRepository.GetByAppUserIdAsync(userId, cancellationToken);

            if (student is null) return Result<StudentResponse>.Failure(StudentErrors.StudentNotFound);

            int maxCredits = student.GetMaxAllowedCreditHours();

            int completedCredits = student.Registrations
                .Where(r => r.Status == RegistrationStatus.Completed && r.Grade is not null && r.Grade.IsPassing)
                .Sum(r => r.CourseOffering.Course.Credits);

            int totalProgramCredits = student.Program.RequiredCredits;

            var response = new StudentResponse(
                student.Id,
                student.StudentName,
                student.AcademicNumber,
                student.Program.Name,
                student.CGPA,
                student.AcademicStatus.ToString(),
                student.AcademicLevel.ToString(),
                maxCredits,
                student.NationalId,
                completedCredits,
                totalProgramCredits
            );

            return Result<StudentResponse>.Success(response);
        }
    }
}
