using CollegeControlSystem.Domain.Abstractions;

namespace CollegeControlSystem.Domain.RegistrationPeriods
{
    public static class RegistrationPeriodErrors
    {
        public static readonly Error NotFound = new(
            "RegistrationPeriod.NotFound",
            "Registration period not found.");

        public static readonly Error NameRequired = new(
            "RegistrationPeriod.NameRequired",
            "Registration period name is required.");

        public static readonly Error InvalidDateRange = new(
            "RegistrationPeriod.InvalidDateRange",
            "Start date must be before end date.");

        public static readonly Error AlreadyActive = new(
            "RegistrationPeriod.AlreadyActive",
            "Registration period is already active.");

        public static readonly Error AlreadyInactive = new(
            "RegistrationPeriod.AlreadyInactive",
            "Registration period is already inactive.");

        public static readonly Error RegistrationClosed = new(
            "RegistrationPeriod.Closed",
            "Registration is currently closed. Please wait for the next registration period.");
    }
}
