using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Bible.API.Data;
using Bible.API.Models;
using Bible.API.DTOs;

namespace Bible.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodayMessageController(BibleContext context) : ControllerBase
    {
        private readonly BibleContext _context = context;

        // GET: api/TodayMessage
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodayMessage>>> GetTodayMessages()
        {
            return await _context.TodayMessages.ToListAsync();
        }
        // POST: api/TodayMessage
        [HttpPost]
        public async Task<ActionResult<ResponseDTO>> PostTodayMessage([FromBody] TodayMessage todayMessage)
        {
            if (todayMessage == null)
                return BadRequest(new ResponseDTO(false, "데이터가 없습니다.", string.Empty));

            var check = await _context.TodayMessages.AnyAsync();

            int id = check ? await _context.TodayMessages.MaxAsync(x => x.Id) + 1 : 1;

            var data = new TodayMessage
            {
                Id = id,
                UserId = todayMessage.UserId,
                Message = todayMessage.Message,
            };

            await _context.TodayMessages.AddAsync(data);

            var result = await _context.SaveChangesAsync();

            if (result == 0) return BadRequest(new ResponseDTO(false, "저장에 실패했습니다.", string.Empty));

            return Ok(new ResponseDTO(true, "성공적으로 저장되었습니다.", data));
        }

        // GET: api/TodayMessage/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TodayMessage>> GetTodayMessage(int id)
        {
            var todayMessage = await _context.TodayMessages.FindAsync(id);

            if (todayMessage == null)
            {
                return NotFound();
            }

            return todayMessage;
        }

        // GET: api/TodayMessage/user/{userId}
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<TodayMessage>> GetUserMessage(string userId)
        {
            var todayMessage = await _context.TodayMessages.OrderByDescending(x => x.Id).FirstOrDefaultAsync(x => x.UserId == userId);

            if (todayMessage == null)
                return NotFound(new ResponseDTO(false, "메시지가 없습니다.", string.Empty));

            return Ok(new ResponseDTO(true, "오늘의 메시지", todayMessage));
        }

        // PUT: api/TodayMessage/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTodayMessage(int id, TodayMessage todayMessage)
        {
            if (id != todayMessage.Id)
            {
                return BadRequest();
            }

            _context.Entry(todayMessage).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TodayMessageExists(id))
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


        // DELETE: api/TodayMessage/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodayMessage(int id)
        {
            var todayMessage = await _context.TodayMessages.FindAsync(id);
            if (todayMessage == null)
            {
                return NotFound();
            }

            _context.TodayMessages.Remove(todayMessage);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TodayMessageExists(int id)
        {
            return _context.TodayMessages.Any(e => e.Id == id);
        }
    }
}
