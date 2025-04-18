using System.Text.Json.Serialization;
using ViVaKR.API.Models;

namespace ViVaKR.API.DTOs;

public class CodeResDTO
{
    [JsonPropertyName("isSuccess")]
    public bool IsSuccess { get; set; }

    [JsonPropertyName("message")]
    public string? Message { get; set; }

    [JsonPropertyName("data")]
    public object? Data { get; set; }
}
