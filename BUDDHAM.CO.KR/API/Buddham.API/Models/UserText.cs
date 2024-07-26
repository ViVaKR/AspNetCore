using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Buddham.API.Models;

public class UserText
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [DataType(DataType.DateTime)]
    public DateTime DateOfWrite { get; set; } = DateTime.Now;

    [Required]
    [StringLength(450)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength]
    public string Text { get; set; } = string.Empty;

    [Required]
    public int SutraId { get; set; }

    [ForeignKey("SutraId")]
    public virtual Sutras? Sutras { get; set; }

}
