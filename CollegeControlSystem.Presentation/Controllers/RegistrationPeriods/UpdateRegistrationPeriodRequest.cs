namespace CollegeControlSystem.Presentation.Controllers.RegistrationPeriods
{
    public sealed record UpdateRegistrationPeriodRequest(
        string? Name,
        DateTime? StartDateUtc,
        DateTime? EndDateUtc,
        bool? IsActive,
        string? Term,
        int? Year);
}
