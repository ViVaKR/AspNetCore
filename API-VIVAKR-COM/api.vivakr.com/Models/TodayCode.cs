using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ViVaKR.API.Models;

[Table("today_code")]
public class TodayCode
{
    [Key]
    [Column("id")]
    [JsonPropertyName("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Column("image")]
    [JsonPropertyName("image")]
    [MaxLength]
    [DataType("bytea")]
    public byte[]? Image { get; set; } = null;

    [Column("title")]
    [JsonPropertyName("title")]
    [MaxLength(500)]
    public string? Title { get; set; }

    [Column("text")]
    [JsonPropertyName("text")]
    [MaxLength]
    public string Text { get; set; } = "-";

    [Column("user_id")]
    [JsonPropertyName("userId")]
    [MaxLength(50)]
    public required string UserId { get; set; }
}
