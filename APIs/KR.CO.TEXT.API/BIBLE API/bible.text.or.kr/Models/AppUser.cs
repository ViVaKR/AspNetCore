using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;

namespace Bible.API.Models;

public class AppUser : IdentityUser
{
    [JsonPropertyName("fullName")]
    public string? FullName { get; set; }

    [JsonPropertyName("refreshToken")]
    public string? RefreshToken { get; set; }

    [JsonPropertyName("refreshTokenExpiryTime")]
    public DateTime? RefreshTokenExpiryTime { get; set; }
}
