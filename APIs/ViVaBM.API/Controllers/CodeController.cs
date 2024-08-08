using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ViVaBM.API.Data;
using ViVaBM.API.Models;

namespace ViVaBM.API.Controllers
{
    [Route("api/code")]
    [ApiController]
    public class CodeController(VivabmDbContext context) : ControllerBase
    {
        private readonly VivabmDbContext _context = context;

        //--> GET: api/code/list
        [HttpGet("list")]
        public async Task<IEnumerable<Code>> GetListAsync()
        {
            var list = await _context.Codes.Include(x => x.Category).ToListAsync();
            return list;
        }

        //--> GET api/code/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Code>> GetCodeAsync(int id)
        {
            var code = await _context.Codes.FindAsync(id);
            if (code == null) return NotFound(new { message = "Code not found" });

            code.Category = await _context.Categories.FindAsync(code.CategoryId);
            return Ok(code);
        }

        //--> POST api/code
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Code value)
        {
            if (value == null) return BadRequest(new { message = "Invalid data" });

            await _context.Codes.AddAsync(value);
            var result = await _context.SaveChangesAsync();
            if (result == 0) return BadRequest(new { message = "Failed to create code" });
            return Ok(new { message = "Code created" });
        }

        //--> PUT api/code/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Code value)
        {
            var code = await _context.Codes.FindAsync(id);
            if (code == null || id != value.Id) return NotFound(new { message = "Code not found" });

            // code.Title = value.Title;
            // code.Content = value.Content;
            // code.Created = value.Created;
            // code.Note = value.Note;
            // code.AppUserId = value.AppUserId;
            // code.CategoryId = value.CategoryId;
            // _context.Codes.Update(code);

            _context.Entry(code).CurrentValues.SetValues(value);
            // _context.Entry(code).State = EntityState.Modified;

            var result = await _context.SaveChangesAsync();
            if (result == 0) return BadRequest(new { message = "Failed to update code" });
            return Ok(new { message = "Code updated" });
        }

        //--> DELETE api/code/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var code = await _context.Codes.FindAsync(id);
            if (code == null) return NotFound(new { message = "Code not found" });

            _context.Codes.Remove(code);
            var result = await _context.SaveChangesAsync();
            if (result == 0) return BadRequest(new { message = "Failed to delete code" });
            return Ok(new { message = "Code deleted" });
        }
    }
}
