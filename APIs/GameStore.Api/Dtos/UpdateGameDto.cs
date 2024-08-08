using System.ComponentModel.DataAnnotations;

namespace GameStore.Api.Dtos;

public record class UpdateGameDto(

    [Required(ErrorMessage =" Name is Required!!")]
    [StringLength(100)]
    string Name,

    int GenreId,

    [Range(1,100)]
    decimal Price,

    [Required]
    [DataType(DataType.DateTime)]
    DateOnly ReleaseDate);

