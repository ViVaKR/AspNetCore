using System.ComponentModel.DataAnnotations;

namespace Buddham.SharedLib.DTOs;

public class UpdateAccountDTO
{
    [Required]
    [DataType(DataType.Text)]
    public string? FullName { get; set; }

    [Required]
    [EmailAddress]
    [DataType(DataType.EmailAddress)]
    public string? Email { get; set; }

    [DataType(DataType.PhoneNumber)]
    public string? PhoneNumber { get; set; }

    [DataType(DataType.Text)]
    [StringLength(100)]
    public string? Address { get; set; }
    public string? Country { get; set; }

}
