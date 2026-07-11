using CollegeControlSystem.Application.Abstractions.Messaging;

namespace CollegeControlSystem.Application.RegistrationPeriods.UpdateRegistrationPeriod
{
    public sealed record UpdateRegistrationPeriodCommand(
        Guid Id,
        string? Name,
        DateTime? StartDateUtc,
        DateTime? EndDateUtc,
        bool? IsActive,
        string? Term,
        int? Year) : ICommand;
}
