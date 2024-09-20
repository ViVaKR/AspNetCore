using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ViVaBM.API.DTOs;

public class ChangePasswordDTO
{
    [Required]
    [EmailAddress]
    [DataType(DataType.EmailAddress)]
    [JsonPropertyName("email")]
    public string? Email { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    [JsonPropertyName("currentPassword")]
    public string? CurrentPassword { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [JsonPropertyName("newPassword")]
    public string? NewPassword { get; set; }
}
