namespace CollegeControlSystem.Presentation.Controllers.RegistrationPeriods
{
    public sealed record CreateRegistrationPeriodRequest(
        string Name,
        DateTime StartDateUtc,
        DateTime EndDateUtc,
        bool IsActive,
        string Term,
        int Year);
}
