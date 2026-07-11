using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Application.Abstractions.Pagination;

namespace CollegeControlSystem.Application.Departments.GetDepartments
{
    public sealed record GetDepartmentsQuery(
        string? Search,
        int Page = 1,
        int PageSize = 10
    ) : IQuery<PagedResponse<DepartmentResponse>>;
}
