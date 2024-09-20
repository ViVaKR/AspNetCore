using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Buddham.SharedLib.DTOs;

public class CreateRoleDTO
{
    [Required(ErrorMessage = "Role name is required")]
    [JsonPropertyName("roleName")]
    public string? RoleName { get; set; }
}
