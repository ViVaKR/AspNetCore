using System.ComponentModel.DataAnnotations;

namespace Buddham.SharedLib.DTOs;

public class ConfirmEmailDTO
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}
