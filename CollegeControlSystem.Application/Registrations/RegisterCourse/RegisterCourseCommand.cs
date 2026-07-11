using CollegeControlSystem.Application.Abstractions.Messaging;

namespace CollegeControlSystem.Application.Registrations.RegisterCourse
{
    public sealed record RegisterCourseCommand(
        Guid StudentId,
        Guid CourseOfferingId
    ) : ICommand<Guid>;
}
