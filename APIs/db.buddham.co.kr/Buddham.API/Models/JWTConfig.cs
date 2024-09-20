namespace Buddham.API.Models;

public class JWTConfig
{
    public string Key { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int ExpiraInMinutes { get; set; }
    public int RefreshTokenValidityIn { get; set; }
}
