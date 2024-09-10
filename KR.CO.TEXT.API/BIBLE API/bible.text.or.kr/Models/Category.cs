using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Bible.API.Models;

[Table("categories")]
public class Category
{
    /// <summary>
    /// 카테고리 ID
    /// </summary>
    [Key]
    [Column("id")]
    [JsonPropertyName("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    /// 성경 구분
    /// </summary>
    [Column("testament")]
    [JsonPropertyName("testament")]
    [Required]
    [EnumDataType(typeof(Testament))]
    public Testament Testament { get; set; }

    /// <summary>
    /// 영어 이름
    /// </summary>
    [Required]
    [StringLength(50)]
    [Column("eng_name")]
    [JsonPropertyName("engName")]
    public string EngName { get; set; } = string.Empty;

    /// <summary>
    /// 한글 이름
    /// </summary>
    [Required]
    [StringLength(50)]
    [Column("kor_name")]
    [JsonPropertyName("korName")]
    public string KorName { get; set; } = string.Empty;

    /// <summary>
    /// 영어 약어
    /// </summary>
    [Required]
    [Column("eng_abbreviation")]
    [JsonPropertyName("engAbbreviation")]
    [StringLength(20)]
    public string? EngAbbreviation { get; set; }

    /// <summary>
    /// 한글 약어
    /// </summary>
    [Column("kor_abbreviation")]
    [JsonPropertyName("korAbbreviation")]
    [StringLength(20)]
    public string? KorAbbreviation { get; set; }

    /// <summary>
    /// 장 수
    /// </summary>
    [Required]
    [Column("chapter_count")]
    [JsonPropertyName("chapterCount")]
    public int ChapterCount { get; set; }

    /// <summary>
    /// 절 수
    /// </summary>
    [Required]
    [Column("verse_count")]
    [JsonPropertyName("verseCount")]
    public int VerseCount { get; set; }

    /// <summary>
    /// 순서
    /// </summary>
    [Column("order")]
    [JsonPropertyName("order")]
    public int Order { get; set; }

    /// <summary>
    /// 설명
    /// </summary>
    [Column("description")]
    [JsonPropertyName("description")]
    [MaxLength]
    public string? Description { get; set; }

    /// <summary>
    /// 성경 목록
    /// </summary>
    [JsonIgnore]
    public virtual ICollection<BibleModel>? Bibles { get; set; } = [];
}


public enum Testament
{
    OLD,
    NEW
}
