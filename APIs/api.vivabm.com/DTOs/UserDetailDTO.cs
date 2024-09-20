using System.Text.Json.Serialization;

namespace ViVaBM.API.DTOs;

public class UserDetailDTO
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("fullName")]
    public string? FullName { get; set; }

    [JsonPropertyName("email")]
    public string? Email { get; set; }

    [JsonPropertyName("emailConfirmed")]
    public bool EmailConfirmed { get; set; }

    [JsonPropertyName("roles")]
    public string[]? Roles { get; set; }

    [JsonPropertyName("phoneNumber")]
    public string? PhoneNumber { get; set; }

    [JsonPropertyName("twoFactorEnabled")]
    public bool TwoFactorEnabled { get; set; }

    [JsonPropertyName("phoneNumberConformed")]
    public bool PhoneNumberConformed { get; set; }

    [JsonPropertyName("accessFailedCount")]
    public int AccessFailedCount { get; set; }

}
