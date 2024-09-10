using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Bible.API.DTOs;

public class DeleteAccountDTO
{
    [Required]
    [EmailAddress]
    [DataType(DataType.EmailAddress)]
    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    [JsonPropertyName("password")]
    public string Password { get; set; } = string.Empty;

}
