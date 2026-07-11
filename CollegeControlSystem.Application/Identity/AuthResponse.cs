namespace CollegeControlSystem.Application.Identity
{
    public class AuthResponse
    {
          public string ? Token { get; set; }
          public string ? RefreshToken { get; set; }
          public DateTime ? RefreshTokenExpiresOn { get; set; }
    }
               
}
