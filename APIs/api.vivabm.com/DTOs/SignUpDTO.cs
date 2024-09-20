using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ViVaBM.API.DTOs;

public class SignUpDTO
{
    [Required]
    [JsonPropertyName("fullName")]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [JsonPropertyName("email")]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [JsonPropertyName("password")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    public List<string>? Roles { get; set; }
}
