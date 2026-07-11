namespace CollegeControlSystem.Application.Registrations.SubmitGrades
{
    public sealed record GradeSubmission(
            Guid RegistrationId,
            decimal SemesterWork,
            decimal FinalExam
        );
}
