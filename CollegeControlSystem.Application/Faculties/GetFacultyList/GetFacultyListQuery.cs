using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Application.Abstractions.Pagination;
using CollegeControlSystem.Domain.Faculties;

namespace CollegeControlSystem.Application.Faculties.GetFacultyList
{
    public sealed record GetFacultyListQuery(
        string? Search,
        Guid? DepartmentId,
        FacultyStatus? Status,
        int Page = 1,
        int PageSize = 10
    ) : IQuery<PagedResponse<GetFacultyListQueryResponse>>;
}
