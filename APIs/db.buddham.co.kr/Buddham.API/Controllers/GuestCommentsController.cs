using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Buddham.API.Data;
using Buddham.API.Models;

namespace Buddham.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GuestCommentsController : ControllerBase
    {
        private readonly BuddhaContext _context;

        public GuestCommentsController(BuddhaContext context)
        {
            _context = context;
        }

        // GET: api/GuestComments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GuestComment>>> GetGuestComments()
        {
            return await _context.GuestComments.ToListAsync();
        }

        // GET: api/GuestComments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GuestComment>> GetGuestComment(int id)
        {
            var guestComment = await _context.GuestComments.FindAsync(id);

            if (guestComment == null)
            {
                return NotFound();
            }

            return guestComment;
        }

        // PUT: api/GuestComments/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGuestComment(int id, GuestComment guestComment)
        {
            if (id != guestComment.Id)
            {
                return BadRequest();
            }

            _context.Entry(guestComment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GuestCommentExists(id))
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

        // POST: api/GuestComments
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<GuestComment>> PostGuestComment(GuestComment guestComment)
        {
            _context.GuestComments.Add(guestComment);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetGuestComment", new { id = guestComment.Id }, guestComment);
        }

        // DELETE: api/GuestComments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGuestComment(int id)
        {
            var guestComment = await _context.GuestComments.FindAsync(id);
            if (guestComment == null)
            {
                return NotFound();
            }

            _context.GuestComments.Remove(guestComment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool GuestCommentExists(int id)
        {
            return _context.GuestComments.Any(e => e.Id == id);
        }
    }
}
