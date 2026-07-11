namespace CollegeControlSystem.Application.RegistrationPeriods
{
    public sealed record RegistrationPeriodResponse(
        Guid Id,
        string Name,
        DateTime StartDateUtc,
        DateTime EndDateUtc,
        bool IsActive,
        string Term,
        int Year,
        DateTime CreatedAtUtc);
}
