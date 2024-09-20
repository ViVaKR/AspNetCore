using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Buddham.API.Models;

[Table("play_grounds")]
public class PlayGround
{
    [Key]
    [Column("id")]
    [JsonPropertyName("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [StringLength(100)]
    [Column("text")]
    [JsonPropertyName("text")]
    public string Text { get; set; } = string.Empty;

    [DataType(DataType.Date)]
    [Column("create_at")]
    [JsonPropertyName("create_at")]
    public DateOnly CreateAt { get; set; } = DateOnly.FromDateTime(DateTime.Now);

    [DataType(DataType.DateTime)]
    [JsonPropertyName("modified_at")]
    [Column("modified_at")]
    public DateTime ModifiedAt { get; set; } = DateTime.Now;

    [Column("numbers", TypeName = "integer[]")]
    [JsonPropertyName("numbers")]
    public int[]? Numbers { get; set; }

}
