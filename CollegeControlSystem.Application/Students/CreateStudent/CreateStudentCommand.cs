using CollegeControlSystem.Application.Abstractions.Messaging;

namespace CollegeControlSystem.Application.Students.CreateStudent
{
    public sealed record CreateStudentCommand(
        string UserName,
        string Email,
        string Password,
        string? PhoneNumber,
        string FullName,
        string AcademicNumber,
        string NationalId,
        Guid ProgramId
    ) : ICommand<Guid>;
}
