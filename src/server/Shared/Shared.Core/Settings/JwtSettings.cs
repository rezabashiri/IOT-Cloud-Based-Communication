namespace Shared.Core.Settings;

public class JwtSettings
{
    public string Key { get; set; }
    public int TokenExpirationInMinutes { get; set; }
    public int RefreshTokenExpirationInDays { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }
}