namespace CollegeControlSystem.Application.Registrations.GetGrade;

public sealed record GetGradeResponse(
    Guid RegistrationId,
    decimal SemesterWorkGrade,
    decimal FinalGrade,
    decimal TotalGrade,
    string LetterGrade,
    decimal GradePoints,
    bool IsPassing,
    bool IsRetake
);
