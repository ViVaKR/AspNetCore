using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ViVaBM.API.Models;

[Table("demo")]
public class Demo
{
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Column("title")]
    [StringLength(250)]
    public string? Title { get; set; }
}
