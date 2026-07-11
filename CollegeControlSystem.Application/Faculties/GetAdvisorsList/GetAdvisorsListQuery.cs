using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Application.Abstractions.Pagination;

namespace CollegeControlSystem.Application.Faculties.GetAdvisorsList
{
    public sealed record GetAdvisorsListQuery(
        string? Search,
        Guid? DepartmentId,
        int Page = 1,
        int PageSize = 10
    ) : IQuery<PagedResponse<GetAdvisorsListQueryResponse>>;
}
