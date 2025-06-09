using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Buddha.Models;

[Table("subscribes")]
public class Subscribe
{
    [Key]
    [Column("id")]
    [JsonPropertyName("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; init; }

    [Column("email")]
    [JsonPropertyName("email")]
    [DataType(DataType.EmailAddress)]
    [MaxLength(100)]
    public string Email { get; init; } = string.Empty;

    [Column("created")]
    [JsonPropertyName("created")]
    [DataType(DataType.Date)]
    public DateOnly Created { get; init; }
    
    [Column("is_active")]
    [JsonPropertyName("isActive")]
    public bool IsActive { get; set; }
}