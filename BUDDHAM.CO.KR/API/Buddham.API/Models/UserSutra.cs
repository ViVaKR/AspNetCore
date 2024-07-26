using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Buddham.API.Models;

public class UserSutra
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [MaxLength]
    public string? Sutra { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.DateTime)]
    public DateTime Created { get; set; } = DateTime.Now;


}
