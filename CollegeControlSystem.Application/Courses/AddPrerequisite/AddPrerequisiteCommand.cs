using CollegeControlSystem.Application.Abstractions.Messaging;

namespace CollegeControlSystem.Application.Courses.AddPrerequisite
{
    public sealed record AddPrerequisiteCommand(
        Guid CourseId,
        Guid PrerequisiteCourseId
    ) : ICommand;
}
