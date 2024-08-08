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

    [MaxLength(500)]
    [Column("memo")]
    [JsonPropertyName("memo")]
    public string? Memo { get; set; }

    [JsonPropertyName("codes")]
    public virtual ICollection<Code>? Codes { get; set; }

    [JsonPropertyName("codeQuestions")]
    public virtual ICollection<CodeQuestion>? CodeQuestions { get; set; }

    public Category()
    {
        Codes = [];
        CodeQuestions = [];
    }
}
