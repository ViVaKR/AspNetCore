using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Buddham.API.Data;
using Buddham.API.Models;
using Buddham.SharedLib.DTOs;
using System.Net;
using System.Net.Sockets;

namespace Buddham.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayGroundController(BuddhaContext context) : ControllerBase
    {
        private readonly BuddhaContext _context = context;

        // GET: api/PlayGround
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlayGround>>> GetPlayGrounds()
        {
            return await _context.PlayGrounds.ToListAsync();
        }

        // POST: api/PlayGround
        [HttpPost]
        public async Task<ActionResult<ResponseDTO>> PostPlayGround([FromBody] PlayGround playGround)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ResponseDTO { Success = false, Message = "Invalid data", Data = null });

            var data = new PlayGround
            {
                Text = playGround.Text,
                CreateAt = DateOnly.FromDateTime(DateTime.UtcNow),
                ModifiedAt = playGround.ModifiedAt.ToUniversalTime(),
                Numbers = playGround.Numbers,
            };

            await _context.PlayGrounds.AddAsync(data);
            var result = await _context.SaveChangesAsync();

            if (result > 0)
                return Ok(new ResponseDTO { Success = true, Message = "Data saved", Data = playGround });
            else
                return BadRequest(new ResponseDTO { Success = false, Message = "Data not saved", Data = null });
        }

        // GET: api/PlayGround/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PlayGround>> GetPlayGround(int id)
        {
            var playGround = await _context.PlayGrounds.FindAsync(id);

            if (playGround == null)
            {
                return NotFound();
            }
            return playGround;
        }

        // PUT: api/PlayGround/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPlayGround(int id, PlayGround playGround)
        {
            if (id != playGround.Id)
            {
                return BadRequest();
            }

            _context.Entry(playGround).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlayGroundExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }



        // DELETE: api/PlayGround/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlayGround(int id)
        {
            var playGround = await _context.PlayGrounds.FindAsync(id);
            if (playGround == null)
            {
                return NotFound();
            }

            _context.PlayGrounds.Remove(playGround);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PlayGroundExists(int id)
        {
            return _context.PlayGrounds.Any(e => e.Id == id);
        }
    }
}
