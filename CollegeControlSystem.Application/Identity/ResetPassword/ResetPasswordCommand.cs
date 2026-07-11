using CollegeControlSystem.Application.Abstractions.Messaging;

namespace CollegeControlSystem.Application.Identity.ResetPassword
{
    public record ResetPasswordCommand(string UserId, string Code, string NewPassword) : ICommand<string>;
}
