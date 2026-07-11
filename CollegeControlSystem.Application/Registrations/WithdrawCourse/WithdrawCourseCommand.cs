using CollegeControlSystem.Application.Abstractions.Messaging;

namespace CollegeControlSystem.Application.Registrations.WithdrawCourse
{
    public sealed record WithdrawCourseCommand(
        Guid RegistrationId,
        Guid StudentId // Required for security (ownership check)
    ) : ICommand;
}
