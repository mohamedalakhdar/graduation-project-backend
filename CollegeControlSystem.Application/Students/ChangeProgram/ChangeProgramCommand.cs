using CollegeControlSystem.Application.Abstractions.Messaging;

namespace CollegeControlSystem.Application.Students.ChangeProgram
{
    public sealed record ChangeProgramCommand(Guid StudentId, Guid NewProgramId) : ICommand;
}
