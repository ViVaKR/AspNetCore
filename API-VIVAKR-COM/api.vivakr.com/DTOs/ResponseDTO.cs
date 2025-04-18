using System;
using System.Text.Json.Serialization;

namespace ViVaKR.API.DTOs;

public class ResponseDTO(bool isSuccess, string message, object data)
{
    [JsonPropertyName("isSuccess")]
    public bool IsSuccess { get; set; } = isSuccess;

    [JsonPropertyName("message")]
    public string? Message { get; set; } = message;

    [JsonPropertyName("data")]
    public object? Data { get; set; } = data;
}
