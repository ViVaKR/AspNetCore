using System.Text.Json.Serialization;

namespace ViVaKR.API.DTOs;

public class AvataDTO
{
    [JsonPropertyName("userId")]
    public string? UserId { get; set; }

    [JsonPropertyName("avataUrl")]
    public string AvataUrl { get; set; } = string.Empty;
}
