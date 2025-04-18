using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ViVaKR.API.DTOs;

public class UpdateUserNameDTO
{
    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("newUserName")]
    public string NewUserName { get; set; } = string.Empty;
}
