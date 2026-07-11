using CollegeControlSystem.Application.Abstractions.Messaging;
namespace CollegeControlSystem.Application.Registrations.GetPendingRegistrations
{
    public sealed record GetPendingRegistrationsQuery(
        Guid AdvisorId
    ) : IQuery<List<PendingRegistrationResponse>>;
}
