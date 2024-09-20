using System.ComponentModel.DataAnnotations;

namespace Buddham.SharedLib.DTOs;

public class ChangePasswordDTO
{
    [Required]
    [EmailAddress]
    [DataType(DataType.EmailAddress)]
    public string? Email { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    public string? CurrentPassword { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string? NewPassword { get; set; }
}
