namespace CollegeControlSystem.Application.CourseOfferings.GetRoster
{
    public sealed record StudentRosterResponse(
            Guid RegistrationId,
            string AcademicNumber,
            string StudentName,
            string Status,
            bool IsRetake,
            decimal? SemesterWork,
            decimal? FinalExam,
            string? LetterGrade
        );
}
