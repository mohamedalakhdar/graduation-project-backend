using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Application.Abstractions.Pagination;

namespace CollegeControlSystem.Application.Courses.GetCourseList
{
    public sealed record GetCourseListQuery(
        string? Search,
        Guid? DepartmentId,
        int? MinCredits,
        int? MaxCredits,
        int Page = 1,
        int PageSize = 10
    ) : IQuery<PagedResponse<GetCourseListQueryResponse>>;
}
