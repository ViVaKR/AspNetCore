using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Buddham.API.Models;

[Table("sutra_images")]
public class SutraImage
{
    [Key]
    [Column("id")]
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [Column("image")]
    [JsonPropertyName("image")]
    [MaxLength]
    [DataType("bytea")]
    public byte[]? Image { get; set; } = null;
}
