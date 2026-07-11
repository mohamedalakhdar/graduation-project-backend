using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Domain.Abstractions;
using CollegeControlSystem.Domain.Identity;
using Microsoft.AspNetCore.Identity;

namespace CollegeControlSystem.Application.Identity.ResetPassword
{
    internal class ResetPasswordCommandHandler : ICommandHandler<ResetPasswordCommand, string>
    {
        private readonly UserManager<AppUser> userManager;

        public ResetPasswordCommandHandler(UserManager<AppUser> userManager)
        {
            this.userManager = userManager;
        }
        public async Task<Result<string>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByIdAsync(request.UserId);
            if (user == null)
            {
                return Result<string>.Failure(IdentityErrors.NotFoundUser);
            }

            // Decode the token before using it
            var decodedCode = Uri.UnescapeDataString(request.Code);

            var result = await userManager.ResetPasswordAsync(user, decodedCode, request.NewPassword);
            if (result.Succeeded)
            {
                return Result<string>.Success("Password Reset successfully");
            }
            else
            {
                return Result<string>.Failure(IdentityErrors.PasswordResetFailed);
            }
        }
    }
}
