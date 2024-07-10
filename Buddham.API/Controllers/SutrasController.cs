using Buddham.API.Data;
using Buddham.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Buddham.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SutrasController(DataContext context) : ControllerBase
    {
        private readonly DataContext _context = context;

        // GET: api/<SutrasController>
        [HttpGet]
        public async Task<IEnumerable<Sutras>> Get()
        {
            var Sutras = await _context.Sutras.AsNoTracking().ToListAsync();
            return Sutras;
        }

        //--> GET api/<SutrasController>/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var sutras = await _context.Sutras.FindAsync(id);

            if (sutras == null)
                return NotFound();

            return Ok(sutras);
        }

        //--> POST api/<SutrasController>, 데이터 추가
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Sutras sutras)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            await _context.Sutras.AddAsync(sutras);
            var result = await _context.SaveChangesAsync();

            if (result > 0)
                return Ok(sutras);
            return BadRequest("Failed to add data");
        }

        // PUT api/<SutrasController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<SutrasController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
