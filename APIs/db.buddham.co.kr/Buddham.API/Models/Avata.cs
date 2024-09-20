using Microsoft.AspNetCore.Mvc;

namespace Buddham.API.Models;

public class Avata
{

    [FromForm(Name = "file")]
    public IFormFile? AvataImage { get; set; }

    [FromForm(Name = "name")]
    public string? Name { get; set; }
}
