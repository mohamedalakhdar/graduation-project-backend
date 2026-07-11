using CollegeControlSystem.Application.Abstractions.Messaging;

namespace CollegeControlSystem.Application.RegistrationPeriods.CreateRegistrationPeriod
{
    public sealed record CreateRegistrationPeriodCommand(
        string Name,
        DateTime StartDateUtc,
        DateTime EndDateUtc,
        bool IsActive,
        string Term,
        int Year) : ICommand<Guid>;
}

//{
//    "name": "Fall 2026 Registration",
//  "startDateUtc": "2026-09-01T00:00:00Z",
//  "endDateUtc": "2026-09-10T23:59:59Z",
//  "isActive": true,
//  "term": "Fall",
//  "year": 2026
//}