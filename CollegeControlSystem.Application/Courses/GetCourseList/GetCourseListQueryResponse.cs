
namespace CollegeControlSystem.Application.Courses.GetCourseList
{
    public sealed record GetCourseListQueryResponse(
    Guid Id,
    string Code,
    string Title,
    int Credits
    );
}
