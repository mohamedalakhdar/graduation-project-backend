namespace CollegeControlSystem.Presentation.Controllers.Registratoins
{
    // In a real app with Auth, AdvisorId would come from the User Context (JWT), not the body
    public record ApproveRegistrationRequest(Guid AdvisorId);

}
