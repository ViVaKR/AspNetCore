using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ViVaKR.API.Models;

[Table("file_manager")]
public class FileManager
{
    [Key]
    [Column("id")]
    [JsonPropertyName("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [Column("user_id")]
    [JsonPropertyName("userId")]
    [MaxLength(50)]
    public string? UserId { get; set; } = string.Empty;

    [Column("file_name")]
    [JsonPropertyName("fileName")]
    [MaxLength]
    public string FileName { get; set; } = "-";

    [Column("file_path")]
    [JsonPropertyName("filePath")]
    [MaxLength]
    public string FilePath { get; set; } = "-";

}
