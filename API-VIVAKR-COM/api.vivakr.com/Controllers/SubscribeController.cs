using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Authorization;
using ViVaKR.API.Data;
using ViVaKR.API.DTOs;
using ViVaKR.API.Interfaces;
using ViVaKR.API.Models;

namespace ViVaKR.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SubscribeController(VivaKRDbContext context, IEmailService emailService) : ControllerBase
{
    private readonly VivaKRDbContext _context = context;
    private readonly IEmailService _emailService = emailService;

    //? api/subscribe/list?limit= 0
    [Authorize(Roles = "Admin")]
    [HttpGet("list")]
    public async Task<ActionResult<IEnumerable<Subscribe>?>> GetList([FromQuery] int limit = 10)
    {
        try
        {
            IQueryable<Subscribe> query = _context.Subscribes.OrderByDescending(c => c.Id);
            var data = limit < 1
            ? await query.ToListAsync()
            : await query.Take(limit).ToListAsync();
            return Ok(data);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    //? api/subscribe/new
    [HttpPost("new")]
    public async Task<IActionResult> Create([FromBody] CreateSubscribeDto subscribeDto)
    {
        try
        {
            // 중복 이메일 체크 (Subscribes 테이블에서 확인)
            var existingSubscription = await _context.Subscribes.AnyAsync(s => s.Email == subscribeDto.Email);
            if (existingSubscription)
            {
                return BadRequest(new { message = "이미 가입한 메일입니다." });
            }

            var data = new Subscribe
            {
                Email = subscribeDto.Email,
                Created = DateOnly.FromDateTime(DateTime.UtcNow),
            };

            // 엔티티 추가 (ID는 자동 생성됨)
            await _context.Subscribes.AddAsync(data);
            await _context.SaveChangesAsync();

            await _emailService.SendEmailAsync(data.Email, "ViVaKR 구독완료", "환영합니다!<br />" +
            $"{data.Email}님 " +
            "ViVaKR 사이트 구독하여 주심을 진심으로 감사드립니다.<br />" +
            "운영진의 판단하에 구독자님들에게 유익한 정보이며 공유가 필요한 정보는 비 정기적으로 메일을 드리겠습니다. <br />" +
            "감사합니다. 사이트 운영자 배상 <br />" +
            "https://viv.vivabm.com");
            return Ok(new { id = data.Id, email = data.Email, Create = data.Created, message = "구독 완료되었습니다." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"서버 오류: {ex.Message}" });
        }
    }


    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Subscribe subscribe)
    {
        // ID 불일치 체크
        if (id != subscribe.Id)
        {
            return BadRequest(new { message = "요청 ID와 가입정보 ID가 일치하지 않습니다." });
        }

        // DB에서 기존 엔티티 조회
        var existingSubscribe = await _context.Subscribes.FindAsync(id);
        if (existingSubscribe == null)
        {
            return NotFound(new { message = "가입정보가 존재하지 않습니다." });
        }

        // 기존 엔티티에 값 업데이트 (필요한 필드만)
        existingSubscribe.Email = subscribe.Email;

        try
        {
            // 변경 사항 저장
            await _context.SaveChangesAsync();
            return Ok(new { message = "가입정보가 수정되었습니다." });
        }
        catch (DbUpdateConcurrencyException)
        {
            return Conflict(new { message = "동시성 오류: 다른 사용자가 이미 수정했을 수 있습니다." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"서버 오류: {ex.Message}" });
        }
    }


    /*
        최신 트렌드
        최근 트렌드와 추가 제안
        PATCH 지원 고려
        PUT은 전체 리소스를 교체하는 용도로 주로 쓰이는데, 부분 업데이트가 필요하다면 [HttpPatch]와 JsonPatchDocument를 사용하는 게 요즘 트렌드야. 예를 들어:
    */

    [HttpPatch("{id}")]
    public async Task<IActionResult> Patch(int id, [FromBody] JsonPatchDocument<Subscribe> patchDoc)
    {
        var subscribe = await _context.Subscribes.FindAsync(id);
        if (subscribe == null) return NotFound();
        patchDoc.ApplyTo(subscribe);
        await _context.SaveChangesAsync();
        return Ok(new { message = "가입정보가 부분 수정되었습니다." });
    }


    // * DELETE : 관리자용
    //? api/subscribe/1
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            // DB에서 해당 엔티티 조회
            var subscribe = await _context.Subscribes.FindAsync(id);
            if (subscribe == null)
            {
                return NotFound(new { message = "가입정보가 없습니다." });
            }

            // 엔티티 삭제
            _context.Subscribes.Remove(subscribe);
            await _context.SaveChangesAsync();
            return Ok(new { message = "가입정보가 삭제되었습니다." });
        }
        catch (DbUpdateException ex)
        {
            // 외래 키 제약 등 DB 관련 오류 처리
            return StatusCode(400, new { message = $"삭제 실패: 연관된 데이터가 존재합니다. ({ex.Message})" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"서버 오류: {ex.Message}" });
        }
    }

    /*
        // ? api/subscribe/remove-subscription
        [HttpPost("remove-subscribe")]
        public async Task<IActionResult> RemoveSubscribe(ConfirmEmailDTO confirmEmailDTO)
        {
            try
            {
                var user = await _context.Subscribes.AnyAsync(x => x.Email.Equals(confirmEmailDTO.Email));
                if (!user)
                {
                    return BadRequest(new ResponseModel(ResponseCode.Error, "사용자를 찾을 수 없습니다.", confirmEmailDTO));
                }
                confirmEmailDTO.ReplayUrl = "https://api.vivabm.com";
                var removeLink = $"https://api.vivabm.com/api/subscribe/reply-remove-subscribe?email={confirmEmailDTO.Email}";

                await _emailService.SendEmailAsync(confirmEmailDTO.Email,
                "구독 취소 확인메일",
                "<h3>이메일 확인 링크</h3><br />" +
                $"<p><a href='{removeLink}'>구독 취소하기</a> </p>");

                return Ok(new AuthResponseDTO
                {
                    IsSuccess = true,
                    Message = "구독 취소 확인을 위한 메일을 보냈습니다. 메일을 확인하세요."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseModel(ResponseCode.Error, "서버 오류발생", ex.Message));
            }
        }
     */
    // 예시: UnsubscribeTokens 테이블 구조
    // Id (PK), Email (string), Token (string, unique index), ExpiresAt (DateTime), UsedAt (DateTime?, nullable)

    [HttpPost("remove-subscribe")]
    public async Task<IActionResult> RemoveSubscribe(ConfirmEmailDTO confirmEmailDTO)
    {
        try
        {
            var userExists = await _context.Subscribes.AnyAsync(x => x.Email.Equals(confirmEmailDTO.Email));
            if (!userExists)
            {
                // 사용자가 없더라도, "메일을 보냈다"고 응답하는 것이 더 나을 수 있음 (이메일 존재 여부 노출 방지)
                // 또는 상황에 맞게 처리
                return Ok(new AuthResponseDTO { IsSuccess = true, Message = "구독 취소 확인을 위한 메일을 보냈습니다. 메일을 확인하세요." });
                // return BadRequest(new ResponseModel(ResponseCode.Error, "구독 정보를 찾을 수 없습니다.", confirmEmailDTO));
            }


            // 1. 고유 토큰 생성
            var token = Guid.NewGuid().ToString("N"); // 또는 다른 안전한 랜덤 문자열 생성 방식 사용
            var email = confirmEmailDTO.Email;
            var subscriber = await _context.Subscribes.FirstOrDefaultAsync(x => x.Email.Equals(email));
            if (subscriber == null)
                return Ok(new AuthResponseDTO { IsSuccess = true, Message = "구독 취소 확인을 위한 메일을 보냈습니다. 메일을 확인하세요." });
            var subscriberId = subscriber.Id;
            var expiresAt = DateTime.UtcNow.AddHours(24); // 24시간 후 만료


            // 2. 토큰 저장 (새 테이블 UnsubscribeTokens 사용 예시)
            var unsubscribeRequest = new UnSubscribeToken
            {
                Email = confirmEmailDTO.Email,
                Token = token,
                ExpiresAt = expiresAt,
                SubscribeId = subscriberId
            };
            _context.UnSubscribeTokens.Add(unsubscribeRequest);
            await _context.SaveChangesAsync();

            var confirmationLink = $"https://viv.vivabm.com/subscriber/cancel/?token={token}&email={email}&id={subscriberId}";

            // 4. 이메일 발송
            await _emailService.SendEmailAsync(confirmEmailDTO.Email,
            "구독 취소 확인 메일",
            "<h3>구독 취소 확인</h3><br />" +
            "<p>아래 링크를 클릭하여 구독 취소를 완료하세요. 이 링크는 24시간 동안 유효합니다.</p>" +
            $"<p><a href='{confirmationLink}' style='text-content:center; font-size:1.2em; text-decoration:none;'>구독 취소 완료하기</a></p>");

            return Ok(new AuthResponseDTO
            {
                IsSuccess = true,
                Message = "구독 취소 확인을 위한 메일을 보냈습니다. 메일을 확인하세요."
            });
        }
        catch (Exception ex)
        {
            // TODO: 로깅 추가
            return StatusCode(500, new ResponseModel(ResponseCode.Error, "서버 오류 발생", ex.Message));
        }
    }

    // --> api/subscribe
    [HttpDelete("cancel")]
    public async Task<ActionResult> ConfirmUnsubscribe([FromBody] UnSubscribeDTO dto)
    {
        var unsubscribeRequest = await _context.UnSubscribeTokens
            .FirstOrDefaultAsync(t => t.Token == dto.Token && t.ExpiresAt > DateTime.UtcNow);

        if (unsubscribeRequest == null)
            return BadRequest("유효하지 않거나 만료된 링크입니다.");
        try
        {
            var request = await _context.UnSubscribeTokens.FirstOrDefaultAsync(t => t.Token.Equals(dto.Token));
            if (request == null)
                return BadRequest("해당하는 구독자는 존재하지 않습니다.");

            var subscriber = await _context.Subscribes.FirstOrDefaultAsync(t => t.Id == request.SubscribeId);
            if (subscriber == null)
                return BadRequest($"잘못된 요청입니다. 관리자에게 구독취소 메일을 보내주세요. hello.viva.bm@gmail.com");
            _context.Subscribes.Remove(subscriber);
            _context.UnSubscribeTokens.Remove(unsubscribeRequest);
            var rs = await _context.SaveChangesAsync();
            return Ok(new { message = $"{dto.Email} 구독 취소 완료되었습니다." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ResponseModel(ResponseCode.Error,
            "서버 오류 발생",
            $"{ex.Message} 서버오류로 구독 취소가 완료 되지 않았습니다. 사이트 관리자에게 메일 (hello.viva.bm@gmail.com)로 구독 해지 요청을 보내주십시오. hello.viva.bm@gmail.com"));
        }
    }
}


/*
    추가 제안
    소프트 딜리트(Soft Delete) 고려
    요즘은 데이터를 바로 삭제하지 않고, IsDeleted 같은 플래그를 추가해서 "논리적 삭제"를 하는 경우도 많아. 데이터 복구가 필요할 때 유용하니 프로젝트 요구사항에 따라 고민해봐. 예시:
    subscribe.IsDeleted = true;
    await _context.SaveChangesAsync();
    삭제 전 확인 로직 (옵션)
    중요한 데이터라면 삭제 전에 추가 확인(예: 클라이언트에서 확인 플래그를 보내도록 요구)을 넣을 수도 있어.
*/
