using System.Text.Json.Serialization;

namespace Bible.API.DTOs;

public class RoleResponseDTO
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("totalUsers")]
    public int TotalUsers { get; set; }

    [JsonPropertyName("normalizedName")]
    public string? NormalizedName { get; set; }

    [JsonPropertyName("concurrencyStamp")]
    public string? ConcurrencyStamp { get; set; }
}
