using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ViVaKR.API.Models;

[Table("subscribes")]
public class Subscribe
{
    [Key]
    [Column("id")]
    [JsonPropertyName("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Column("email")]
    [JsonPropertyName("email")]
    [DataType(DataType.EmailAddress)]
    [MaxLength(100)]
    public string Email { get; set; } = string.Empty;

    [Column("created")]
    [JsonPropertyName("created")]
    [DataType(DataType.Date)]
    public DateOnly Created { get; set; } = new DateOnly();
}
