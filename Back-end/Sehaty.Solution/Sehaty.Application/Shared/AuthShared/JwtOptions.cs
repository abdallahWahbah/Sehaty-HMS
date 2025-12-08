namespace Sehaty.Application.Shared.AuthShared
{
    public class JwtOptions
    {
        public string Key { get; set; }

        public string Issuer { get; set; }

        public string Audience { get; set; }
        public double AccessTokenExpirationInMinutes { get; set; }

        public double RefreshTokenExpirationInDays { get; set; }
    }
}
