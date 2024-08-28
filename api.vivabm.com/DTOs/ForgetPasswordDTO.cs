using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ViVaBM.API.DTOs;

public class ForgetPasswordDTO
{
    [Required]
    [EmailAddress]
    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;
}
