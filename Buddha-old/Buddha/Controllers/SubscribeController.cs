using Buddha.Data;
using Buddha.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Buddha.Controllers;

[Route("api/subscribe")]
[ApiController]
public class SubscribeController( BuddhaDbContext dbContext ) : ControllerBase
{
    [HttpGet("list")]
    public async Task<IEnumerable<Subscribe>> Get()
    {
        try
        {
            var query = dbContext.Subscribes.AsQueryable();
            var data = await query.ToListAsync();
            return data;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }
    }
}