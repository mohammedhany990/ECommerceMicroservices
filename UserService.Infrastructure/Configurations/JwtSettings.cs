namespace UserService.Infrastructure.Configurations
{
    public class JwtSettings
    {
        public string Key { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public double AccessTokenDurationMinutes { get; set; }
        public int RefreshTokenDurationDays { get; set; }
    }

}
