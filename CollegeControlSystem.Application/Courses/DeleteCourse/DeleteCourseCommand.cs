using CollegeControlSystem.Application.Abstractions.Messaging;

namespace CollegeControlSystem.Application.Courses.DeleteCourse;

public sealed record DeleteCourseCommand(Guid CourseId) : ICommand;
