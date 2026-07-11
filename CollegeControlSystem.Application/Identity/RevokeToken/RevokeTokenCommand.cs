
using CollegeControlSystem.Application.Abstractions.Messaging;

namespace CollegeControlSystem.Application.Identity.RevokeToken
{
    public record RevokeTokenCommand(string RefreshToken):ICommand;
}
