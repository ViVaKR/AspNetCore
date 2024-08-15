using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ViVaBM.API.Data;
using ViVaBM.API.Models;

namespace ViVaBM.API.Controllers
{
    [Route("api/code_question")]
    [ApiController]
    public class CodeQuestionController(VivabmDbContext context) : ControllerBase
    {
        private readonly VivabmDbContext _context = context;

        //--> GET: api/code_question
        [HttpGet("list")]
        public async Task<IEnumerable<CodeQuestion>> Get()
        {
            return await _context.CodeQuestions.Include(x => x.Category).ToListAsync();
        }

        //--> GET api/code_question/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CodeQuestion>> Get(int id)
        {
            var codeQuestion = await _context.CodeQuestions.FindAsync(id);
            if (codeQuestion == null)
            {
                return NotFound();
            }
            codeQuestion.Category = await _context.Categories.FindAsync(codeQuestion.CategoryId);
            return Ok(codeQuestion);
        }

        //--> POST api/<CodeQuestionController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CodeQuestion data)
        {
            if (data == null)
            {
                return BadRequest();
            }
            await _context.CodeQuestions.AddAsync(data);
            await _context.SaveChangesAsync();
            return Ok(new { message = "데이터 추가 완료" });
        }

        //--> PUT api/<CodeQuestionController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] CodeQuestion data)
        {
            if (id != data.Id || id != data.Id)
            {
                return BadRequest();
            }
            // _context.Entry(data).State = EntityState.Modified;
            _context.Entry(data).CurrentValues.SetValues(data);
            await _context.SaveChangesAsync();
            return Ok(new { message = "데이터 업데이트 완료" });
        }

        //--> DELETE api/<CodeQuestionController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var codeQuestion = await _context.CodeQuestions.FindAsync(id);
            if (codeQuestion == null)
            {
                return NotFound();
            }
            _context.CodeQuestions.Remove(codeQuestion);
            await _context.SaveChangesAsync();
            return Ok(new { message = "데이터 삭제 완료" });
        }
    }
}
