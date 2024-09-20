using Buddham.API.Data;
using Buddham.API.Models;
using Buddham.SharedLib.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Buddham.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SutrasController(BuddhaContext context) : ControllerBase
    {
        private readonly BuddhaContext _context = context;

        //* 목록 *//
        //--> `GET` api/Sutras
        [HttpGet]
        public async Task<IEnumerable<Sutra>> Get()
        {
            var Sutras = await _context.Sutras.OrderBy(x => x.Title).ToListAsync();
            return Sutras;
        }

        //* 추가 *//
        //--> `POST` api/Sutras, 데이터 추가
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<ResponseDTO>> Post([FromBody] SutraDTO sutraDTO)
        {
            if (sutraDTO == null)
                return BadRequest(new ResponseDTO { Success = false, Message = "유효하지 않은 데이터입니다.", Data = null });
            if (!ModelState.IsValid) return BadRequest(new ResponseDTO
            {
                Success = false,
                Message = "유효하지 않은 데이터입니다.",
                Data = "모델 상태 오류"
            }); // 유효성 검사

            try
            {
                bool check = await _context.Sutras.AnyAsync();
                int id = check ? await _context.Sutras.MaxAsync(x => x.Id) + 1 : 1;
                var newSutra = new Sutra
                {
                    Id = id,
                    Title = sutraDTO.Title,
                    Subtitle = sutraDTO.Subtitle,
                    Author = sutraDTO.Author,
                    Translator = sutraDTO.Translator,
                    Summary = sutraDTO.Summary,
                    Text = sutraDTO?.Text ?? string.Empty,
                    OriginalText = sutraDTO?.OriginalText,
                    Annotation = sutraDTO?.Annotation,
                    HangulOrder = sutraDTO?.HangulOrder,
                    Created = DateTime.UtcNow,
                    UserId = sutraDTO?.UserId,
                    UserName = sutraDTO?.UserName
                };

                await _context.Sutras.AddAsync(newSutra); // 추가

                var result = await _context.SaveChangesAsync(); // 저장

                if (result > 0) return Ok(new ResponseDTO { Success = true, Message = "경전 사경완료", Data = newSutra }); // 성공

                return BadRequest(new ResponseDTO { Success = false, Message = "경전 사경에 실패하였습니다.", Data = null }); // 실패
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseDTO { Success = false, Message = "알수없는 오류발생: " + ex.Message, Data = null });
            }
        }


        //* 읽기 *//
        //--> `GET` api/Sutras/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ResponseDTO>> Get(int id)
        {
            if (id < 1) return BadRequest(new ResponseDTO { Success = false, Message = "유효하지 않은 데이터입니다.", Data = null }); // 유효성 검사

            var sutras = await _context.Sutras.FindAsync(id);
            if (sutras is null) return NotFound(new ResponseDTO { Success = false, Message = "경전이 존재하지 않습니다.", Data = null }); // 없으면

            return Ok(new ResponseDTO { Success = true, Message = "경전", Data = sutras });
        }

        //* 수정 *//
        //--> PUT api/<SutrasController>/5
        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult<ResponseDTO>> Put(int id, [FromBody] SutraDTO sutraDTO)
        {
            if (sutraDTO is null) return BadRequest(new ResponseDTO
            {
                Success = false,
                Message = "유효하지 않은 데이터입니다.",
                Data = null
            }); // 유효성 검사

            if (!ModelState.IsValid) return BadRequest(new ResponseDTO
            {
                Success = false,
                Message = "유효하지 않은 데이터입니다.",
                Data = "모델 상태 오류"
            }); // 유효성 검사
            var sutras = await _context.Sutras.FindAsync(id);

            if (sutras is null) return NotFound(new ResponseDTO
            {
                Success = false,
                Message = "경전이 존재하지 않습니다.",
                Data = null
            }); // 없으면

            sutras.Id = sutraDTO.Id;
            sutras.Title = sutraDTO.Title;
            sutras.Subtitle = sutraDTO.Subtitle;
            sutras.HangulOrder = sutraDTO.HangulOrder;
            sutras.Author = sutraDTO.Author;
            sutras.Translator = sutraDTO.Translator;
            sutras.Summary = sutraDTO.Summary;
            sutras.Text = sutraDTO.Text;
            sutras.OriginalText = sutraDTO.OriginalText;
            sutras.Annotation = sutraDTO.Annotation;
            sutras.UserId = sutraDTO.UserId;
            sutras.UserName = sutraDTO.UserName;

            var result = await _context.SaveChangesAsync();

            if (result > 0) return Ok(new ResponseDTO
            {
                Success = true,
                Message = "경전 수정완료",
                Data = sutras
            });

            return BadRequest(new ResponseDTO
            {
                Success = false,
                Message = "경전 수정에 실패하였습니다.",
                Data = null
            });
        }

        //* 삭제 *//
        //--> DELETE api/<SutrasController>/5
        [Authorize]
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<ResponseDTO>> Delete(int id)
        {
            if (id < 1) return BadRequest(new ResponseDTO { Success = false, Message = "유효하지 않은 데이터입니다.", Data = null }); // 유효성 검사

            var sutras = await _context.Sutras.FindAsync(id); // 찾아서

            if (sutras is null) return NotFound(new ResponseDTO
            {
                Success = false,
                Message = "경전이 존재하지 않습니다.",
                Data = null
            }); // 없으면

            _context.Sutras.Remove(sutras); // 삭제
            var result = await _context.SaveChangesAsync(); // 저장

            if (result > 0) return Ok(new ResponseDTO
            {
                Success = true,
                Message = "경전 삭제완료",
                Data = sutras
            }); // 성공

            return BadRequest(new ResponseDTO
            {
                Success = false,
                Message = "경전 삭제에 실패하였습니다.",
                Data = null
            }); // 실패
        }
    }
}
