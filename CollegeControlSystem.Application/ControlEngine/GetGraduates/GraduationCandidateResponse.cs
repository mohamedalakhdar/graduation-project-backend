
namespace CollegeControlSystem.Application.ControlEngine.GetGraduates
{
    public sealed record GraduationCandidateResponse(
            Guid StudentId,
            string AcademicNumber,
            string StudentName,
            string ProgramName,
            decimal CGPA,
            int EarnedCredits,
            int RequiredCredits,
            bool IsEligible,
            List<string> MissingRequirements // E.g., "Missing Summer Training", "Needs 3 more credits"
        );
}
