using CollegeControlSystem.Application.Abstractions.Messaging;

namespace CollegeControlSystem.Application.Departments.DeleteProgram
{
    public sealed record DeleteProgramCommand(
        //Guid DepartmentId,
        Guid ProgramId
    ) : ICommand;
}
