using CollegeControlSystem.Domain.Abstractions;

namespace CollegeControlSystem.Domain.Shared
{
    public static class SemsterErrors
    {
        public static Error EmptyTerm => new("Semester.TermEmpty", "Term cannot be empty.");
        public static Error InvalidTerm => new("Semester.InvalidTerm", "Invalid term. Must be Fall, Spring, or Summer.");
        public static Error InvalidYear => new("Semester.InvalidYear", "Year must be between 2020 and 2100.");
    }
}
