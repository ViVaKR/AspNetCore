using System.ComponentModel.DataAnnotations;

namespace Buddham.SharedLib.DTOs;

public class RegisterDTO
{
    [Required]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    public List<string>? Roles { get; set; }
}
