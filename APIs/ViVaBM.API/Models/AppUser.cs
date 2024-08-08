using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;

namespace ViVaBM.API.Models;

public class AppUser : IdentityUser
{
    [Column("full_name")]
    [JsonPropertyName("fullName")]
    public string? FullName { get; set; }

    [Column("refresh_token")]
    [JsonPropertyName("refreshToken")]
    public string? RefreshToken { get; set; }

    [Column("refresh_token_expiry_time")]
    [JsonPropertyName("refreshTokenExpiryTime")]
    public DateTime? RefreshTokenExpiryTime { get; set; }
}
