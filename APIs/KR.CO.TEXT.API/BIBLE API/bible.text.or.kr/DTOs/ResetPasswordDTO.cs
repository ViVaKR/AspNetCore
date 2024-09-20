using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Bible.API.DTOs;

public class ResetPasswordDTO
{
    [Required]
    [EmailAddress]
    [JsonPropertyName("email")]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [JsonPropertyName("token")]
    public string Token { get; set; } = string.Empty;

    [Required]
    [JsonPropertyName("newPassword")]
    public string NewPassword { get; set; } = string.Empty;
}
