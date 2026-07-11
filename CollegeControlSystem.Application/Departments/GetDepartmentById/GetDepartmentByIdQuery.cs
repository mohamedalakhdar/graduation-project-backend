using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Application.Departments.GetDepartments;

namespace CollegeControlSystem.Application.Departments.GetDepartmentById
{
    public record GetDepartmentByIdQuery(Guid DepartmentId) : IQuery<DepartmentResponse>;
}
