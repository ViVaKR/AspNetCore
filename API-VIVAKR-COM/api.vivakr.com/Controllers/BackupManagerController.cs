using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO.Compression;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using ViVaKR.API.Helpers;
using ViVaKR.API.Data;
using ViVaKR.API.DTOs;
using ViVaKR.API.Models;

namespace ViVaKR.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BackupManagerController(VivaKRDbContext context, UserManager<AppUser> userManager) : ControllerBase
    {
        private readonly VivaKRDbContext _context = context;
        private readonly UserManager<AppUser> _userManager = userManager;

        //? 사용자 아이디로 데이터 조회 및 데이터리스트를 CSV 포맷으로 반환
        //--> GET api/BackupManager/downloadCSV/<id>
        [Authorize]
        [HttpGet("downloadCSV/{id}")]
        public async Task<ActionResult<CodeResDTO>> GetCodeByUserIdCSVAsync(string id)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier); // 현재 사용자 ID를 가져옵니다.

            if (currentUserId == null) return Unauthorized(); // 현재 사용자와 요청 사용자가 다른 경우 권한이 없습니다.

            var user = await _userManager.FindByIdAsync(currentUserId);

            if (user is null)
                return NotFound(new ResponseModel(ResponseCode.Error, "가입된 회원이 아닙니다.", "회원가입 후 다시 시도해주세요."));

            var userId = user.Id;

            var list = await _context.Codes.Where(c => c.UserId == id).ToListAsync();
            if (list == null || list.Count == 0) return NotFound();

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ",",
                HasHeaderRecord = true,
                IgnoreBlankLines = true,
                Encoding = Encoding.UTF8,
                NewLine = Environment.NewLine
            };

            using var mem = new MemoryStream();
            using var writer = new StreamWriter(mem);
            using var csvWriter = new CsvWriter(writer, config);
            csvWriter.WriteRecords(list);

            writer.Flush();
            var csv = Encoding.UTF8.GetString(mem.ToArray());
            var folderName = Path.Combine("Temp", "FileData", "Backup", "Code");
            var pathToSave = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), folderName);

            if (!Directory.Exists(pathToSave)) // 폴더가 없는 경우 생성합니다.,
                Directory.CreateDirectory(pathToSave); // 지정된 경로의 모든 디렉터리와 서브 디렉터리를 만듭니다.

            var fileName = $"backup_{userId}_{DateTime.Now:yyyyMMddHHmmss}.csv";
            var fullPath = Path.Combine(pathToSave, fileName);
            await System.IO.File.WriteAllTextAsync(fullPath, csv);
            var rs = await SaveCodeAsync(userId, fileName, fullPath);

            if (rs <= 0)
                return Ok(new CodeResDTO
                {
                    IsSuccess = false,
                    Message = "백업용 다운로드 대상 파일 저장에는 실패하였습니다.",
                    Data = csv
                });
            else
                return Ok(new CodeResDTO
                {
                    IsSuccess = true,
                    Message = fullPath,
                    Data = csv
                });
        }


        //? 사용자 아이디로 데이터 조회 및 데이터리스트를 JSON 포맷으로 반환
        //--> GET api/BackupManager/downloadJSON/<id>
        [Authorize]
        [HttpGet("downloadJSON/{id}")]
        public async Task<ActionResult<CodeResDTO>> GetCodeByUserIdJSONAsync(string id)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier); // 현재 사용자 ID를 가져옵니다.

            if (currentUserId == null) return Unauthorized(); // 현재 사용자와 요청 사용자가 다른 경우 권한이 없습니다.

            var user = await _userManager.FindByIdAsync(currentUserId);

            if (user is null)
                return NotFound(new ResponseModel(ResponseCode.Error, "가입된 회원이 아닙니다.", "회원가입 후 다시 시도해주세요."));

            var userId = user.Id;

            var list = await _context.Codes.Where(c => c.UserId == id).ToListAsync();
            if (list == null || list.Count == 0) return NotFound();

            var folderName = Path.Combine("Temp", "FileData", "Backup", "Code");
            var pathToSave = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), folderName);

            if (!Directory.Exists(pathToSave)) // 폴더가 없는 경우 생성합니다.,
                Directory.CreateDirectory(pathToSave); // 지정된 경로의 모든 디렉터리와 서브 디렉터리를 만듭니다.

            var fileName = $"backup_{userId}_{DateTime.Now:yyyyMMddHHmmss}.json";
            var fullPath = Path.Combine(pathToSave, fileName);
            await System.IO.File.WriteAllTextAsync(fullPath, ToJson(list));

            var rs = await SaveCodeAsync(userId, fileName, fullPath);

            if (rs <= 0)
                return Ok(new CodeResDTO
                {
                    IsSuccess = false,
                    Message = "백업용 다운로드 대상 파일 저장에는 실패하였습니다.",
                    Data = list
                });
            else
                return Ok(new CodeResDTO
                {
                    IsSuccess = true,
                    Message = fullPath,
                    Data = list
                });
        }

        public string ToJson<T>(List<T> list)
        {
            return JsonSerializer.Serialize(list);
        }

        //--> 데이터베이스에 저장
        private async Task<int> SaveCodeAsync(string userId, string fileName, string filePath)
        {
            var id = _context.BackupManagers.Any() ? _context.BackupManagers.Max(b => b.Id) + 1 : 1;
            var newData = new BackupManager
            {
                Id = id,
                UserId = userId,
                FileName = fileName,
                FilePath = filePath,
                BackupDate = DateTime.UtcNow.SetKindUtc(),
            };
            _context.BackupManagers.Add(newData);
            var rs = await _context.SaveChangesAsync();
            return newData.Id;
        }

        //--> 다운로드 URL 생성
        //--> GET api/BackupManager/DownloadCodeFile?fileUrl=<fileUrl>
        [HttpGet, DisableRequestSizeLimit]
        [Authorize]
        [Route("DownloadCodeFile")]
        public async Task<IActionResult> DownloadCodeFile([FromQuery] string fileUrl)
        {
            try
            {
                var path = Path.Combine("Temp", "FileData", "Backup", "Code");
                var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), path, fileUrl);
                if (!System.IO.File.Exists(filePath))
                    return NotFound("파일을 찾을 수 없습니다.");

                var memory = new MemoryStream();
                using (var stream = new FileStream(filePath, FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }
                memory.Position = 0;

                var provider = new FileExtensionContentTypeProvider();
                if (!provider.TryGetContentType(filePath, out string? contentType))
                {
                    contentType = "application/octet-stream";
                }

                return File(memory, contentType, Path.GetFileName(filePath));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"내부서버오류: {ex}");
            }
        }

        //  zip 파일로 압축 하여 저장
        public async Task UploadCSVAsync(string csv)
        {
            if (string.IsNullOrEmpty(csv)) return;
            var folderName = Path.Combine("Temp", "FileData", "Backup", "Code");
            var pathToSave = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), folderName);

            if (!Directory.Exists(pathToSave)) // 폴더가 없는 경우 생성합니다.,
                Directory.CreateDirectory(pathToSave); // 지정된 경로의 모든 디렉터리와 서브 디렉터리를 만듭니다.

            var fileName = $"Code_{DateTime.Now:yyyyMMddHHmmss}.csv";
            var fullPath = Path.Combine(pathToSave, fileName);
            await System.IO.File.WriteAllTextAsync(fullPath, csv);
            using var zip = ZipFile.Open(fullPath + ".zip", ZipArchiveMode.Create);
            zip.CreateEntryFromFile(fullPath, fileName);

        }

        // GET: api/BackupManager
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BackupManager>>> GetBackupManagers()
        {
            return await _context.BackupManagers.ToListAsync();
        }

        // GET: api/BackupManager/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BackupManager>> GetBackupManager(int id)
        {
            var backupManager = await _context.BackupManagers.FindAsync(id);

            if (backupManager == null)
            {
                return NotFound();
            }

            return backupManager;
        }

        // PUT: api/BackupManager/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBackupManager(int id, BackupManager backupManager)
        {
            if (id != backupManager.Id)
            {
                return BadRequest();
            }

            _context.Entry(backupManager).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BackupManagerExists(id))
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

        // POST: api/BackupManager
        [HttpPost]
        public async Task<ActionResult<BackupManager>> PostBackupManager(BackupManager backupManager)
        {
            _context.BackupManagers.Add(backupManager);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBackupManager", new { id = backupManager.Id }, backupManager);
        }

        // DELETE: api/BackupManager/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBackupManager(int id)
        {
            var backupManager = await _context.BackupManagers.FindAsync(id);
            if (backupManager == null)
            {
                return NotFound();
            }

            _context.BackupManagers.Remove(backupManager);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BackupManagerExists(int id)
        {
            return _context.BackupManagers.Any(e => e.Id == id);
        }
    }
}
