using CollegeControlSystem.Application.Abstractions.Messaging;

namespace CollegeControlSystem.Application.Identity.ForgetPassword
{
    public record ForgetPasswordCommand(string Email,string BaseUrl) : ICommand<string>;
}
