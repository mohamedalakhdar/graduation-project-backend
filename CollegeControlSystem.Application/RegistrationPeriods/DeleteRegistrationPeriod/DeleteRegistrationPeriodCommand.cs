using CollegeControlSystem.Application.Abstractions.Messaging;

namespace CollegeControlSystem.Application.RegistrationPeriods.DeleteRegistrationPeriod
{
    public sealed record DeleteRegistrationPeriodCommand(Guid Id) : ICommand;
}
