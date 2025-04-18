using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ViVaKR.API.DTOs;

public class SignInDTO
{
    [Required]
    [EmailAddress]
    [JsonPropertyName("email")]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [JsonPropertyName("password")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
}
