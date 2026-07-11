using CollegeControlSystem.Application.Abstractions.Messaging;

namespace CollegeControlSystem.Application.Registrations.DropCourse
{
    public sealed record DropCourseCommand(
        Guid RegistrationId,
        Guid StudentId
    ) : ICommand;
}
