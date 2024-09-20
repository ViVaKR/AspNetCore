using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Bible.API.Models;

[Table("today_message")]
public class TodayMessage
{
    [Key]
    [Column("id")]
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    [Column("user_id")]
    [JsonPropertyName("userId")]
    public string UserId { get; set; } = string.Empty;

    [Required]
    [MaxLength]
    [Column("message")]
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
}
