using CollegeControlSystem.Application.Registrations.SubmitGrades;

namespace CollegeControlSystem.Presentation.Controllers.Registratoins
{
    public record SubmitGradesRequest(
        Guid OfferingId,
        List<GradeSubmission> Submissions
    );
}
