using System.Text.Json.Serialization;

namespace ViVaKR.API.DTOs;

public class SendMailDTO(string subject, string message)
{
    [JsonPropertyName("subject")]
    public string? Subject { get; set; } = subject;

    [JsonPropertyName("message")]
    public string? Message { get; set; } = message;

}
