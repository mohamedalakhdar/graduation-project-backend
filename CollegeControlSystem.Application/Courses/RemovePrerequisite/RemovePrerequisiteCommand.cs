using CollegeControlSystem.Application.Abstractions.Messaging;

namespace CollegeControlSystem.Application.Courses.RemovePrerequisite
{
    public sealed record RemovePrerequisiteCommand(Guid CourseId, Guid PrerequisiteCourseId) : ICommand;
}
