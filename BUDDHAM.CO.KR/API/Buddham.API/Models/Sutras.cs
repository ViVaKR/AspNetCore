﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Buddham.API.Models;

public class Sutras
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string? Title { get; set; } = string.Empty;

    [MaxLength(250)]
    public string? Subtitle { get; set; } = string.Empty;

    [StringLength(200)]
    public string? Author { get; set; } = "Unknown";

    [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 1)]
    public string? Translator { get; set; } = string.Empty;

    [MaxLength]
    [DisplayName("요약")]
    public string? Summary { get; set; }

    [MaxLength]
    [Required]
    public string? Sutra { get; set; } = string.Empty;

    [MaxLength]
    public string? OriginalText { get; set; } = string.Empty;

    [MaxLength]
    public string? Annotation { get; set; } = string.Empty;

    [EnumDataType(typeof(HangulOrder))]
    public HangulOrder HangulOrder { get; set; }

    [DataType(DataType.DateTime)]
    public DateTime Created { get; set; } = DateTime.Now;

    [StringLength(450)]
    [DisplayName("작가 아이디")]
    public string? UserId { get; set; } = string.Empty;

    [MaxLength]
    [DisplayName("작가 이름/필명")]
    public string? UserName { get; set; } = string.Empty;

    public virtual ICollection<UserText>? UserTexts { get; set; }

    public Sutras()
    {
        UserTexts = [];
    }
}
