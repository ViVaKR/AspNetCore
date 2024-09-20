using System.Text.Json.Serialization;
using ViVaBM.API.Models;

namespace ViVaBM.API.DTOs;

public class CodeResDTO
{
    [JsonPropertyName("isSuccess")]
    public bool IsSuccess { get; set; }

    [JsonPropertyName("message")]
    public string? Message { get; set; }

    [JsonPropertyName("data")]
    public object? Data { get; set; }
}
