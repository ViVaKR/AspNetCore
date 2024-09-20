using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication;

namespace ViVaBM.API.Models;

public class ExternalLogin
{
    [Required]
    [EmailAddress]
    [JsonPropertyName("email")]
    public required string Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [JsonPropertyName("password")]
    public required string Password { get; set; }

    [Display(Name = "Remember me?")]
    [JsonPropertyName("rememberMe")]
    public bool RememberMe { get; set; }

    [JsonPropertyName("returnUrl")]
    public string? ReturnUrl { get; set; }

    [JsonPropertyName("externalLogins")]
    public IList<AuthenticationScheme>? ExternalLogins { get; set; }
}
