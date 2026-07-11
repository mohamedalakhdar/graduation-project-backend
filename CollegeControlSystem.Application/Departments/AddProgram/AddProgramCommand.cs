using CollegeControlSystem.Application.Abstractions.Messaging;
namespace CollegeControlSystem.Application.Departments.AddProgram
{
    public sealed record AddProgramCommand(
        Guid DepartmentId,
        string Name,
        int RequiredCredits
    ) : ICommand<Guid>;
}
