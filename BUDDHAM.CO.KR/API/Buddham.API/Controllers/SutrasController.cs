using Buddham.API.Data;
using Buddham.API.Models;
using Microsoft.AspNetCore.Authorization;
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
            var Sutras = await _context.Sutras.OrderBy(x => x.Title).ToListAsync();
            return Sutras;
        }

        //* 추가 *//
        //--> `POST` api/Sutras, 데이터 추가
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Sutras sutras)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState); // 유효성 검사

            await _context.Sutras.AddAsync(sutras); // 추가

            var result = await _context.SaveChangesAsync(); // 저장

            if (result > 0) return Ok(sutras); // 성공

            return BadRequest("경전 사경에 실패하였습니다."); // 실패
        }


        //* 읽기 *//
        //--> `GET` api/Sutras/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var sutras = await _context.Sutras.FindAsync(id);
            if (sutras is null) return NotFound();
            return Ok(sutras);
        }

        //* 수정 *//
        //--> PUT api/<SutrasController>/5
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Sutras value)
        {
            var sutras = await _context.Sutras.FindAsync(id);

            if (sutras is null) return NotFound();

            sutras.Id = value.Id;
            sutras.Title = value.Title;
            sutras.Subtitle = value.Subtitle;
            sutras.HangulOrder = value.HangulOrder;
            sutras.Author = value.Author;
            sutras.Translator = value.Translator;
            sutras.Summary = value.Summary;
            sutras.Sutra = value.Sutra;
            sutras.OriginalText = value.OriginalText;
            sutras.Annotation = value.Annotation;
            sutras.UserId = value.UserId;
            sutras.UserName = value.UserName;

            var result = await _context.SaveChangesAsync();

            if (result > 0) return Ok(sutras);

            return BadRequest("Failed to update data");
        }

        //* 삭제 *//
        //--> DELETE api/<SutrasController>/5
        [Authorize]
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
