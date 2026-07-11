using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Application.Abstractions.Pagination;

namespace CollegeControlSystem.Application.Departments.GetPrograms
{
    public sealed record GetProgramsQuery(
        string? Search,
        Guid? DepartmentId,
        int Page = 1,
        int PageSize = 10
    ) : IQuery<PagedResponse<ProgramListResponse>>;
}
