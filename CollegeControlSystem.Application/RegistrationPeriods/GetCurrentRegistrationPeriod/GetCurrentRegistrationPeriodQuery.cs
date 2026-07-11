using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Application.RegistrationPeriods;

namespace CollegeControlSystem.Application.RegistrationPeriods.GetCurrentRegistrationPeriod
{
    public sealed record GetCurrentRegistrationPeriodQuery()
        : IQuery<RegistrationPeriodResponse>;
}
