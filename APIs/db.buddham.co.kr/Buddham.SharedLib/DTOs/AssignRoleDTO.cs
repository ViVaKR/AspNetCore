using System.Text.Json.Serialization;

namespace Buddham.SharedLib.DTOs;

public class AssignRoleDTO
{
    [JsonPropertyName("userId")]
    public string UserId { get; set; } = null!;

    [JsonPropertyName("roleId")]
    public string RoleId { get; set; } = null!;
}
