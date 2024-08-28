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

    [Required]
    [StringLength(450)]
    [JsonPropertyName("subTitle")]
    [Column("sub_title")]
    public required string SubTitle { get; set; }

    [MaxLength]
    [JsonPropertyName("content")]
    [Column("content")]
    public required string Content { get; set; }

    [MaxLength]
    [JsonPropertyName("note")]
    [Column("note")]
    public string? Note { get; set; }

    [Required]
    [JsonPropertyName("categoryId")]
    [Column("category_id")]
    public int CategoryId { get; set; }

    [JsonPropertyName("created")]
    [Column("created")]
    [DataType(DataType.Date)]
    public DateOnly? Created { get; set; }

    [JsonPropertyName("modified")]
    [Column("modified")]
    [DataType(DataType.DateTime)]
    public DateTime? Modified { get; set; }


    [StringLength(450)]
    [Column("user_id")]
    [JsonPropertyName("userId")]
    public string? UserId { get; set; }

    [StringLength(256)]
    [Column("user_name")]
    [JsonPropertyName("userName")]
    public string? UserName { get; set; }

    [StringLength(20)]
    [Column("myip")]
    [JsonPropertyName("myIp")]
    public string? MyIp { get; set; }
}
