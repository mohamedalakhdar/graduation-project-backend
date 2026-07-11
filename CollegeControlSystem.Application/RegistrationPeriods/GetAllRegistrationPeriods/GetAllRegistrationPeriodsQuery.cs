using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Application.RegistrationPeriods;

namespace CollegeControlSystem.Application.RegistrationPeriods.GetAllRegistrationPeriods
{
    public sealed record GetAllRegistrationPeriodsQuery()
        : IQuery<List<RegistrationPeriodResponse>>;
}
