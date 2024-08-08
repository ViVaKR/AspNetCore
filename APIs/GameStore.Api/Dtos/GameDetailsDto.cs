using System.ComponentModel.DataAnnotations;

namespace GameStore.Api.Dtos;

public record class GameDetailsDto(int Id,
    [Required(ErrorMessage =" Name is Required!!")]
    [StringLength(100)]
    string Name,

    int GenreId,

    [Range(1,100,ErrorMessage ="Price value must be between 1 and 100")]
    decimal Price,

    [Required]
    [DataType(DataType.DateTime)]
    DateOnly ReleaseDate);
