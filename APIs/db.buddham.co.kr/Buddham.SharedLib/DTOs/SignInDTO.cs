using System.ComponentModel.DataAnnotations;

namespace Buddham.SharedLib.DTOs;

public class SignInDTO
{
    [Required]
    [EmailAddress]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
}
