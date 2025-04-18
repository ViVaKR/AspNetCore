namespace ViVaKR.API.Models;

public class JwtConfig
{
    /// <summary>
    /// JWT Key
    /// </summary>
    public string Key { get; init; } = string.Empty;

    /// <summary>
    /// JWT Issuer
    /// </summary>
    public string Issuer { get; init; } = string.Empty;

    /// <summary>
    /// JWT Audience
    /// </summary>
    public string Audience { get; init; } = string.Empty;

    /// <summary>
    /// JWT Expiration in minutes
    /// </summary>
    public int ExpiraInMinutes { get; init; }

    /// <summary>
    /// JWT Refresh Token Expiration in minutes
    /// </summary>
    public int RefreshTokenValidityIn { get; init; }
}
