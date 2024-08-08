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

        // GET: api/code_question
        [HttpGet("list")]
        public async Task<IEnumerable<CodeQuestion>> Get()
        {
            return await _context.CodeQuestions.Include(x => x.Category).ToListAsync();
        }

        // GET api/<CodeQuestionController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<CodeQuestionController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<CodeQuestionController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<CodeQuestionController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
