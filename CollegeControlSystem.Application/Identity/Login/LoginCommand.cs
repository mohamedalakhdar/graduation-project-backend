using CollegeControlSystem.Application.Abstractions.Messaging;
namespace CollegeControlSystem.Application.Identity.Login
{
    public record LoginCommand(string Email, string Password):ICommand<AuthResponse>;
}
