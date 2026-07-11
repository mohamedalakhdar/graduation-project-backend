using Microsoft.AspNetCore.Identity;

namespace CollegeControlSystem.Domain.Identity
{
    public class AppUser : IdentityUser
    {
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}
