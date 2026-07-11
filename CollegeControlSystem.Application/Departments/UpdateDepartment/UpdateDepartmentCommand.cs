using CollegeControlSystem.Application.Abstractions.Messaging;

namespace CollegeControlSystem.Application.Departments.UpdateDepartment
{
    public sealed record UpdateDepartmentCommand(
        Guid DepartmentId,
        string Name,
        string? Description
    ) : ICommand;
}
