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
        private readonly DataContext _context = context; // DI

        //* 목록 *//
        //--> `GET` api/Sutras
        [HttpGet]
        public async Task<IEnumerable<Sutras>> Get()
        {
            var Sutras = await _context.Sutras.AsNoTracking().ToListAsync();
            return Sutras;
        }

        //* 상세보기 *//
        //--> `GET` api/SutrasC/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var sutras = await _context.Sutras.FindAsync(id);
            if (sutras is null) return NotFound();
            return Ok(sutras);
        }

        //* 추가 *//
        //--> `POST` api/<SutrasController>, 데이터 추가
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

        //* 수정 *//
        // PUT api/<SutrasController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Sutras value)
        {
            var sutras = await _context.Sutras.FindAsync(id);

            if (sutras is null) return NotFound();

            sutras.Id = value.Id;
            sutras.Title = value.Title;
            sutras.Subtitle = value.Subtitle;
            sutras.Author = value.Author;
            sutras.Translator = value.Translator;
            sutras.Summary = value.Summary;
            sutras.Sutra = value.Sutra;
            sutras.OriginalText = value.OriginalText;
            sutras.Annotation = value.Annotation;

            var result = await _context.SaveChangesAsync();

            if (result > 0) return Ok(sutras);

            return BadRequest("Failed to update data");
        }

        //* 삭제 *//
        //--> DELETE api/<SutrasController>/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var sutras = await _context.Sutras.FindAsync(id); // 찾아서

            if (sutras is null) return NotFound(); // 없으면

            _context.Sutras.Remove(sutras); // 삭제
            var result = await _context.SaveChangesAsync(); // 저장

            if (result > 0) return Ok(sutras); // 성공

            return BadRequest("Failed to delete data"); // 실패
        }
    }
}
