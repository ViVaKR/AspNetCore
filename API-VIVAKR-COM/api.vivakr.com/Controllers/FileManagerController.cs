using System.Net.Http.Headers;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using ViVaKR.API.Data;
using ViVaKR.API.DTOs;
using ViVaKR.API.Models;

namespace ViVaKR.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class FileManagerController(UserManager<AppUser> userManager, VivaKRDbContext dbContext) : ControllerBase
{
    private readonly VivaKRDbContext _dbContext = dbContext;
    private readonly UserManager<AppUser> _userManager = userManager;

    // 전체 파일 목록을 User Id 로 Group by 그룹하여 파일 리스트를 반환 하기
    //--> https://api.vivabm.com/api/FileManager/GetAvataList
    [HttpGet("GetAvataList")]
    public async Task<List<AvataDTO>> GetFilesByUserId()
    {
        return await _dbContext.FileManagers.Select(x =>
            new AvataDTO
            {
                UserId = x.UserId,
                AvataUrl = $"https://api.vivakr.com/images/{x.UserId}_{x.FileName}"
            }
        ).ToListAsync();
    }

    // 오래된 파일 삭제하기
    public async Task DeleteOldFiles()
    {
        var list = await _dbContext.FileManagers.ToListAsync();
        if (list.Count == 0) return;

        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier); // 현재 사용자 ID를 가져옵니다.

        if (currentUserId is null)
            return;

        var user = await _userManager.FindByIdAsync(currentUserId);

        if (user is null)
            return;

        var target = list.Where(x => x.UserId == user.Id).ToList();

        foreach (var item in target)
        {
            var filePath = item.FilePath;
            if (System.IO.File.Exists(filePath))
                System.IO.File.Delete(filePath);
        }

        _dbContext.FileManagers.RemoveRange(target);
        await _dbContext.SaveChangesAsync();
        await Task.CompletedTask;
    }


    /// <summary>
    ///     아바다 사진 파일  업로드
    ///     파일 업로드는 RequestSizeLimitAttribute를 사용하여 요청 크기 제한을 설정
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    [RequestSizeLimit(30 * 1024 * 1024)]
    [Authorize]
    [Route("Upload")]
    public async Task<IActionResult> Upload()
    {
        try
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier); // 현재 사용자 ID를 가져옵니다.

            if (currentUserId is null)
                return NotFound(new ResponseModel(ResponseCode.Error, "사용자를 찾을 수 없습니다", "로그인 후 다시 시도해주세요."));

            var user = await _userManager.FindByIdAsync(currentUserId);

            if (user is null)
                return NotFound(new ResponseModel(ResponseCode.Error, "가입된 회원이 아닙니다.", "회원가입 후 다시 시도해주세요."));

            if (Request.Form.Files.Count == 0) return BadRequest("파일을 선택해주세요.");
            if (user.AccessFailedCount > 3) return BadRequest("로그인 실패 횟수가 3회 이상입니다.");
            var userId = user.Id; // 사용자 ID를 가져옵니다.
            var formCollection = await Request.ReadFormAsync(); // Request.Form을 사용하여 파일을 가져옵니다.

            var file = formCollection.Files[0]; // 파일은 여러 개일 수 있으므로 배열로 받습니다.
            // 파일 크기가 30MB 이상인 경우 BadRequest를 반환합니다.
            if (file.Length > 30 * 1024 * 1024) return BadRequest("파일 크기는 30MB 이하로 업로드해주세요.");

            var folderName = Path.Combine("Temp", "FileData", "Images", "Code");
            var pathToSave = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), folderName);

            if (!Directory.Exists(pathToSave)) // 폴더가 없는 경우 생성합니다.,
                Directory.CreateDirectory(pathToSave); // 지정된 경로의 모든 디렉터리와 서브 디렉터리를 만듭니다.

            if (file.Length > 0)
            {
                // 파일 이름을 가져오면서 경로를 제거합니다.
                var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName?.Trim('"');

                var extension = Path.GetExtension(fileName); // 파일 확장자를 가져옵니다.
                var allowedExtensions = new[]
                    { ".jpg", ".jpeg", ".png", ".webp", ".gif", ".svg", ".bmp", ".ico", ".tif" };
                if (string.IsNullOrEmpty(extension) ||
                    !allowedExtensions.Contains(extension.ToLower())) // 허용된 확장자가 아닌 경우 BadRequest를 반환합니다.
                    return BadRequest("이미지 파일만 업로드할 수 있습니다.");

                var guid = Guid.NewGuid().ToString(); // 파일 이름을 유니크하게 만듭니다.
                var uniqueName = Path.GetFileNameWithoutExtension(fileName); // 파일 이름에서 확장자를 제거합니다.
                if (string.IsNullOrWhiteSpace(uniqueName)) // 파일 이름이 없는 경우 BadRequest를 반환합니다.
                    return BadRequest("잘못된 파일 이름입니다.");

                // 파일 이름을 생성합니다. (사용자 ID + 파일 이름 + 확장자)
                fileName = $"{userId}_{uniqueName.Replace(" ", "_").ToLower()}{extension}";

                if (string.IsNullOrWhiteSpace(fileName))
                    return BadRequest("잘못된 파일 이름입니다.");

                if (fileName.Contains('/')) // 파일 이름에 경로가 포함되어 있을 수 있으므로 제거합니다.
                    fileName = fileName[(fileName.LastIndexOf('/') + 1)..];

                if (fileName.Contains('\\')) // 파일 이름에 경로가 포함되어 있을 수 있으므로 제거합니다.
                    fileName = fileName[(fileName.LastIndexOf('\\') + 1)..];

                var fullPath = Path.Combine(pathToSave, fileName); // 파일 경로를 생성합니다.
                await DeleteOldFiles(); // 오래된 파일 삭제하기
                using var stream = new FileStream(fullPath, FileMode.Create);
                await file.CopyToAsync(stream); // 파일을 저장합니다.
                await SaveToDatabaseAsync(userId, file.FileName, fullPath); // 파일 정보를 DB에 저장합니다.
                return Ok(new FileDTO(fileName, fullPath)); // 파일 경로를 반환합니다.
            }

            return BadRequest();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"내부서버오류: {ex}");
        }
    }


    [HttpPost]
    [DisableRequestSizeLimit]
    [Authorize]
    [Route("UploadAttachImage")]
    public async Task<IActionResult> UploadAttachImage()
    {
        try
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier); // 현재 사용자 ID를 가져옵니다.

            if (currentUserId is null)
                return NotFound(new ResponseModel(ResponseCode.Error, "사용자를 찾을 수 없습니다", "로그인 후 다시 시도해주세요."));

            var user = await _userManager.FindByIdAsync(currentUserId);

            if (user is null)
                return NotFound(new ResponseModel(ResponseCode.Error, "가입된 회원이 아닙니다.", "회원가입 후 다시 시도해주세요."));

            if (Request.Form.Files.Count == 0) return BadRequest("파일을 선택해주세요.");
            if (user.AccessFailedCount > 5) return BadRequest("로그인 실패 횟수가 5회 이상입니다.");
            var userId = user.Id; // 사용자 ID를 가져옵니다.
            var formCollection = await Request.ReadFormAsync(); // Request.Form을 사용하여 파일을 가져옵니다.

            var file = formCollection.Files[0]; // 파일은 여러 개일 수 있으므로 배열로 받습니다.
            // 파일 크기가 30MB 이상인 경우 BadRequest를 반환합니다.
            if (file.Length > 30 * 1024 * 1024) return BadRequest("파일 크기는 30MB 이하로 업로드해주세요.");

            var folderName = Path.Combine("Temp", "FileData", "Images", "Code", "Attach");
            var pathToSave = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), folderName);

            if (!Directory.Exists(pathToSave)) // 폴더가 없는 경우 생성합니다.,
                Directory.CreateDirectory(pathToSave); // 지정된 경로의 모든 디렉터리와 서브 디렉터리를 만듭니다.

            if (file.Length > 0)
            {
                // 파일 이름을 가져오면서 경로를 제거합니다.
                var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName?.Trim('"');

                var extension = Path.GetExtension(fileName); // 파일 확장자를 가져옵니다.
                var allowedExtensions = new[]
                    { ".jpg", ".jpeg", ".png", ".webp", ".gif", ".svg", ".bmp", ".ico", ".tif" };
                if (string.IsNullOrEmpty(extension) ||
                    !allowedExtensions.Contains(extension.ToLower())) // 허용된 확장자가 아닌 경우 BadRequest를 반환합니다.
                    return BadRequest("이미지 파일만 업로드할 수 있습니다.");

                var guid = Guid.NewGuid().ToString(); // 파일 이름을 유니크하게 만듭니다.
                var uniqueName = Path.GetFileNameWithoutExtension(fileName); // 파일 이름에서 확장자를 제거합니다.
                if (string.IsNullOrWhiteSpace(uniqueName)) // 파일 이름이 없는 경우 BadRequest를 반환합니다.
                    return BadRequest("잘못된 파일 이름입니다.");

                // 파일 이름을 생성합니다. (사용자 ID + 파일 이름 + 확장자)
                fileName = $"{userId}_{uniqueName.Replace(" ", "_").ToLower()}{extension}";

                if (string.IsNullOrWhiteSpace(fileName))
                    return BadRequest("잘못된 파일 이름입니다.");

                if (fileName.Contains('/')) // 파일 이름에 경로가 포함되어 있을 수 있으므로 제거합니다.
                    fileName = fileName[(fileName.LastIndexOf('/') + 1)..];

                if (fileName.Contains('\\')) // 파일 이름에 경로가 포함되어 있을 수 있으므로 제거합니다.
                    fileName = fileName[(fileName.LastIndexOf('\\') + 1)..];

                var fullPath = Path.Combine(pathToSave, fileName); // 파일 경로를 생성합니다.

                using var stream = new FileStream(fullPath, FileMode.Create);

                await file.CopyToAsync(stream);

                return Ok(new FileDTO(fileName, fullPath)); // 파일 경로를 반환합니다.
            }

            return BadRequest();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"내부서버오류: {ex}");
        }
    }

    /// <summary>
    ///     업로드 파일 정보를 DB에 저장합니다.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="file"></param>
    /// <param name="fileName"></param>
    /// <param name="fullPath"></param>
    /// <returns></returns>
    private async Task SaveToDatabaseAsync(string userId, string fileName, string fullPath)
    {
        try
        {
            // FileManager Table 에 파일 정보 저장
            var id = _dbContext.FileManagers.Any() ? _dbContext.FileManagers.Max(x => x.Id) + 1 : 1;
            var fileManager = new FileManager
            {
                Id = id,
                UserId = userId,
                FileName = fileName,
                FilePath = fullPath
            };
            _dbContext.FileManagers.Add(fileManager);
            await _dbContext.SaveChangesAsync();

            await Task.CompletedTask;
        }
        catch
        {
        }
    }

    [HttpPost]
    [DisableRequestSizeLimit]
    [Authorize]
    [Route("UploadFile")]
    public async Task<IActionResult> UploadFile()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null)
            return NotFound(new ResponseModel(ResponseCode.Error, "사용자를 찾을 수 없습니다", "로그인 후 다시 시도해주세요."));

        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
            return NotFound(new ResponseModel(ResponseCode.Error, "가입된 회원이 아닙니다.", "회원 가입 후 다시 시도해주세요."));

        if (Request.Form.Files.Count == 0) return BadRequest("파일을 선택해주세요.");

        var id = user.Id;
        var formCollection = await Request.ReadFormAsync();
        var file = formCollection.Files[0];
        if (file.Length > 50 * 1024 * 1024) return BadRequest("파일 크기는 30MB 이하로 업로드해주세요.");

        var folderName = Path.Combine("Temp", "FileData", "Files", "Code");

        var pathToSave = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), folderName);

        if (!Directory.Exists(pathToSave)) Directory.CreateDirectory(pathToSave);

        if (file.Length <= 0) return BadRequest("파일이 없습니다.");
        var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition)
            .FileName?.Trim('"');
        var extension = Path.GetExtension(fileName);
        var allowedExtensions = new[]
        {
            ".html", ".css", ".sln", ".csproj", ".mdf", ".ldf", ".scss", ".js", ".dart", ".lock", ".yaml", ".mjx",
            ".rs", ".c", ".cpp", ".gitignore", ".toml", ".jpg", ".wma", ".wav", ".cs", ".hwp", ".avi", ".mkv", ".flv",
            ".md", ".psd", ".rar", ".egg", ".mp3", ".mp4", ".pdf", ".xls", ".doc", ".ppt", ".csv", ".txt", ".json",
            ".java", ".tar", ".7z", ".jpeg", ".png", ".webp", ".gif", ".svg", ".bmp", ".ico", ".tif", ".zip", ".xlsx",
            ".xlsm", ".docx", ".pptx"
        };

        if (string.IsNullOrEmpty(extension) || !allowedExtensions.Contains(extension.ToLower()))
            return BadRequest("허용된 파일만 업로드할 수 있습니다.");

        // var guid = Guid.NewGuid().ToString();
        var uniqueName = Path.GetFileNameWithoutExtension(fileName);
        if (string.IsNullOrWhiteSpace(uniqueName))
            return BadRequest("잘못된 파일 이름입니다.");

        fileName = $"{userId}_{uniqueName.Replace(" ", "_").ToLower()}{extension}";
        if (fileName.Contains('/')) // 파일 이름에 경로가 포함되어 있을 수 있으므로 제거합니다.
            fileName = fileName[(fileName.LastIndexOf('/') + 1)..];

        if (fileName.Contains('\\')) // 파일 이름에 경로가 포함되어 있을 수 있으므로 제거합니다.
            fileName = fileName[(fileName.LastIndexOf('\\') + 1)..];

        var fullPath = Path.Combine(pathToSave, fileName); // 파일 경로를 생성합니다.
        using var stream = new FileStream(fullPath, FileMode.Create);

        await file.CopyToAsync(stream);
        // await SaveToDatabaseAsync(userId, file.FileName, fullPath); // 파일 정보를 DB에 저장합니다.

        return Ok(new FileDTO(fileName, fullPath, file.Length)); // 파일 경로를 반환합니다.
    }

    [HttpGet]
    [DisableRequestSizeLimit]
    [Authorize]
    [Route("Download")]
    public async Task<IActionResult> Download([FromQuery] string fileUrl)
    {
        try
        {
            var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Temp",
                "FileData", "Images", "Code", fileUrl);
            if (!System.IO.File.Exists(filePath))
                return NotFound("파일을 찾을 수 없습니다.");

            var memory = new MemoryStream();
            using (var stream = new FileStream(filePath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }

            memory.Position = 0;

            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(filePath, out var contentType)) contentType = "application/octet-stream";

            return File(memory, contentType, Path.GetFileName(filePath));
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"내부서버오류: {ex}");
        }
    }


    [HttpGet]
    [DisableRequestSizeLimit]
    [Authorize]
    [Route("DownloadCodeFile")]
    public async Task<IActionResult> DownloadCodeFile([FromQuery] string fileUrl)
    {
        try
        {
            var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Temp",
                "FileData", "Files", "Code", fileUrl);

            if (!System.IO.File.Exists(filePath))
                return NotFound("파일을 찾을 수 없습니다.");

            var memory = new MemoryStream();
            using (var stream = new FileStream(filePath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }

            memory.Position = 0;

            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(filePath, out var contentType)) contentType = "application/octet-stream";

            return File(memory, contentType, Path.GetFileName(filePath));
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"내부서버오류: {ex}");
        }
    }

    private static string GetContentType(string filePath)
    {
        var provider = new FileExtensionContentTypeProvider();

        if (!provider.TryGetContentType(filePath, out var contentType)) contentType = "application/octet-stream";
        return contentType;
    }

    // 사용자 아이디 로 이미지 파일 가져오기
    [HttpGet]
    [DisableRequestSizeLimit]
    [Route("GetUserImageByUserId")]
    public async Task<IActionResult> GetUserImageByUserId([FromQuery] string userId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(userId))
                return NotFound("사용자를 찾을 수 없습니다.");

            if (!_dbContext.FileManagers.Any(x => x.UserId == userId))
                return Ok(new FileDTO("-", "-")); // 파일 경로를 반환합니다.
            // File Id 로 가장 최신 파일 정보 가져오기
            var fileManager = await _dbContext.FileManagers.OrderByDescending(x => x.Id)
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (fileManager is null) return Ok(new FileDTO("-", "-")); // 파일 경로를 반환합니다.

            return Ok(new FileDTO(fileManager.FileName, fileManager.FilePath)); // 파일 경로를 반환합니다.
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"내부서버오류: {ex}");
        }
    }

    /// <summary>
    ///     사용자 이미지 가져오기
    /// </summary>
    /// <returns></returns>
    [Authorize]
    [HttpGet("GetUserImage")]
    public async Task<IActionResult> GetUserImage()
    {
        try
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier); // 현재 사용자 ID를 가져옵니다.

            if (currentUserId is null)
                return NotFound(new ResponseDTO(false, "사용자를 찾을 수 없습니다.", "로그인 후 다시 시도해주세요."));

            var user = await _userManager.FindByIdAsync(currentUserId);

            if (user is null)
                return NotFound(new ResponseDTO(false, "회원가입 정보가 없습니다.", "회원 가입 후 다시 시도해주세요."));

            var userId = user.Id; // 사용자 ID를 가져옵니다.

            if (!_dbContext.FileManagers.Any(x => x.UserId == userId))
                return Ok(new FileDTO("-", "-")); // 파일 경로를 반환합니다.
            // File Id 로 가장 최신 파일 정보 가져오기
            var fileManager = _dbContext.FileManagers.OrderByDescending(x => x.Id)
                .FirstOrDefault(x => x.UserId == userId);

            if (fileManager is null) return Ok(new FileDTO("-", "-")); // 파일 경로를 반환합니다.

            return Ok(new FileDTO(fileManager.FileName, fileManager.FilePath)); // 파일 경로를 반환합니다.
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"내부서버오류: {ex}");
        }
    }

    [HttpGet]
    [DisableRequestSizeLimit]
    [Route("GetFileList")]
    public async Task<ActionResult<IEnumerable<string>>> GetFiles()
    {
        try
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (currentUserId is null)
                return NotFound(new ResponseModel(ResponseCode.Error, "사용자를 찾을 수 없습니다", "로그인 후 다시 시도해주세요."));
            var user = await _userManager.FindByIdAsync(currentUserId);
            if (user is null)
                return NotFound(new ResponseModel(ResponseCode.Error, "사용자를 찾을 수 없습니다", "로그인 후 다시 시도해주세요."));
            var userId = user.Id; // 사용자 ID를 가져옵니다.
            var folderName = Path.Combine("Temp", "FileData", "Images", "Code");
            var pathToRead = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), folderName);

            var photos = Directory.EnumerateFiles(pathToRead).Where(IsAPhotoFile)
                .Select(x => Path.GetFileName(x));


            // var files = _dbContext.FileManagers.Where(x => x.UserId == userId).ToList();

            return Ok(photos);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"내부 서버 오류 발생: {ex.Message}");
        }
    }

    private static bool IsAPhotoFile(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return extension switch
        {
            ".jpg" => true,
            ".jpeg" => true,
            ".png" => true,
            ".webp" => true,
            ".gif" => true,
            ".svg" => true,
            ".bmp" => true,
            ".ico" => true,
            ".tif" => true,
            _ => false
        };
    }
}
