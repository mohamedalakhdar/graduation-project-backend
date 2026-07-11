namespace CollegeControlSystem.Presentation.Controllers.Courses;

public record UpdateCourseRequest(
    Guid DepartmentId,
    string Title,
    string Description,
    int Credits,
    int LectureHours,
    int LabHours);
