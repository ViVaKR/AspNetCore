using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ViVaBM.API.Models;

[Table("code_questions")]
public class CodeQuestion
{
    [Key]
    [Column("id")]
    [JsonPropertyName("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [StringLength(450)]
    [Column("title")]
    [JsonPropertyName("title")]
    public required string Title { get; set; }

    [Required]
    [Column("content")]
    [JsonPropertyName("content")]
    [MaxLength]
    public required string Content { get; set; }

    [Required]
    [DataType(DataType.DateTime)]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    [Column("created")]
    [JsonPropertyName("created")]
    public DateTime Created { get; set; } = DateTime.Now;

    [Column("user_id")]
    [JsonPropertyName("userId")]
    [StringLength(450)]
    public required string UserId { get; set; }

    [Required]
    [Column("category_id")]
    [JsonPropertyName("categoryId")]
    public int CategoryId { get; set; }

    [ForeignKey(nameof(CategoryId))]
    [Column("category")]
    [JsonPropertyName("category")]
    public virtual Category? Category { get; set; }
}
