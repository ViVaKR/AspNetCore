
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Bible.API.Models;

[Table("bibles")]
public class BibleModel
{
    /// <summary>
    /// 번호
    /// </summary>
    [Key]
    [Column("id")]
    [JsonPropertyName("id")]
    [DisplayName("번호")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    /// 제목
    /// </summary>
    [Column("title")]
    [JsonPropertyName("title")]
    [Display(Name = "제목")]
    [StringLength(250)]
    public string? Title { get; set; }

    /// <summary>
    /// 카테고리 ID (외래키)
    /// </summary>
    [Required]
    [Column("category_id")]
    [JsonPropertyName("categoryId")]
    [DisplayName("성경 구분")]
    public int CategoryId { get; set; }

    /// <summary>
    /// 장
    /// </summary>
    [Display(Name = "장")]
    [Column("chapter")]
    [JsonPropertyName("chapter")]
    public int Chapter { get; set; }

    /// <summary>
    /// 절
    /// </summary>
    [Display(Name = "절")]
    [Column("verse")]
    [JsonPropertyName("verse")]
    public int Verse { get; set; }

    /// <summary>
    /// 본문(한글)
    /// </summary>
    [Display(Name = "본문(한글)")]
    [Column("text_kor")]
    [MaxLength]
    [JsonPropertyName("textKor")]
    public string TextKor { get; set; } = string.Empty;

    /// <summary>
    /// 본문(영어)
    /// </summary>
    [Display(Name = "본문(영어)")]
    [Column("text_eng")]
    [MaxLength]
    [JsonPropertyName("textEng")]
    public string? TextEng { get; set; }

    /// <summary>
    /// 주석
    /// </summary>
    [Display(Name = "주석")]
    [Column("comments")]
    [MaxLength]
    [JsonPropertyName("comments")]
    public string? Comments { get; set; }

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

    /// <summary>
    /// 카테고리, 외래키
    /// </summary>
    // [JsonIgnore]
    [ForeignKey("CategoryId")]
    [JsonPropertyName("category")]
    public virtual Category? Category { get; set; } = null!;
}
