using CollegeControlSystem.Domain.Abstractions;

namespace CollegeControlSystem.Domain.Registrations.Events
{
    public sealed record RegistrationCreatedDomainEvent(Guid RegistrationId) : IDomainEvent;
}
