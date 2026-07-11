using CollegeControlSystem.Domain.Identity;

namespace CollegeControlSystem.Application.Identity
{
    public interface ITokenGenerator
    {
        Task<string> GenerateJwtTokenAsync(AppUser appUser);
        Domain.Identity.RefreshToken GenereteRefreshToken();
    }
}
