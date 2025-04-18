using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ViVaKR.API.Helpers;
using ViVaKR.API.Data;
using ViVaKR.API.Models;

namespace ViVaKR.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QnAController(VivaKRDbContext context) : ControllerBase
    {
        private readonly VivaKRDbContext _context = context;

        // GET: api/QnA
        [HttpGet]
        public async Task<ActionResult<IEnumerable<QnA>>> GetQnAs()
        {
            return await _context.QnAs.ToListAsync();
        }

        // GET: api/QnA/5
        [HttpGet("{codeId}")]
        public async Task<ActionResult<IEnumerable<QnA>>> GetQnA(int codeId)
        {
            if (!_context.QnAs.Any(x => x.CodeId == codeId))
                NotFound($"({codeId}) 해당 자료가 없습니다.");
            var qnA = await _context.QnAs.Where(x => x.CodeId == codeId).ToListAsync();
            return qnA;
        }

        // POST: api/QnA
        [HttpPost("ask")]
        public async Task<ActionResult<QnA>> PostQnA(QnA qnA)
        {
            var id = _context.QnAs.Any() ? await _context.QnAs.MaxAsync(c => c.Id) + 1 : 1;
            var qna = new QnA
            {
                Id = id,
                CodeId = qnA.CodeId,
                UserId = qnA.UserId,
                UserName = qnA.UserName,
                QnaText = qnA.QnaText,
                Created = DateTime.UtcNow.SetKindUtc(),
                MyIp = qnA.MyIp
            };
            await _context.QnAs.AddAsync(qna);

            if (await _context.SaveChangesAsync() > 0)
                return Ok(qna);

            return BadRequest();
        }

        // PUT: api/QnA/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutQnA(int id, QnA qnA)
        {
            if (id != qnA.Id)
                return BadRequest();

            _context.Entry(qnA).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!QnAExists(id))
                    return NotFound();
                else
                    return BadRequest();
            }
            return NoContent();
        }

        // DELETE: api/QnA/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQnA(int id)
        {
            var qnA = await _context.QnAs.FindAsync(id);
            if (qnA == null)
                return NotFound();

            _context.QnAs.Remove(qnA);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool QnAExists(int id)
        {
            return _context.QnAs.Any(e => e.Id == id);
        }
    }
}
