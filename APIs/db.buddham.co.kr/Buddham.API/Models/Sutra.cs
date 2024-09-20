using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Buddham.SharedLib.Models;

namespace Buddham.API.Models;

[Table("sutras")]
public class Sutra
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
    [Required]
    [StringLength(500)]
    [DisplayName("제목")]
    [Column("title")]
    [JsonPropertyName("title")]
    public string? Title { get; set; } = string.Empty;

    /// <summary>
    /// 부제목
    /// </summary>
    [Column("subtitle")]
    [JsonPropertyName("subtitle")]
    [StringLength(250)]
    [DisplayName("부제목")]
    public string? Subtitle { get; set; } = string.Empty;

    /// <summary>
    /// 저자
    /// </summary>
    [Column("author")]
    [JsonPropertyName("author")]
    [StringLength(200)]
    [DisplayName("저자")]
    public string? Author { get; set; } = "Unknown";

    /// <summary>
    /// 번역자
    /// </summary>
    [Column("translator")]
    [JsonPropertyName("translator")]
    [StringLength(100)]
    [DisplayName("번역자")]
    public string? Translator { get; set; } = string.Empty;

    /// <summary>
    /// 요약
    /// </summary>
    [Column("summary")]
    [JsonPropertyName("summary")]
    [MaxLength]
    [DisplayName("요약")]
    public string? Summary { get; set; } = string.Empty;

    /// <summary>
    /// 불경본문
    /// </summary>
    [Required]
    [Column("text")]
    [JsonPropertyName("text")]
    [DisplayName("불경본문")]
    [MaxLength]
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// 원문
    /// </summary>
    [MaxLength]
    [Column("original_text")]
    [JsonPropertyName("originalText")]
    [DisplayName("원문")]
    public string? OriginalText { get; set; } = string.Empty;

    /// <summary>
    /// 주석
    /// </summary>
    [MaxLength]
    [Column("annotation")]
    [DisplayName("주석")]
    [JsonPropertyName("annotation")]
    public string? Annotation { get; set; } = string.Empty;

    /// <summary>
    /// 한글순서
    /// </summary>
    [Column("hangul_order")]
    [DisplayName("한글순서")]
    [EnumDataType(typeof(HangulOrder))]
    [JsonPropertyName("hangulOrder")]
    public HangulOrder? HangulOrder { get; set; }

    /// <summary>
    /// 작성일
    /// </summary>
    [Column("created")]
    [DataType(DataType.DateTime)]
    [JsonPropertyName("created")]
    public DateTime Created { get; set; } = DateTime.Now;

    /// <summary>
    /// 작성자 아이디
    /// </summary>
    [StringLength(50)]
    [Column("user_id")]
    [JsonPropertyName("userId")]
    [DisplayName("작가 아이디")]
    public string? UserId { get; set; } = string.Empty;

    /// <summary>
    /// 작성자 이름
    /// </summary>
    [StringLength(50)]
    [Column("user_name")]
    [JsonPropertyName("userName")]
    [DisplayName("작가")]
    public string? UserName { get; set; } = string.Empty;
}
