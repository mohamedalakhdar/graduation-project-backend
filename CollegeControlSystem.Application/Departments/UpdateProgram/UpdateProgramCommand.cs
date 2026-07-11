using CollegeControlSystem.Application.Abstractions.Messaging;

namespace CollegeControlSystem.Application.Departments.UpdateProgram
{
    public sealed record UpdateProgramCommand(
        //Guid DepartmentId,
        Guid ProgramId,
        string Name
    ) : ICommand;
}
