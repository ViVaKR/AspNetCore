using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ViVaKR.API.DTOs;

public class ConfirmEmailDTO
{
    [Required]
    [EmailAddress]
    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("replayUrl")]
    public string ReplayUrl { get; set; } = "https://viv.vivabm.com";
}
