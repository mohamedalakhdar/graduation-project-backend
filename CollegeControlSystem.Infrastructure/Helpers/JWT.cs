namespace CollegeControlSystem.Infrastructure.Helpers
{
    internal sealed class JWT
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string SecretKey { get; set; }
        public float ExpireAfterInMinute { get; set; }
    }
}
