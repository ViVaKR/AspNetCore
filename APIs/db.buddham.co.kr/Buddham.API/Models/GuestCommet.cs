using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Buddham.API.Models;

[Table("guest_comments")]
public class GuestComment
{
    [Key]
    [Column("id")]
    [JsonPropertyName("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [MaxLength]
    [Column("comments")]
    [JsonPropertyName("comments")]
    public string Comments { get; set; } = string.Empty;

    [Required]
    [Column("create_at")]
    [JsonPropertyName("createAt")]
    [DataType(DataType.DateTime)]
    public DateTime CreateAt { get; set; } = DateTime.Now;

    [Required]
    [Column("sutra_id")]
    [JsonPropertyName("sutraId")]
    public int SutraId { get; set; }

    [Required]
    [Column("user_id")]
    [JsonPropertyName("userId")]
    public string? UserId { get; set; }

    [Required]
    [Column("user_name")]
    [JsonPropertyName("userName")]
    public string? UserName { get; set; }
}
