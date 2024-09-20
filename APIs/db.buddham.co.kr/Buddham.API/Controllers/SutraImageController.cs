
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Buddham.API.Data;
using Buddham.API.Models;
using System.Net.Http.Headers;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace Buddham.API.Controllers
{
    [Route("api/sutraimage")]
    [ApiController]
    public class SutraImageController(BuddhaContext context, UserManager<AppUser> userManager) : ControllerBase
    {
        private readonly BuddhaContext _context = context;
        private readonly UserManager<AppUser> _userManager = userManager;

        // GET: api/SutraImage
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SutraImage>>> GetSutraImages()
        {
            return await _context.SutraImages.ToListAsync();
        }

        /// <summary>
        /// 파일 업로드
        /// 파일 업로드는 RequestSizeLimitAttribute를 사용하여 요청 크기 제한을 설정할 수 있습니다.
        /// </summary>
        /// <returns></returns>
        [HttpPost, DisableRequestSizeLimit]
        public async Task<IActionResult> Upload()
        {
            try
            {
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier); // 현재 사용자 ID를 가져옵니다.

                if (currentUserId is null)
                    return NotFound(new ResponseModel(ResponseCode.Error, "사용자를 찾을 수 없습니다", "로그인 후 다시 시도해주세요."));

                var user = await _userManager.FindByIdAsync(currentUserId);

                if (user is null)
                    return NotFound(new ResponseModel(ResponseCode.Error, "사용자를 찾을 수 없습니다", "로그인 후 다시 시도해주세요."));

                if (Request.Form.Files.Count == 0) // 파일이 없는 경우 BadRequest를 반환합니다.
                    return BadRequest("파일을 선택해주세요.");

                if (user.AccessFailedCount > 5) // 로그인 실패 횟수가 5회 이상인 경우 BadRequest를 반환합니다.
                    return BadRequest("로그인 실패 횟수가 5회 이상입니다.");
                var userId = user.Id; // 사용자 ID를 가져옵니다.

                var formCollection = await Request.ReadFormAsync(); // Request.Form을 사용하여 파일을 가져옵니다.

                var file = formCollection.Files[0]; // 파일은 여러 개일 수 있으므로 배열로 받습니다.

                var folderName = Path.Combine("Resources", "Images");

                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

                if (!Directory.Exists(pathToSave)) // 폴더가 없는 경우 생성합니다.,
                    Directory.CreateDirectory(pathToSave); // 지정된 경로의 모든 디렉터리와 서브 디렉터리를 만듭니다.

                if (file.Length > 0)
                {
                    // 파일 이름을 가져오면서 경로를 제거합니다.
                    var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName?.Trim('"');

                    var extension = Path.GetExtension(fileName); // 파일 확장자를 가져옵니다.
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp", ".gif", ".svg", ".bmp", ".ico", ".tif" }; // 허용할 확장자를 지정합니다.
                    if (string.IsNullOrEmpty(extension) || !allowedExtensions.Contains(extension.ToLower())) // 허용된 확장자가 아닌 경우 BadRequest를 반환합니다.
                        return BadRequest("이미지 파일만 업로드할 수 있습니다.");

                    var guid = Guid.NewGuid().ToString(); // 파일 이름을 유니크하게 만듭니다.
                    var uniqueName = Path.GetFileNameWithoutExtension(fileName); // 파일 이름에서 확장자를 제거합니다.
                    if (string.IsNullOrWhiteSpace(uniqueName)) // 파일 이름이 없는 경우 BadRequest를 반환합니다.
                        return BadRequest("잘못된 파일 이름입니다.");

                    // 파일 이름을 생성합니다. (사용자 ID + 파일 이름 + 확장자)
                    fileName = $"{userId}_{uniqueName.Replace(" ", "_").ToLower()}{extension}";

                    if (string.IsNullOrWhiteSpace(fileName))
                        return BadRequest("Invalid file name");

                    if (fileName.Contains('/')) // 파일 이름에 경로가 포함되어 있을 수 있으므로 제거합니다.
                        fileName = fileName[(fileName.LastIndexOf('/') + 1)..];

                    if (fileName.Contains('\\')) // 파일 이름에 경로가 포함되어 있을 수 있으므로 제거합니다.
                        fileName = fileName[(fileName.LastIndexOf('\\') + 1)..];

                    var fullPath = Path.Combine(pathToSave, fileName); // 파일 경로를 생성합니다.

                    var dbPath = Path.Combine(folderName, fileName); // DB에 저장할 경로를 생성합니다.

                    using var stream = new FileStream(fullPath, FileMode.Create);

                    await file.CopyToAsync(stream);

                    return Ok(new { dbPath }); // 파일 경로를 반환합니다.
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }


        // GET: api/SutraImage/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SutraImage>> GetSutraImage(int id)
        {
            var sutraImage = await _context.SutraImages.FindAsync(id);

            if (sutraImage == null)
            {
                return NotFound();
            }

            return sutraImage;
        }

        // PUT: api/SutraImage/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSutraImage(int id, SutraImage sutraImage)
        {
            if (id != sutraImage.Id)
            {
                return BadRequest();
            }

            _context.Entry(sutraImage).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SutraImageExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }


        // DELETE: api/SutraImage/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSutraImage(int id)
        {
            var sutraImage = await _context.SutraImages.FindAsync(id);
            if (sutraImage == null)
            {
                return NotFound();
            }

            _context.SutraImages.Remove(sutraImage);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SutraImageExists(int id)
        {
            return _context.SutraImages.Any(e => e.Id == id);
        }
    }
}
