using System;

namespace Bible.API.Models;

public class JWTConfig
{
    /// <summary>
    /// JWT Key
    /// </summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// JWT Issuer
    /// </summary>
    public string Issuer { get; set; } = string.Empty;

    /// <summary>
    /// JWT Audience
    /// </summary>
    public string Audience { get; set; } = string.Empty;

    /// <summary>
    /// JWT Expiration in minutes
    /// </summary>
    public int ExpiraInMinutes { get; set; }

    /// <summary>
    /// JWT Refresh Token Expiration in minutes
    /// </summary>
    public int RefreshTokenValidityIn { get; set; }


}
