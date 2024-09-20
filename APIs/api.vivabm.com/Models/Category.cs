using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ViVaBM.API.Models;

[Table("categories")]
public class Category
{
    [Key]
    [Column("id")]
    [JsonPropertyName("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [StringLength(250)]
    [Required]
    [Column("name")]
    [JsonPropertyName("name")]
    public required string Name { get; set; }
}
