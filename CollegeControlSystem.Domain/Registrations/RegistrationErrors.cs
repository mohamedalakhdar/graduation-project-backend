using CollegeControlSystem.Domain.Abstractions;

namespace CollegeControlSystem.Domain.Registrations
{
    public  static class RegistrationErrors
    {
        public static readonly Error AlreadyCompleted = new(
        "Registration.AlreadyCompleted",
        "Cannot modify a registration that is already completed.");

        public static readonly Error NotPending = new(
            "Registration.NotPending",
            "Only pending registrations can be approved or rejected.");

        public static readonly Error AlreadyDropped = new(
            "Registration.AlreadyDropped",
            "This registration has already been dropped.");

        public static readonly Error DuplicateRegistration = new(
            "Registration.Duplicate",
            "Student is already registered for this course.");

        public static  Error Overload(int maxAllowed) => new(
            "Registration.Overload",
            $"Cannot register. Total credits would exceed the limit of {maxAllowed} hours.");
        public static  Error PrerequisiteNotMet(Guid courseId) => new(
            "Registration.PrerequisiteNotMet",
            $"Prerequisite course {courseId} has not been completed.");

        public static readonly Error NotFound = new(
            "Registration.NotFound",
            "Registration request not found.");

         public static readonly Error Unauthorized = new(
            "Registration.Unauthorized",
            "You are not the assigned advisor for this student.");

    }
}
