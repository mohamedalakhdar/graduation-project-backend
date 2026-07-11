using CollegeControlSystem.Application.Abstractions.Messaging;

namespace CollegeControlSystem.Application.Identity.RefreshToken
{
    public record RefreshTokenCommand(string RefreshToken) : ICommand<AuthResponse>;
}
