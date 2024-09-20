using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Bible.API.DTOs;

public class ConfirmEmailDTO
{
    [Required]
    [EmailAddress]
    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;
}
