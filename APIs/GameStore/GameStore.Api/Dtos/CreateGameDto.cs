using System.ComponentModel.DataAnnotations;

namespace GameStore.Api.Dtos;

public record class CreateGameDto(
    [Required(ErrorMessage =" Name is Required!!")]
    [StringLength(100)]
    string Name,

    [Required(ErrorMessage = "Genre Field is required!!")]
    [StringLength(50)]
    string Genre,

    [Range(1,100)]
    decimal Price,

    [Required]
    [DataType(DataType.DateTime)]
    DateOnly ReleaseDate);
