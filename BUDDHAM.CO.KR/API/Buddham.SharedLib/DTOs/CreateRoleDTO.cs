using System.ComponentModel.DataAnnotations;

namespace Buddham.SharedLib.DTOs;

public class CreateRoleDTO
{
    [Required(ErrorMessage = "Role name is required")]
    public string? RoleName { get; set; }
}
