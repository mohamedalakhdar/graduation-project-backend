using CollegeControlSystem.Domain.Abstractions;

namespace CollegeControlSystem.Domain.CourseOfferings
{
    public static class CourseOfferingErrors
    {
        public static readonly Error CapacityExceeded = new(
            "CourseOffering.CapacityExceeded",
            "Cannot enroll student. The course has reached its maximum capacity.");

        public static readonly Error InvalidCapacity = new(
            "CourseOffering.InvalidCapacity",
            "Capacity must be greater than zero.");

        public static readonly Error InstructorRequired = new(
            "CourseOffering.InstructorRequired",
            "A valid instructor must be assigned to the offering.");

        public static readonly Error CannotReduceCapacity = new(
            "CourseOffering.CapacityConflict",
            "Cannot reduce capacity below the number of currently enrolled students.");

        public static readonly Error OfferingNotFound = new(
            "CourseOffering.NotFound",
            "Course offering not found.");

        public static readonly Error DuplicateOffering = new(
            "CourseOffering.Duplicate",
            "This course is already offered by the same instructor in the same semester.");

        public static readonly Error AlreadyCancelled = new(
            "CourseOffering.AlreadyCancelled",
            "This course offering has already been cancelled.");

        public static readonly Error HasEnrolledStudents = new(
            "CourseOffering.HasEnrolledStudents",
            "Cannot cancel course offering because students are already enrolled.");

        public static readonly Error HasRegistrations = new(
            "CourseOffering.HasRegistrations",
            "Cannot delete course offering because students are registered.");

        public static readonly Error OfferingCancelled = new(
            "CourseOffering.OfferingCancelled",
            "This course offering has been cancelled and cannot be used.");
    }
}
