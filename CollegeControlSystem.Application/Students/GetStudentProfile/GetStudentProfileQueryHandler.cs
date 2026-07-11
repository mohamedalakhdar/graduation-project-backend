using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Domain.Abstractions;
using CollegeControlSystem.Domain.Registrations;
using CollegeControlSystem.Domain.Students;

namespace CollegeControlSystem.Application.Students.GetStudentProfile
{
    internal sealed class GetStudentProfileQueryHandler : IQueryHandler<GetStudentProfileQuery, StudentResponse>
    {
        private readonly IStudentRepository _studentRepository;

        public GetStudentProfileQueryHandler(IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository;
        }

        public async Task<Result<StudentResponse>> Handle(GetStudentProfileQuery request, CancellationToken cancellationToken)
        {
            var student = await _studentRepository.GetTranscriptDataAsync(request.StudentId, cancellationToken);

            if (student is null) return Result<StudentResponse>.Failure(StudentErrors.StudentNotFound);

            // Use your Domain Logic: Article 12 Load Limits
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
