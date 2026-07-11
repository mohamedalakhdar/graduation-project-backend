namespace CollegeControlSystem.Application.ControlEngine.GetWarnings
{
    public sealed record AcademicWarningResponse(
            Guid StudentId,
            string AcademicNumber,
            string StudentName,
            string ProgramName,
            decimal CGPA,
            int ConsecutiveWarnings,
            string AcademicStatus
        );
}
