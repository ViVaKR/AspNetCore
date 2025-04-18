using System.Text.Json.Serialization;

namespace ViVaKR.API.DTOs;

public class AssignRoleRequestDTO
{
    [JsonPropertyName("userId")]
    public required string UserId { get; set; }

    [JsonPropertyName("roleId")]
    public required string RoleId { get; set; }

}
