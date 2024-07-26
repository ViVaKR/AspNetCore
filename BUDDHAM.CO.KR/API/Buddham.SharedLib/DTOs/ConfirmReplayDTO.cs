using System.ComponentModel.DataAnnotations;

namespace Buddham.SharedLib.DTOs;

public class ConfirmReplayDTO
{
    [Required]
    [EmailAddress]
    [StringLength(100)]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Token { get; set; } = string.Empty;
}
