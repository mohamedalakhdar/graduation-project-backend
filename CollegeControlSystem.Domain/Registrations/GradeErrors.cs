using CollegeControlSystem.Domain.Abstractions;

namespace CollegeControlSystem.Domain.Registrations
{

    public static class GradeErrors
    {
        public static Error EmptyLetter => new("Grade.EmptyLetter", "Grade Letter cannot be empty.");
        public static Error InvalidLetter(string letter) => new("Grade.InvalidLetter", $"Invalid grade letter {letter}.");
        public static Error NegativeScore => new("Grade.NegativeScore", "Scores cannot be negative.");
        public static Error ExceededScore => new("Grade.ExceededScore", "Scores cannot exceed 100.");
    public static Error Unauthorized => new("Grades.Unauthorized", "You are not authorized to submit grades for this course offering.");
    public static readonly Error NotFound = new("Grade.NotFound", "Grade not found.");
}
}
