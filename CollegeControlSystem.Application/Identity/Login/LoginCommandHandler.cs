using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Domain.Abstractions;
using CollegeControlSystem.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CollegeControlSystem.Application.Identity.Login
{
    public class LoginCommandHandler : ICommandHandler<LoginCommand, AuthResponse>
    {
        private readonly UserManager<AppUser> userManager;
        private readonly ITokenGenerator tokenGenerator;

        public LoginCommandHandler(UserManager<AppUser> userManager, ITokenGenerator tokenGenerator)
        {
            this.userManager = userManager;
            this.tokenGenerator = tokenGenerator;
        }

        public async Task<Result<AuthResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            // to include the RefreshTokens 
            var user = await userManager.Users
                                    .Include(u => u.RefreshTokens)
                                    .FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null)
                return Result<AuthResponse>.Failure(IdentityErrors.InvalidCredentials);

            var result = await userManager.CheckPasswordAsync(user, request.Password);
            if (!result)
                return Result<AuthResponse>.Failure(IdentityErrors.InvalidCredentials);

            var token = await tokenGenerator.GenerateJwtTokenAsync(user);


            var authResult = new AuthResponse
            {
                Token = token
            };
            // check if user has already active refresh token 
            // so no need to give him new refresh token
            if (user.RefreshTokens.Any(r => r.IsActive))
            {
                // TODO: check this 
                var UserRefreshToken = user.RefreshTokens.FirstOrDefault(r => r.IsActive);
                authResult.RefreshToken = UserRefreshToken.Token;
                authResult.RefreshTokenExpiresOn = UserRefreshToken.ExpiresOn;
            }

            // if he does not
            // generate new refreshToken
            else
            {
                var refreshToken = tokenGenerator.GenereteRefreshToken();
                authResult.RefreshToken = refreshToken.Token;
                authResult.RefreshTokenExpiresOn = refreshToken.ExpiresOn;

                // then save it in db
                user.RefreshTokens.Add(refreshToken);
                await userManager.UpdateAsync(user);
            }

            return Result<AuthResponse>.Success(authResult);
        }
    }
}
