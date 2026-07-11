using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Domain.Abstractions;
using CollegeControlSystem.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CollegeControlSystem.Application.Identity.RefreshToken
{
    internal class RefreshTokenCommandHandler : ICommandHandler<RefreshTokenCommand, AuthResponse>
    {
        private readonly UserManager<AppUser> userManager;
        private readonly ITokenGenerator tokenGenerator;

        public RefreshTokenCommandHandler(UserManager<AppUser> userManager,ITokenGenerator token)
        {
            this.userManager = userManager;
            this.tokenGenerator = token;
        }
        public async Task<Result<AuthResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            // ensure there is user has this refresh token
            var user = await userManager.Users.SingleOrDefaultAsync(u => u.RefreshTokens.Any(r => r.Token == request.RefreshToken));
            if (user == null)
                return  Result<AuthResponse>.Failure(IdentityErrors.InvalidToken);

            // ensure this token is active
            var oldRefreshToken = user.RefreshTokens.SingleOrDefault(t => t.Token == request.RefreshToken);
            if (!oldRefreshToken.IsActive)
                return Result<AuthResponse>.Failure(IdentityErrors.InvalidToken);

            // if all things well
            //revoke old refresh token
            oldRefreshToken.RevokedOn = DateTime.UtcNow;

            // generate new refresh token and add it to db
            var newRefreshToken = tokenGenerator.GenereteRefreshToken();
            user.RefreshTokens.Add(newRefreshToken);
            await userManager.UpdateAsync(user);

            // generate new JWT Token
            var jwtToken = await tokenGenerator.GenerateJwtTokenAsync(user);

            return Result<AuthResponse>.Success(new AuthResponse
            {
                Token = jwtToken,
                RefreshToken = newRefreshToken.Token,
                RefreshTokenExpiresOn = newRefreshToken.ExpiresOn
            });
        }
    }
}
