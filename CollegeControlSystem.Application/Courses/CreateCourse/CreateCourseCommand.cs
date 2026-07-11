using CollegeControlSystem.Application.Abstractions.Messaging;

namespace CollegeControlSystem.Application.Courses.CreateCourse
{
    public sealed record CreateCourseCommand(
        Guid DepartmentId,
        string Code,          // e.g. "CCE 123"
        string Title,
        string Description,
        int Credits,
        int LectureHours,
        int LabHours
    ) : ICommand<Guid>;
}
