using CollegeControlSystem.Application.Abstractions.Messaging;
namespace CollegeControlSystem.Application.Departments.CreateDepartment
{
    public sealed record CreateDepartmentCommand(
        string Name,
        string? Description
    ) : ICommand<Guid>;
}
