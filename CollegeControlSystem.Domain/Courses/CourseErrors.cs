using CollegeControlSystem.Domain.Abstractions;

namespace CollegeControlSystem.Domain.Courses
{
    public static class CourseErrors
    {
        public static readonly Error CodeEmpty = new("Course.CodeEmpty", "Course code cannot be empty.");
        public static readonly Error CodeInvalidFormat = new("Course.CodeInvalid", "Course code must follow format 'ABC 123' (3 Letters + 3 Digits).");
        public static readonly Error CreditsInvalid = new("Course.CreditsInvalid", "Credits must be greater than zero.");
        public static readonly Error PrerequisiteCycle = new("Course.PrerequisiteCycle", "A course cannot be a prerequisite of itself.");
        public static readonly Error PrerequisiteDuplicate = new("Course.PrerequisiteDuplicate", "This prerequisite is already added.");
        public static readonly Error InvalidHours = new("Course.InvalidHours","Lecture hours and lab hours must be zero or greater.");
        public static readonly Error DuplicateCode = new("Course.DuplicateCode", "A course with this code already exists.");
        public static readonly Error CourseNotFound = new("Course.NotFound", "Target course not found.");
        public static readonly Error PrerequisiteNotFound = new("Course.PrerequisiteNotFound", "Prerequisite course does not exist.");
        public static readonly Error HasOfferings = new("Course.HasOfferings", "Cannot delete course because it has existing offerings.");
        public static readonly Error HasRegistrations = new("Course.HasRegistrations", "Cannot delete course because students are registered.");
    }
}
