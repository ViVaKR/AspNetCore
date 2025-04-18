using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ViVaKR.API.Models;

[Table("un-subscribe-tokens")]
public class UnSubscribeToken
{
    [Key]
    [Column("id")]
    [JsonPropertyName("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Column("email")]
    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [Column("token")]
    [JsonPropertyName("token")]
    public string Token { get; set; } = string.Empty;

    [Column("expires-at")]
    [JsonPropertyName("expiresAt")]
    public DateTime? ExpiresAt { get; set; } = DateTime.Now.AddDays(1);

    [Column("subscribe-id")]
    public int SubscribeId { get; set; }
}
