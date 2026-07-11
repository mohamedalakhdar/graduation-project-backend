using CollegeControlSystem.Application.Abstractions.Messaging;

namespace CollegeControlSystem.Application.Students.DismissStudent
{
    public record DismissStudentCommand(Guid StudentId) : ICommand;
}
