using System.ComponentModel.DataAnnotations;

namespace Buddham.SharedLib.DTOs;

public class ForgetPasswordDTO
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}
