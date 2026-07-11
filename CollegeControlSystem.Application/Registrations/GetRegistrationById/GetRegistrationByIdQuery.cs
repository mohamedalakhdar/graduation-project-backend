using CollegeControlSystem.Application.Abstractions.Messaging;

namespace CollegeControlSystem.Application.Registrations.GetRegistrationById
{
    public sealed record GetRegistrationByIdQuery(Guid RegistrationId) : IQuery<RegistrationResponse>;
}
