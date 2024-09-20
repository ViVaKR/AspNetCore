using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Buddham.API.Data;
using Buddham.API.Models;
using Buddham.SharedLib.DTOs;

namespace Buddham.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodaySutraController(BuddhaContext context) : ControllerBase
    {
        private readonly BuddhaContext _context = context;

        //--> GET: api/TodaySutra
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodaySutra>>> GetTodaySutras()
        {
            return await _context.TodaySutras.ToListAsync();
        }

        //--> GET: api/TodaySutra/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TodaySutra>> GetTodaySutra(int id)
        {
            var todaySutra = await _context.TodaySutras.FindAsync(id);

            if (todaySutra == null) return NotFound();

            return Ok(todaySutra);
        }

        //--> PUT: api/TodaySutra/5
        [HttpPut("{id}")]
        public async Task<ActionResult<ResponseDTO>> PutTodaySutra(int id, TodaySutra todaySutra)
        {
            if (id != todaySutra.Id) return BadRequest(new ResponseDTO { Message = "잘못된 ID 입니다.", Data = id, Success = false });

            _context.Entry(todaySutra).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!TodaySutraExists(id))
                    return NotFound();
                else
                    return BadRequest(new ResponseDTO { Message = $"DB 오류가 발생했습니다. {ex.Message}", Data = id, Success = false });
            }

            return NoContent();
        }

        //--> POST: api/TodaySutra
        [HttpPost]
        public async Task<ActionResult<TodaySutra>> PostTodaySutra([FromBody] TodaySutra todaySutra)
        {
            if (!ModelState.IsValid) return BadRequest(new ResponseDTO { Message = "입력값이 올바르지 않습니다.", Data = todaySutra, Success = false });
            // if (todaySutra.Image == null) return BadRequest(new ResponseDTO { Message = "이미지가 없습니다.", Data = todaySutra, Success = false });

            var id = _context.TodaySutras.Any() ? _context.TodaySutras.Max(x => x.Id) + 1 : 1;
            var data = new TodaySutra
            {
                Id = id,
                Image = todaySutra.Image,
                Title = todaySutra.Title,
                Text = todaySutra.Text,
                UserId = todaySutra.UserId
            };


            _context.TodaySutras.Add(todaySutra);

            await _context.SaveChangesAsync();

            return Ok(new ResponseDTO { Message = "오늘의 수트라가 성공적으로 등록되었습니다.", Data = todaySutra.Id, Success = true });
        }

        // DELETE: api/TodaySutra/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodaySutra(int id)
        {
            var todaySutra = await _context.TodaySutras.FindAsync(id);
            if (todaySutra == null)
            {
                return NotFound();
            }

            _context.TodaySutras.Remove(todaySutra);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TodaySutraExists(int id)
        {
            return _context.TodaySutras.Any(e => e.Id == id);
        }
    }
}
