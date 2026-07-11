using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Application.Registrations;

namespace CollegeControlSystem.Application.Registrations.GetStudentRegistrations
{
    public sealed record GetStudentRegistrationsQuery(Guid StudentId) : IQuery<List<RegistrationResponse>>;
}
