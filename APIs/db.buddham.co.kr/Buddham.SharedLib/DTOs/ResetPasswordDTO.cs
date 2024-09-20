using System.ComponentModel.DataAnnotations;

namespace Buddham.SharedLib.DTOs;

public class ResetPasswordDTO
{
    [Required]
    [EmailAddress]
    [StringLength(100)]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Token { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    public string NewPassword { get; set; } = string.Empty;
}
