using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ViVaBM.API.Models;

[Table("codes")]
public class Code
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [JsonPropertyName("id")]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [StringLength(450)]
    [JsonPropertyName("title")]
    [Column("title")]
    public required string Title { get; set; }

    [MaxLength]
    [JsonPropertyName("content")]
    [Column("content")]
    public required string Content { get; set; }

    [Required]
    [DataType(DataType.DateTime)]
    [JsonPropertyName("created")]
    [Column("created")]
    public DateTime Created { get; set; } = DateTime.Now;

    [MaxLength]
    [JsonPropertyName("note")]
    [Column("note")]
    public string? Note { get; set; }

    [Required]
    [JsonPropertyName("categoryId")]
    [Column("category_id")]
    public int CategoryId { get; set; }

    [ForeignKey(nameof(CategoryId))]
    [JsonPropertyName("category")]
    [Column("category")]
    public virtual Category? Category { get; set; }
}
