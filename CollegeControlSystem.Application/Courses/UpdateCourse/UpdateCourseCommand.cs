using CollegeControlSystem.Application.Abstractions.Messaging;

namespace CollegeControlSystem.Application.Courses.UpdateCourse;

public sealed record UpdateCourseCommand(
    Guid CourseId,
    Guid DepartmentId,
    string Title,
    string Description,
    int Credits,
    int LectureHours,
    int LabHours
) : ICommand;
