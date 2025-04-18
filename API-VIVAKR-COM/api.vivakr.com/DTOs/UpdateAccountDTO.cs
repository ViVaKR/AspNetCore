using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ViVaKR.API.DTOs;

public class UpdateAccountDTO
{
    [Required]
    [DataType(DataType.Text)]
    [JsonPropertyName("fullName")]
    public string? FullName { get; set; }

    [Required]
    [EmailAddress]
    [DataType(DataType.EmailAddress)]
    [JsonPropertyName("email")]
    public string? Email { get; set; }

    [DataType(DataType.PhoneNumber)]
    [JsonPropertyName("phoneNumber")]
    public string? PhoneNumber { get; set; }

    [DataType(DataType.Text)]
    [StringLength(100)]
    [JsonPropertyName("address")]
    public string? Address { get; set; }

    [JsonPropertyName("country")]
    public string? Country { get; set; }

}
