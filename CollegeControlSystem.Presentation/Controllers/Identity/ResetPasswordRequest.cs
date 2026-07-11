namespace CollegeControlSystem.Presentation.Controllers.Identity
{
    public record ResetPasswordRequest(string UserId, string Code, string NewPassword);
}
