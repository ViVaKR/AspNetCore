using Buddha.Data;
using Buddha.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Buddha.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DemoController(ApplicationDbContext context) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Demo>>> GetList()
    {
        var data = await context.Demos.ToListAsync();
        return Ok(data);
    }
}
