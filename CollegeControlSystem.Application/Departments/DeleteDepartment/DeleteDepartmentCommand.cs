using CollegeControlSystem.Application.Abstractions.Messaging;

namespace CollegeControlSystem.Application.Departments.DeleteDepartment
{
    public sealed record DeleteDepartmentCommand(Guid DepartmentId) : ICommand;
}
