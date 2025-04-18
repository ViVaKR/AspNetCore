using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ViVaKR.API.Models;

[Table("user_info")]
public class UserInfo
{
    [Key]
    [Column("id")]
    [JsonPropertyName("id")]
    [StringLength(100)]
    public required string Id { get; set; }

    [Column("avata")]
    [JsonPropertyName("avata")]
    [StringLength(100)]
    public string? Avata { get; set; }
}
