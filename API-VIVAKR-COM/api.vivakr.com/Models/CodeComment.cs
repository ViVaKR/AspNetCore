using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ViVaKR.API.Models;

[Table("code_comment")]
public class CodeComment
{
    [Key]
    [Column("id")]
    [JsonPropertyName("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [MaxLength]
    [Column("comments")]
    [JsonPropertyName("comments")]
    public string Comments { get; set; } = string.Empty;

    [Required]
    [Column("code_id")]
    [JsonPropertyName("codeId")]
    public int CodeId { get; set; }

    [Required]
    [StringLength(50)]
    [Column("user_id")]
    [JsonPropertyName("userId")]
    public string? UserId { get; set; }

    [Required]
    [StringLength(50)]
    [Column("user_name")]
    [JsonPropertyName("userName")]
    public string? UserName { get; set; }

    [Column("my_ip")]
    [JsonPropertyName("myIp")]
    [StringLength(15)]
    public string MyIp { get; set; } = "0.0.0.0";
}
