using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ViVaBM.API.Data;
using ViVaBM.API.DTOs;
using ViVaBM.API.Helpers;
using ViVaBM.API.Models;

namespace ViVaBM.API.Controllers
{
    [Route("api/code")]
    [ApiController]
    public class CodeController(VivabmDbContext context, IHttpClientFactory httpClientFactory) : ControllerBase
    {
        private readonly VivabmDbContext _context = context;
        private readonly IHttpClientFactory httpClientFactory = httpClientFactory;

        //--> GET: api/code/list

        [HttpGet]
        public async Task<IEnumerable<Code>> GetListAsync()
        {
            var list = await _context.Codes.ToListAsync();
            return list;
        }

        //--> GET api/code/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Code>> GetCodeAsync(int id)
        {
            var code = await _context.Codes.FindAsync(id);
            if (code == null) return NotFound(new CodeResDTO
            {
                IsSuccess = false,
                Message = "요청한 데이터가 없음",
                Data = null
            });
            return Ok(code);
        }

        //--> POST api/code
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Code code)
        {
            if (code == null) return BadRequest(new CodeResDTO
            {
                IsSuccess = false,
                Message = "데이터 없음",
                Data = null
            });
            var created = code.Created ?? DateOnly.FromDateTime(DateTime.Now);
            try
            {
                var id = await _context.Codes.MaxAsync(c => c.Id) + 1;
                var data = new Code
                {
                    Id = id,
                    Title = code.Title,
                    SubTitle = code.SubTitle,
                    Content = code.Content,
                    Note = code.Note,
                    Created = DateOnly.FromDateTime(DateTime.Now),
                    Modified = DateTime.UtcNow.SetKindUtc(),
                    CategoryId = code.CategoryId,
                    UserId = code.UserId,
                    UserName = code.UserName,
                    MyIp = code.MyIp
                };

                await _context.Codes.AddAsync(data);

                var result = await _context.SaveChangesAsync();

                if (result == 0) return BadRequest(new CodeResDTO
                {
                    IsSuccess = false,
                    Message = "새로운 코드 생성 실패",
                    Data = null
                });

                return Ok(new CodeResDTO
                {
                    IsSuccess = true,
                    Message = "새로운 코드 생성 성공",
                    Data = id
                });
            }

            catch (Exception ex)
            {
                return BadRequest(new CodeResDTO
                {
                    IsSuccess = false,
                    Message = $"새로운 코드 생성 실패: {ex.Message}",
                    Data = ex.Message
                });
            }
        }

        //--> PUT, UPDATE, PATCH
        //* api/code/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Code code)
        {
            var target = await _context.Codes.FindAsync(id);

            if (target == null || id != code.Id)
                return NotFound(new CodeResDTO
                {
                    IsSuccess = false,
                    Message = "잘못된 데이터 요청 또는 코드 없음",
                    Data = null
                });
            code.Modified ??= DateTime.UtcNow.SetKindUtc();
            _context.Entry(target).CurrentValues.SetValues(code);

            var result = await _context.SaveChangesAsync();

            if (result == 0)
                return BadRequest(new CodeResDTO
                {
                    IsSuccess = false,
                    Message = "코드 업데이트 실패",
                    Data = null
                });

            return Ok(new CodeResDTO
            {
                IsSuccess = true,
                Message = "코드 업데이트 성공",
                Data = id
            });
        }

        //--> DELETE api/code/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var target = await _context.Codes.FindAsync(id);

            if (target == null)
                return NotFound(new CodeResDTO
                {
                    IsSuccess = false,
                    Message = "삭제할 데이터 없음",
                    Data = null
                });

            _context.Codes.Remove(target);

            var result = await _context.SaveChangesAsync();

            if (result == 0) return BadRequest(new CodeResDTO
            {
                IsSuccess = false,
                Message = "데이터 삭제 실패",
                Data = null
            });

            return Ok(new CodeResDTO
            {
                IsSuccess = true,
                Message = "데이터 삭제 성공",
                Data = id
            });
        }

        // //--> Get Client Public IP Address
        [HttpGet("myip")]
        public async Task<IActionResult> GetMyIp()
        {

            var message = new HttpRequestMessage(HttpMethod.Get, "https://api.ipify.org");

            var client = httpClientFactory.CreateClient();
            HttpResponseMessage response = await client.SendAsync(message);

            if (!response.IsSuccessStatusCode)
                return BadRequest(new CodeResDTO()
                {
                    IsSuccess = false,
                    Message = "IP Address",
                    Data = null
                });


            var ip = await response.Content.ReadAsStringAsync();
            return Ok(new CodeResDTO
            {
                IsSuccess = true,
                Message = "IP Address",
                Data = ip
            });


        }
    }
}
