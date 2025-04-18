using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ViVaKR.API.Models;

[Table("qna")]
public class QnA
{
    [Key]
    [Column("id")]
    [JsonPropertyName("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [Column("code_id")]
    [JsonPropertyName("codeId")]
    public int CodeId { get; set; }

    [Required]
    [Column("user_id")]
    [JsonPropertyName("userId")]
    [StringLength(450)]
    public string UserId { get; set; } = string.Empty;

    [Required]
    [Column("user_name")]
    [JsonPropertyName("userName")]
    [StringLength(450)]
    public string UserName { get; set; } = string.Empty;

    [Required]
    [MaxLength]
    [Column("qna_text")]
    [JsonPropertyName("qnaText")]
    public string QnaText { get; set; } = string.Empty;

    [Column("created")]
    [JsonPropertyName("created")]
    public DateTime Created { get; set; } = DateTime.Now.ToUniversalTime();

    [Required]
    [Column("my_ip")]
    [JsonPropertyName("myIp")]
    [StringLength(15)]
    public string MyIp { get; set; } = "0.0.0.0";

}
