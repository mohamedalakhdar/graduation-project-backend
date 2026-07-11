using CollegeControlSystem.Application.Abstractions.Messaging;

namespace CollegeControlSystem.Application.Registrations.SubmitGrades
{
    public sealed record SubmitGradesCommand(
            Guid OfferingId,
            Guid InstructorId, // Passed from the JWT Token for security
            List<GradeSubmission> Submissions
        ) : ICommand;
}
