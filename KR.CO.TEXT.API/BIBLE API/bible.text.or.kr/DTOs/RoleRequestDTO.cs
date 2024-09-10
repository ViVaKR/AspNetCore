using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Bible.API.DTOs;

public class RoleRequestDTO
{
    [Required(ErrorMessage = "역할 이름을 입력하세요.")]
    [JsonPropertyName("roleName")]
    public string? RoleName { get; set; }
}
