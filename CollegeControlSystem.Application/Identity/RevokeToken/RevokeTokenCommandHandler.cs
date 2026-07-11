
using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Domain.Abstractions;
using CollegeControlSystem.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CollegeControlSystem.Application.Identity.RevokeToken
{
    internal class RevokeTokenCommandHandler : ICommandHandler<RevokeTokenCommand>
    {
        private readonly UserManager<AppUser> userManager;

        public RevokeTokenCommandHandler(UserManager<AppUser> userManager)
        {
            this.userManager = userManager;
        }
        public async Task<Result> Handle(RevokeTokenCommand request, CancellationToken cancellationToken)
        {
            var user = await userManager.Users.SingleOrDefaultAsync(u => u.RefreshTokens.Any(r => r.Token == request.RefreshToken));
            if (user == null)
                return Result.Failure(IdentityErrors.InvalidToken);

            var oldRefreshToken = user.RefreshTokens.SingleOrDefault(t => t.Token == request.RefreshToken);
            if (!oldRefreshToken.IsActive)
                return Result.Failure(IdentityErrors.InvalidToken);

            oldRefreshToken.RevokedOn = DateTime.UtcNow;
            await userManager.UpdateAsync(user);

            return Result.Success();
        }
    }
}
