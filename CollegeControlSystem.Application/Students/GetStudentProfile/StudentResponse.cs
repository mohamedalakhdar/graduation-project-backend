
namespace CollegeControlSystem.Application.Students.GetStudentProfile
{
    public sealed record StudentResponse(
        Guid Id,
        string FullName,
        string AcademicNumber,
        string ProgramName,
        decimal CGPA,
        string Status, // GoodStanding, Warning, etc.
        string Level,  // Freshman, Sophomore, etc.
        int MaxCreditsAllowed, // Calculated dynamically via your Domain Logic,
        string NationalId,
        int CompletedCredits,
        int TotalProgramCredits
    );
}
