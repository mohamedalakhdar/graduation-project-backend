namespace CollegeControlSystem.Application.Identity.GetCurrentUser
{
    public record GetCurrentUserResponse(string UserId, string UserName, string Email, string PhoneNumber,string role);
}