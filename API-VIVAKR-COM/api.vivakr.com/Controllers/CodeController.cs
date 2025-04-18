using System.Globalization;
using System.Security.Claims;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ViVaKR.API.Data;
using ViVaKR.API.DTOs;
using ViVaKR.API.Helpers;
using ViVaKR.API.Models;

namespace ViVaKR.API.Controllers;

[Route("api/code")]
[ApiController]
public class CodeController(VivaKRDbContext context) : ControllerBase
{
    private const int MaxIncrementalLimit = 5000; // 한 번에 가져올 수 있는 최대 개수 제한

    //? 전체 데이터
    //--> GET: api/code
    [HttpGet("all")]
    public async Task<IEnumerable<Code>> GetListAsync()
    {
        var list = await context.Codes.ToListAsync();
        return list;
    }

    //? 요청한 최신 데이터 레코드 수 반환
    //--> GET: api/code/take?limit=10
    [HttpGet("take")]
    public async Task<ActionResult<IEnumerable<Code>?>> GetListLatest([FromQuery] int limit = 10)
    {
        if (limit <= 0 || limit > MaxIncrementalLimit)
        {
            limit = Math.Clamp(limit, 1, MaxIncrementalLimit);
            return BadRequest($"Limit은 1과 {MaxIncrementalLimit} 사이여야 합니다.");
        }
        try
        {
            IQueryable<Code> query = context.Codes.OrderByDescending(c => c.Id);
            var data = await query.Take(limit).ToListAsync();
            return Ok(data);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    //? 점진적 전체 데이터 로딩
    //--> GET: api/code/all-incremental?offset=0&limit=1000&categoryId=1
    // [Authorize(Roles = "Admin")] // 필요 시 권한 추가
    [HttpGet("all-incremental")]
    public async Task<ActionResult<IncrementalResult<Code>>> GetAllCodesIncrementallyAsync([FromQuery] int offset = 0,
        [FromQuery] int limit = 1000, [FromQuery] int categoryId = 1)
    {
        if (offset < 0)
            return BadRequest("Offset은 0 이상이어야 합니다.");
        if (limit <= 0 || limit > MaxIncrementalLimit)
        {
            limit = Math.Clamp(limit, 1, MaxIncrementalLimit);
            return BadRequest($"Limit은 1과 {MaxIncrementalLimit} 사이여야 합니다.");
        }

        try
        {
            // 1. 전체 아이템 수 계산 (한 번만 계산되도록 캐싱 고려 가능)
            var totalCount = await context.Codes.Where(x => x.CategoryId == categoryId).CountAsync();

            // 2. IQueryable 생성 및 정렬 (순서 보장 중요!)
            IQueryable<Code> query = context.Codes.OrderBy(c => c.Id); // ID 오름차순 정렬 (필수!)

            // 3. Skip (offset) 및 Take (limit) 적용
            var items = await query
                .Where(x => x.CategoryId == categoryId)
                .Skip(offset)
                .Take(limit)
                .ToListAsync();

            // 4. 결과 반환
            var result = new IncrementalResult<Code>
            {
                Items = items,
                TotalCount = totalCount
            };
            return Ok(result);
        }
        catch (Exception ex)
        {
            // 로깅 추가 권장
            // _logger.LogError(ex, "Failed to get codes incrementally.");
            return StatusCode(500, $"데이터를 가져오는 중 서버 오류가 발생했습니다.{ex.Message}");
        }
    }

    //? 사용자 아이디로 데이터 조회
    //--> GET api/code/user/<userId>
    // 예: GET /api/code/user/some-user-id?sortAscending=true
    [Authorize]
    [HttpGet("user/{userId}")] // 경로 수정: 사용자 ID 명시
    public async Task<ActionResult<IEnumerable<Code>>?> GetUserCodesSortedById(string userId,
        [FromQuery] bool sortAscending = false) // 쿼리 파라미터로 정렬 순서 받기 (기본값: 내림차순)
    {
        if (string.IsNullOrEmpty(userId))
            // UserId가 없는 경우는 경로 매칭이 안되거나, 추가 검증이 필요할 수 있음
            return BadRequest("User ID가 필요합니다."); // 또는 NotFound

        // 1. IQueryable 생성 및 UserId로 필터링
        IQueryable<Code> query = context.Codes.Where(x => x.UserId == userId);

        // 2. IQueryable에 정렬 조건 추가 (DB에서 정렬하도록!)
        query = sortAscending ? query.OrderBy(x => x.Id) : query.OrderByDescending(x => x.Id);

        // 3. 모든 조건이 적용된 후, 최종적으로 DB에서 데이터 가져오기
        var userCodes = await query.ToListAsync();

        // 사용자가 존재하지 않거나 코드가 없는 경우 빈 목록 반환 (NotFound를 반환할 수도 있음)
        if (userCodes.Count == 0)
        {
            // 필요 시 사용자가 실제 존재하는지 추가 확인 가능
            var userExists = await context.Users.AnyAsync(u => u.Id == userId); // Users 테이블 가정
            if (!userExists) return NotFound($"User with ID '{userId}' not found.");
        }

        return Ok(userCodes); // ActionResult를 사용하면 Ok()로 감싸서 반환하는 것이 좋음
    }


    //? 전체 데이터 -> 서버사이드 페이징/정렬/필터링 지원하도록 수정
    //--> GET: api/code/userdata (쿼리 파라미터 사용)
    // * 예: GET api/code/userdata?Page=2&PageSize=10&SortField=modified&SortOrder=desc&CategoryId=5
    [HttpGet("userdata")] // 기존 [HttpGet("list")] 에서 변경 또는 유지 가능
    public async Task<ActionResult<PagedResult<Code>>> GetCodesPagedAsync(
        [FromQuery] CodeQueryParameters queryParameters)
    {
        // 1. 기본 쿼리 생성 (IQueryable 사용!)
        IQueryable<Code> query = context.Codes.AsQueryable();

        // 2. 필터링 적용 (필요한 경우)
        if (queryParameters.CategoryId.HasValue)
            query = query.Where(c => c.CategoryId == queryParameters.CategoryId.Value);

        if (!string.IsNullOrWhiteSpace(queryParameters.SearchTerm))
            // 간단한 예: 제목 또는 부제목에서 검색 (대소문자 무시)
            query = query.Where(c => c.Title.Contains(queryParameters.SearchTerm)
                                     || c.SubTitle.Contains(queryParameters.SearchTerm));

        // TODO: 필요한 경우 다른 필터 추가 (예: UserId)

        // 3. 총 아이템 수 계산 (페이징 *전* 필터링된 결과 기준)
        var totalCount = await query.CountAsync();

        // 4. 정렬 적용
        // 간단한 동적 정렬 예시 (더 많은 필드 지원 시 Helper 함수나 라이브러리 사용 고려)
        if (!string.IsNullOrWhiteSpace(queryParameters.SortField))
        {
            // 기본적인 필드만 예시로 처리
            // 중요: 실제 Code 모델의 프로퍼티 이름과 정확히 일치해야 함
            // 더 안전한 방법은 허용된 필드 목록을 정의하는 것
            var isDescending = queryParameters.SortOrder?.ToLower() == "desc";

            query = queryParameters.SortField.ToLower() switch
            {
                "id" => isDescending ? query.OrderByDescending(c => c.Id) : query.OrderBy(c => c.Id),
                "title" => isDescending ? query.OrderByDescending(c => c.Title) : query.OrderBy(c => c.Title),
                "modified" => isDescending ? query.OrderByDescending(c => c.Modified) : query.OrderBy(c => c.Modified),
                // 카테고리 ID로 정렬
                "categoryid" => isDescending
                    ? query.OrderByDescending(c => c.CategoryId)
                    : query.OrderBy(c => c.CategoryId),
                // 작성자 이름으로 정렬
                "username" => isDescending ? query.OrderByDescending(c => c.UserName) : query.OrderBy(c => c.UserName),
                // 필요한 다른 정렬 필드 추가...
                _ => query.OrderByDescending(c => c.Id) // 기본 정렬 (예: ID 내림차순) - 필드가 유효하지 않을 때
            };
        }
        else
        {
            // 기본 정렬: SortField가 제공되지 않으면 ID 내림차순(최신순)
            query = query.OrderByDescending(c => c.Id);
        }


        // 5. 페이징 적용
        var items = await query
            .Skip((queryParameters.Page - 1) * queryParameters.PageSize)
            .Take(queryParameters.PageSize)
            .ToListAsync();

        // 6. 결과 반환
        var result = new PagedResult<Code>
        {
            Items = items,
            TotalCount = totalCount
        };

        return Ok(result);
    }

    //? 단일 데이터
    //--> GET api/code/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Code>> GetCodeAsync(int id)
    {
        var code = await context.Codes.FindAsync(id);
        if (code == null)
            return NotFound(new CodeResDTO
            {
                IsSuccess = false,
                Message = "요청한 데이터가 없음",
                Data = null
            });
        return Ok(code);
    }

    //? 추가
    //--> POST api/code
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Code code)
    {
        // 현재 사용자 확인
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized(new CodeResDTO { IsSuccess = false, Message = "인증되지 않음" });
        var check = await context.Codes.AnyAsync();

        try
        {
            var id = check ? await context.Codes.MaxAsync(c => c.Id) + 1 : 1;
            var data = new Code
            {
                Id = id,
                Title = code.Title,
                SubTitle = code.SubTitle,
                Content = code.Content,
                SubContent = code.SubContent,
                MarkDown = code.MarkDown,
                Note = code.Note,
                Created = DateOnly.FromDateTime(DateTime.Now),
                Modified = DateTime.UtcNow.SetKindUtc(),
                CategoryId = code.CategoryId,
                UserId = code.UserId,
                UserName = code.UserName,
                MyIp = code.MyIp,
                AttachFileName = code.AttachFileName,
                AttachImageName = code.AttachImageName
            };

            await context.Codes.AddAsync(data);

            var result = await context.SaveChangesAsync();

            if (result == 0)
                return BadRequest(new CodeResDTO
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

    //? 수정
    //--> PUT, UPDATE, PATCH
    //* api/code/5
    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, [FromBody] Code code)
    {
        var target = await context.Codes.FindAsync(id);
        if (target == null || id != code.Id)
            return NotFound(new CodeResDTO
            {
                IsSuccess = false,
                Message = "잘못된 데이터 요청 또는 코드 없음",
                Data = null
            });
        code.Modified = DateTime.UtcNow.SetKindUtc();
        context.Entry(target).CurrentValues.SetValues(code);
        var result = await context.SaveChangesAsync();
        if (result > 0)
            return Ok(new CodeResDTO
            {
                IsSuccess = true,
                Message = "코드 업데이트 성공",
                Data = id
            });

        return BadRequest(new CodeResDTO
        {
            IsSuccess = false,
            Message = "코드 업데이트 실패",
            Data = null
        });
    }

    //? 삭제
    //--> DELETE api/code/5
    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var target = await context.Codes.FindAsync(id);
        if (target == null)
            return NotFound(new CodeResDTO
            {
                IsSuccess = false,
                Message = "삭제할 데이터 없음",
                Data = null
            });

        // 현재 사용자 정보 가져오기
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var isAdmin = User.IsInRole("Admin");

        // 작성자 또는 Admin만 삭제 가능
        if (target.UserId != userId && !isAdmin)
            return Unauthorized(new CodeResDTO
            {
                IsSuccess = false,
                Message = "삭제 권한이 없습니다.",
                Data = null
            });

        context.Codes.Remove(target);
        var result = await context.SaveChangesAsync();
        if (result == 0)
            return BadRequest(new CodeResDTO
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


    //? 사용자 아이디로 데이터 조회 및 JSON 포맷으로 반환
    //--> GET api/code/downloadJson/5
    [Authorize]
    [HttpGet("downloadJson/{id}")]
    public async Task<ActionResult<IEnumerable<CodeResDTO>>> GetCodeByUserIdAsync(string id)
    {
        var list = await context.Codes.Where(c => c.UserId == id).ToListAsync();
        if (list.Count == 0) return NotFound();
        return Ok(new CodeResDTO
        {
            IsSuccess = true,
            Message = list.Count.ToString(),
            Data = list
        });
    }

    //? 사용자 아이디로 데이터 조회 및 데이터리스트를 CSV 포맷으로 반환
    //--> GET api/code/myCodesCSV/5
    [Authorize]
    [HttpGet("downloadCSV/{id}")]
    public async Task<ActionResult<CodeResDTO>> GetCodeByUserIdCsvAsync(string id)
    {
        var list = await context.Codes.Where(c => c.UserId == id).ToListAsync();
        if (list.Count == 0) return NotFound();

        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = ",",
            HasHeaderRecord = true,
            IgnoreBlankLines = true,
            Encoding = Encoding.UTF8,
            NewLine = Environment.NewLine
        };

        using var mem = new MemoryStream();
        await using var writer = new StreamWriter(mem);
        await using var csvWriter = new CsvWriter(writer, config);
        await csvWriter.WriteRecordsAsync(list);

        await writer.FlushAsync();
        var result = Encoding.UTF8.GetString(mem.ToArray());
        return Ok(new CodeResDTO
        {
            IsSuccess = true,
            Message = list.Count.ToString(),
            Data = result
        });
    }
}

/*
현재 코드의 문제점:

[FromBody]를 [HttpGet]과 함께 사용:
HTTP GET 요청은 원래 서버로부터 **데이터를 조회(Retrieve)**하는 데 사용돼. 요청에 필요한 정보(예: 어떤 사용자의 코드를 원하는지)는 일반적으로 URL 경로 파라미터(예: /api/code/user/123)나 쿼리 문자열(예: /api/code/usercode?userId=123&sortAsc=true)을 통해 전달하는 것이 표준이고 권장되는 방식이야.
[FromBody]는 요청 본문(Request Body)에 담긴 데이터를 받겠다는 의미인데, GET 요청에서 본문을 사용하는 것은 매우 이례적이고, 일부 프록시나 캐싱 시스템, 심지어 일부 HTTP 클라이언트에서는 문제를 일으킬 수 있어. RESTful API 설계 원칙에도 잘 맞지 않아.
수정 방향: UserId와 SortOrder는 URL을 통해 받아야 해.
불필요한 ToListAsync() 호출 (성능 저하의 주범!):
var filter = await codes.Where(x => x.UserId == userCode.UserId).ToListAsync(); 이 부분이 가장 큰 문제야. 여기서 .ToListAsync()를 호출하는 순간, 데이터베이스에 쿼리가 실행되어 해당 사용자의 모든 코드가 메모리로 로드돼.
그런 다음 filter.OrderBy(...) 또는 filter.OrderByDescending(...)를 사용하는데, 이건 메모리에 로드된 데이터를 가지고 C# 코드 레벨에서 정렬하는 거야. 데이터베이스가 정렬을 훨씬 더 효율적으로 처리할 수 있는데도 불구하고!
만약 사용자가 수천, 수만 개의 코드를 가지고 있다면, 엄청난 양의 데이터를 메모리에 올리고 C#에서 정렬하는 것은 매우 비효율적이야.
수정 방향: IQueryable의 장점을 살려야 해! Where 조건과 OrderBy/OrderByDescending 조건을 모두 IQueryable에 적용한 후에, 마지막에 딱 한 번 ToListAsync()를 호출해서 데이터베이스가 필터링과 정렬을 모두 최적화하여 수행하도록 해야 해.

*/
