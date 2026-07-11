namespace CollegeControlSystem.Presentation.Controllers.Courses
{
    public record CreateCourseRequest(
        Guid DepartmentId,
        string Code,
        string Title,
        string Description,
        int Credits,
        int LectureHours,
        int LabHours);
}
