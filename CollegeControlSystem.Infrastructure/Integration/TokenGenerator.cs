using CollegeControlSystem.Application.Identity;
using CollegeControlSystem.Domain.Identity;
using CollegeControlSystem.Infrastructure.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace CollegeControlSystem.Infrastructure.Integration
{
    internal class TokenGenerator : ITokenGenerator
    {
        private readonly UserManager<AppUser> userManager;
        private readonly IOptionsMonitor<JWT> JWTConfigs;

        public TokenGenerator(UserManager<AppUser> userManager, IOptionsMonitor<JWT> jWTConfigs)
        {
            this.userManager = userManager;
            JWTConfigs = jWTConfigs;
        }
        public async Task<string> GenerateJwtTokenAsync(AppUser appUser)
        {

            var userClaims = await userManager.GetClaimsAsync(appUser);
            var roles = await userManager.GetRolesAsync(appUser);
            var roleClaims = new List<Claim>();

            foreach (var role in roles)
                roleClaims.Add(new Claim(Keys.RolesKey, role));


            var claims = new[]
            {
                        new Claim(JwtRegisteredClaimNames.Sub, appUser.UserName),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Email, appUser.Email),
                        new Claim(Keys.UserIdKey, appUser.Id)
                    }
            .Union(userClaims)
            .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JWTConfigs.CurrentValue.SecretKey));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: JWTConfigs.CurrentValue.Issuer,
                audience: JWTConfigs.CurrentValue.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(JWTConfigs.CurrentValue.ExpireAfterInMinute),
                signingCredentials: signingCredentials);

            return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);


        }
        public RefreshToken GenereteRefreshToken()
        {

            var randomNum = new byte[32];
            using var generator = RandomNumberGenerator.Create();
            generator.GetBytes(randomNum);

            return new RefreshToken
            {
                ExpiresOn = DateTime.UtcNow.AddDays(15),
                Token = Convert.ToBase64String(randomNum),
                CreatedOn = DateTime.UtcNow
            };
        }
    }
}
