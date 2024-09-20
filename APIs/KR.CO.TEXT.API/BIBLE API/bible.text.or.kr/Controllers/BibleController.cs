using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Bible.API.Models;
using Bible.API.Data;
using Bible.API.DTOs;
using Bible.API.Helpers;
using System.Security.Permissions;

namespace Bible.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BibleController(BibleContext context) : ControllerBase
    {
        private readonly BibleContext _context = context;

        // GET: api/Bible
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BibleModel>>> GetBible()
        {
            return await _context.Bibles.Include(x => x.Category).ToListAsync();
        }

        // 사용자별로 데이터 가져오기
        // GET: api/Bible/user/5
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<BibleModel>>> GetBibleByUser(string userId)
        {
            var result = await _context.Bibles.Where(x => x.UserId == userId).Include(x => x.Category).ToListAsync();

            if (result == null) return NotFound(new ResponseDTO(false, "데이터가 없습니다.", string.Empty));
            return Ok(result);
        }

        // GET: api/Bible/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BibleModel>> GetBible(int id)
        {
            var bibleModel = await _context.Bibles.FindAsync(id);

            if (bibleModel == null) return NotFound(new ResponseDTO(false, "데이터가 없습니다.", string.Empty));
            bibleModel.Category = await _context.Categories.FindAsync(bibleModel.CategoryId);
            return Ok(bibleModel);
        }

        // PUT: api/Bible/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBible(int id, [FromBody] BibleModel bibleModel)
        {
            if (id != bibleModel.Id)
            {
                return BadRequest(new ResponseDTO(false, "데이터가 없습니다.", string.Empty));
            }

            try
            {
                var target = await _context.Bibles.FindAsync(id);

                if (target == null) return NotFound(new ResponseDTO(false, "데이터가 없습니다.", string.Empty));

                bibleModel.Modified = DateTime.UtcNow.SetKindUtc();
                bibleModel.Created = target.Created;
                _context.Entry(target).CurrentValues.SetValues(bibleModel);


                // _context.Entry(bibleModel).State = EntityState.Modified;

                var result = await _context.SaveChangesAsync();
                if (result == 0) return BadRequest(new ResponseDTO(false, "수정에 실패했습니다.", string.Empty));
                return Ok(new ResponseDTO(true, "성공적으로 수정되었습니다.", string.Empty));

            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseDTO(false, $"원인을 알수 없는 오류 발생: {ex.Message}", string.Empty));
            }
        }

        // POST: api/Bible
        [HttpPost]
        public async Task<ActionResult<ResponseDTO>> PostBible([FromBody] BibleModel bible)
        {
            if (bible == null) return BadRequest(new ResponseDTO(false, "데이터가 없습니다.", string.Empty));

            try
            {
                bool check = await _context.Bibles.AnyAsync();
                int id = check ? await _context.Bibles.MaxAsync(x => x.Id) + 1 : 1;

                var data = new BibleModel
                {
                    Id = id,
                    Title = bible.Title,
                    CategoryId = bible.CategoryId,
                    Chapter = bible.Chapter,
                    Verse = bible.Verse,
                    TextKor = bible.TextKor ?? string.Empty,
                    TextEng = bible.TextEng ?? string.Empty,
                    Comments = bible.Comments ?? string.Empty,
                    Created = DateOnly.FromDateTime(DateTime.Now),
                    Modified = DateTime.UtcNow.SetKindUtc(),
                    UserId = bible.UserId ?? string.Empty,
                    UserName = bible.UserName ?? string.Empty,
                    MyIp = bible.MyIp ?? string.Empty
                };

                await _context.Bibles.AddAsync(data);
                var result = await _context.SaveChangesAsync();

                if (result == 0) return BadRequest(new ResponseDTO(false, "저장에 실패했습니다.", string.Empty));

                return Ok(new ResponseDTO(true, "성공적으로 저장되었습니다.", string.Empty));
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseDTO(false, $"원인을 알수 없는 오류 발생: {ex.Message}", string.Empty));
            }
        }

        // DELETE: api/Bible/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBible(int id)
        {
            var bibleModel = await _context.Bibles.FindAsync(id);
            if (bibleModel == null)
            {
                return NotFound();
            }

            _context.Bibles.Remove(bibleModel);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BibleExists(int id)
        {
            return _context.Bibles.Any(e => e.Id == id);
        }
    }
}
