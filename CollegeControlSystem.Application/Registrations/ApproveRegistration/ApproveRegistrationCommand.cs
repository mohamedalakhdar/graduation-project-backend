using CollegeControlSystem.Application.Abstractions.Messaging;

namespace CollegeControlSystem.Application.Registrations.ApproveRegistration
{
    public sealed record ApproveRegistrationCommand(
        Guid RegistrationId,
        Guid AdvisorId // The ID of the advisor performing the action
    ) : ICommand;
}
