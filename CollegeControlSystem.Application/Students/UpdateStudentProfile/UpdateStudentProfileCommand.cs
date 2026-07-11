using CollegeControlSystem.Application.Abstractions.Messaging;

namespace CollegeControlSystem.Application.Students.UpdateStudentProfile
{
    public sealed record UpdateStudentProfileCommand(
        Guid StudentId,
        string NewFullName,
        string NewNationalId // i don't know if allowed to change or not
    ) : ICommand;
}
