using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ViVaKR.API.Data;
using ViVaKR.API.DTOs;
using ViVaKR.API.Interfaces;
using ViVaKR.API.Models;

namespace ViVaKR.API.Controllers;

//--> api/account
[Route("api/account")]
[ApiController]
public class AccountController(
    UserManager<AppUser> userManager,
    VivaKRDbContext context,
    IEmailService emailService,
    IConfiguration configuration
) : ControllerBase
{
    private readonly IConfiguration _configuration = configuration;

    //--> 회원 전체 목록
    //--> GET
    //--> api/account/list
    [Authorize(Roles = "Admin")]
    [HttpGet("list")]
    public async Task<ActionResult<IEnumerable<UserDetailDTO>>> GetList()
    {
        var users = await userManager.Users.ToListAsync();
        var userDetailDTOs = new List<UserDetailDTO>();

        foreach (var user in users)
        {
            var avatar = await context.FileManagers.Where(x => x.UserId == user.Id).OrderByDescending(x => x.Id)
                .FirstOrDefaultAsync();
            userDetailDTOs.Add(new UserDetailDTO
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                EmailConfirmed = user.EmailConfirmed,
                Roles = [.. await userManager.GetRolesAsync(user)],
                PhoneNumber = user.PhoneNumber,
                TwoFactorEnabled = user.TwoFactorEnabled,
                PhoneNumberConformed = user.PhoneNumberConfirmed,
                AccessFailedCount = user.AccessFailedCount,
                Avata = avatar?.FileName ?? string.Empty
            });
        }

        return Ok(userDetailDTOs);
    }

    //--> GET
    //--> api/account/{id}
    [Authorize(Roles = "Admin")]
    [HttpGet("{id}")]
    public async Task<ActionResult<UserDetailDTO>> GetUserById(string id)
    {
        var user = await userManager.FindByIdAsync(id);

        if (user is null) return NotFound(new ResponseModel(ResponseCode.Error, "사용자가 없습니다.", string.Empty));

        var avatar = await context.FileManagers.Where(x => x.UserId == id).OrderByDescending(x => x.Id)
            .FirstOrDefaultAsync();

        return Ok(new UserDetailDTO
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            EmailConfirmed = user.EmailConfirmed,
            Roles = [.. await userManager.GetRolesAsync(user)],
            PhoneNumber = user.PhoneNumber,
            TwoFactorEnabled = user.TwoFactorEnabled,
            PhoneNumberConformed = user.PhoneNumberConfirmed,
            AccessFailedCount = user.AccessFailedCount,
            Avata = avatar?.FileName ?? string.Empty
        });
    }

    //--> 회원 정보 수정
    //--> PUT
    //--> api/account/{id}
    [Authorize]
    [HttpPut("{id}")]
    public async Task<ActionResult<UserInfo>> UpdateUserInfo(string id, UserInfo userInfo)
    {
        var user = await userManager.FindByIdAsync(id);
        if (user is null) return NotFound(new ResponseModel(ResponseCode.Error, "사용자가 없습니다.", string.Empty));

        var exist = await context.UserInfos.AnyAsync(x => x.Id == id);

        if (exist)
        {
            var info = await context.UserInfos.SingleAsync(x => x.Id == id);
            info.Avata = userInfo.Avata;
            return Ok(new ResponseModel(ResponseCode.OK, "사용자 정보를 성공적으로 수정했습니다.", string.Empty));
        }

        var result = await userManager.UpdateAsync(user);

        if (!result.Succeeded)
            return BadRequest(new ResponseModel(ResponseCode.Error, "회원정보 수정에 실패했습니다",
                result.Errors.Select(x => x.Description).ToArray()));

        return Ok(new ResponseModel(ResponseCode.OK, "회원정보를 성공적으로 수정했습니다", string.Empty));
    }

    // 회원가입
    //--> POST
    //--> api/account/signup
    [AllowAnonymous]
    [HttpPost("signup")]
    public async Task<IActionResult> SignUp([FromBody] SignUpDTO signUpDTO)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(new ResponseModel(ResponseCode.Error, "Invalid model", string.Empty));

            if (string.IsNullOrEmpty(signUpDTO.Email) || string.IsNullOrEmpty(signUpDTO.Password))
                return BadRequest(new ResponseModel(ResponseCode.Error, "Email or password is empty", string.Empty));

            var check = userManager.Users.Any(x => x.Email!.Equals(signUpDTO.Email));

            if (check)
                return BadRequest(new ResponseModel(ResponseCode.Error, "이미등록된 회원입니다.", signUpDTO.Email));

            var user = new AppUser
            {
                UserName = signUpDTO.Email,
                Email = signUpDTO.Email,
                FullName = signUpDTO.FullName
            };

            var result = await userManager.CreateAsync(user, signUpDTO.Password);

            if (!result.Succeeded)
                return BadRequest(new ResponseModel(ResponseCode.Error, "회원등록에 실패하였습니다.",
                    result.Errors.Select(x => x.Description).ToArray()));

            var tempUser = await userManager.FindByEmailAsync(signUpDTO.Email);

            if (tempUser is null)
                return BadRequest(new ResponseModel(ResponseCode.Error, "회원정보를 찾을 수 없습니다.",
                    string.Empty));

            try
            {
                await userManager.AddToRoleAsync(tempUser, "User");
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseModel(ResponseCode.Error, "역할 추가에 실패하였습니다.", ex.Message));
            }

            return Ok(new AuthResponseDTO
            {
                IsSuccess = true,
                Message = "회원등록에 성공하였습니다."
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new AuthResponseDTO
            {
                IsSuccess = false,
                Message = ex.Message
            });
        }
    }

    //--> 로그인
    //--> POST
    //--> api/account/signin
    [AllowAnonymous]
    [HttpPost("signin")]
    public async Task<IActionResult> SignIn([FromBody] SignInDTO signInDTO)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(new AuthResponseDTO
                {
                    IsSuccess = false,
                    Token = string.Empty,
                    Message = "Invalid model",
                    RefreshToken = string.Empty
                });

            if (string.IsNullOrEmpty(signInDTO.Email) || string.IsNullOrEmpty(signInDTO.Password))
                return BadRequest(new AuthResponseDTO
                {
                    IsSuccess = false,
                    Token = string.Empty,
                    Message = "이메일 또는 비밀번호가 잘못되었습니다.",
                    RefreshToken = string.Empty
                });

            var user = await userManager.FindByEmailAsync(signInDTO.Email);

            if (user is null)
                return BadRequest(new AuthResponseDTO
                {
                    IsSuccess = false,
                    Token = string.Empty,
                    Message = "사용자를 찾을 수 없습니다.",
                    RefreshToken = string.Empty
                });

            var result = await userManager.CheckPasswordAsync(user, signInDTO.Password);

            if (!result)
                return BadRequest(new AuthResponseDTO
                {
                    IsSuccess = false,
                    Token = string.Empty,
                    Message = "비밀번호가 일치하지 않습니다.",
                    RefreshToken = string.Empty
                });

            var token = GenerateToken(user);

            var refreshToken = GenerateRefreshToken();

            // Configuration["JWTConfig:
            var _validityIn = _configuration["JWTConfig:RefreshTokenValidityIn"] ?? "60";
            var rs = int.TryParse(_validityIn, out var refreshTokenValidity);
            // var rs = int.TryParse(AuthSettings.RefreshTokenValidityIn.ToString(), out var refreshTokenValidity);

            if (!rs)
                return BadRequest(new AuthResponseDTO
                {
                    IsSuccess = false,
                    Token = string.Empty,
                    Message = "RefreshTokenValidityIn 설정값이 잘못되었습니다.",
                    RefreshToken = string.Empty
                });

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(refreshTokenValidity);

            await userManager.UpdateAsync(user);

            return Ok(new AuthResponseDTO
            {
                IsSuccess = true,
                Token = token,
                Message = "로그인에 성공하였습니다.",
                RefreshToken = refreshToken
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new AuthResponseDTO
            {
                IsSuccess = false,
                Token = string.Empty,
                Message = $"알수 없는 이유로 로그인에 실패하였습니다\n{ex.Message}",
                RefreshToken = string.Empty
            });
        }
    }

    private static string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private string GenerateToken(AppUser user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        var _key = _configuration["JWTConfig:Key"]!;
        var key = Encoding.ASCII.GetBytes(_key);
        var roles = userManager.GetRolesAsync(user).Result;

        var _audience = _configuration["JWTConfig:Audience"]!;
        var _issuer = _configuration["JWTConfig:Issuer"]!;

        List<Claim> claims =
        [
            new(JwtRegisteredClaimNames.Email, user.Email!),
            new(JwtRegisteredClaimNames.Name, user.FullName!),
            new(JwtRegisteredClaimNames.NameId, user.Id),
            new(JwtRegisteredClaimNames.Aud, _audience),
            new(JwtRegisteredClaimNames.Iss, _issuer)
        ];

        foreach (var role in roles)
            claims.Add(new Claim(ClaimTypes.Role, role));

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(365),
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    //--> Refresh Token, 토큰 갱신
    //--> POST
    //--> api/account/refresh-token
    [AllowAnonymous]
    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken(TokenDTO tokenDTO)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(new ResponseModel(ResponseCode.Error, "Invalid model", string.Empty));

            var principal = GetPrincipalFromExpiredToken(tokenDTO.Token);

            var user = await userManager.FindByEmailAsync(tokenDTO.Email);

            // 토큰 유효성 검사
            if (principal is null
                || user is null
                || user.RefreshToken != tokenDTO.RefreshToken
                || user.RefreshTokenExpiryTime < DateTime.UtcNow)
                return BadRequest(new AuthResponseDTO
                {
                    IsSuccess = false,
                    Message = "토큰이 유효하지 않습니다."
                });

            var newJwtToken = GenerateToken(user);
            var newRefreshToken = GenerateRefreshToken();
            var _validityIn = _configuration["JWTConfig:RefreshTokenValidityIn"] ?? "60";
            var rs = int.TryParse(_validityIn, out var refreshTokenValidityIn);

            if (!rs)
                return BadRequest(new ResponseModel(ResponseCode.Error, "RefreshTokenValidityIn 설정값이 잘못되었습니다.",
                    string.Empty));
            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(refreshTokenValidityIn);

            await userManager.UpdateAsync(user);

            return Ok(new AuthResponseDTO
            {
                IsSuccess = true,
                Token = newJwtToken,
                Message = "토큰 갱신에 성공하였습니다.",
                RefreshToken = newRefreshToken
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new AuthResponseDTO
            {
                IsSuccess = false,
                Message = $"토큰 갱신 실패: {ex.Message}"
            });
        }
    }

    private ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
    {
        var _key = _configuration["JWTConfig:Key"]!;
        var tokenParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key)),
            ValidateLifetime = false
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenParameters, out var securityToken);

        if (securityToken is not JwtSecurityToken jwtSecurityToken
            || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase))
            throw new SecurityTokenException("잘못된 토큰 입니다.");

        return principal;
    }

    // 회원 상세 정보
    //--> GET api/account/detail
    [Authorize(Roles = "Admin, User, Writer")]
    [HttpGet("detail")]
    public async Task<ActionResult> GetUserDetail()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null) return NotFound(new ResponseModel(ResponseCode.Error, "사용자가 없습니다.", string.Empty));
        var user = await userManager.FindByIdAsync(userId);
        if (user is null) return NotFound(new ResponseModel(ResponseCode.Error, "사용자가 없습니다.", string.Empty));

        var avatar = await context.FileManagers.Where(x => x.UserId == userId).OrderByDescending(x => x.Id)
            .FirstOrDefaultAsync();

        return Ok(new UserDetailDTO
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            EmailConfirmed = user.EmailConfirmed,
            Roles = [.. await userManager.GetRolesAsync(user)],
            PhoneNumber = user.PhoneNumber,
            TwoFactorEnabled = user.TwoFactorEnabled,
            PhoneNumberConformed = user.PhoneNumberConfirmed,
            AccessFailedCount = user.AccessFailedCount,
            Avata = avatar?.FileName ?? string.Empty
        });
    }

    //--> POST
    //--> api/account/forget-password
    [AllowAnonymous]
    [HttpPost("forget-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgetPasswordDTO forgetPasswordDTO)
    {
        try
        {
            var user = await userManager.FindByEmailAsync(forgetPasswordDTO.Email);
            if (user is null)
                return BadRequest(new ResponseModel(ResponseCode.Error, "사용자를 찾을 수 없습니다.", forgetPasswordDTO.Email));

            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            var resetLink =
                $"https://vivakr.com/reset-password?email={user.Email}&token={WebUtility.UrlEncode(token)}";
            await emailService.SendEmailAsync(user.Email!, "비밀번호 재설정",
                $"<p>비밀번호 재설정을 위해 아래 링크를 클릭하세요.</p><a href='{resetLink}'>비밀번호 재설정</a>");

            return Ok(new ResponseModel(ResponseCode.OK, "비밀번호 재설정을 위한 이메일이 발송되었습니다.", user.Email!));
        }
        catch (Exception ex)
        {
            return BadRequest(new ResponseModel(ResponseCode.Error, $"== {ex.Message} ==", string.Empty));
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("send-mail")]
    public async Task<IActionResult> SendMailToSubscriber([FromBody] SendMailDTO? mail)
    {
        try
        {
            if (mail == null) return BadRequest(mail);
            var subscribers = context.Subscribes;
            var count = 0;
            foreach (var member in subscribers)
            {
                await emailService.SendEmailAsync(member.Email, mail.Subject!, mail.Message!);
                count++;
            }

            return Ok(new ResponseModel(ResponseCode.OK, $"== {count} ==", string.Empty));
        }
        catch (Exception ex)
        {
            return BadRequest(new ResponseModel(ResponseCode.Error, $"== {ex.Message} ==", string.Empty));
        }
    }

    [AllowAnonymous]
    //--> POST api/account/forgetpwd
    [HttpPost("forgetpwd")]
    public async Task<IActionResult> ForgetPwd(ForgetPasswordDTO forgetPasswordDTO)
    {
        try
        {
            var user = await userManager.FindByEmailAsync(forgetPasswordDTO.Email);

            if (user is null)
                return Ok(new AuthResponseDTO
                {
                    IsSuccess = false,
                    Message = "비밀번호 초기화를 위한 링크를 보냈습니다. 이메일을 확인하여 보세요. "
                });

            var token = await userManager.GeneratePasswordResetTokenAsync(user);

            if (string.IsNullOrEmpty(forgetPasswordDTO.ReplayUrl))
                forgetPasswordDTO.ReplayUrl = "https://vivakr.com";

            var passwordResetLink =
                $"{forgetPasswordDTO.ReplayUrl}/reset-password?email={Uri.EscapeDataString(user.Email!)}&token={WebUtility.UrlEncode(token)}";

            await emailService.SendEmailAsync(user.Email!, @"비밀번호 재설정",
                $"<a href='{passwordResetLink}'" +
                "style='text-decoration: none; font-size: 1.5rem; margin: 1em auto; width: 100%;" +
                "color: grey;'>" +
                $"&#9772; &nbsp; [ {user.FullName}, {user.Email} ] 비밀번호 재설정 &nbsp; &#9772;" +
                "<img src='https://vivakr.com/images/robot-man.webp'" +
                "width='80%' height='auto' alt='-' style='border-radius: 2em; border: 5px solid gray; display: flex; justify-content: center;'/>" +
                "</a>");

            return Ok(new AuthResponseDTO
            {
                IsSuccess = true,
                Message = "비밀번호 초기화를 위한 링크를 보냈습니다. 이메일을 확인하여 보세요."
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ResponseModel(ResponseCode.Error, $"서버에러: {ex.Message}", string.Empty));
        }
    }

    // 사용자 이름 변경
    //--> PUT api/account/update-username
    [Authorize]
    [HttpPut("updateuser")]
    public async Task<IActionResult> UpdateUserName([FromBody] UpdateUserNameDTO updateUserNameDTO)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(new ResponseDTO(false, "잘못된 데이터입니다.", string.Empty));

            var user = await userManager.FindByEmailAsync(updateUserNameDTO.Email);
            if (user is null)
                return NotFound(new ResponseDTO(false, "사용자를 찾을 수 없습니다", updateUserNameDTO.Email));

            user.FullName = updateUserNameDTO.NewUserName;
            var result = await userManager.UpdateAsync(user);

            if (!result.Succeeded)
                return BadRequest(new ResponseDTO(false, "사용자 이름 변경에 실패했습니다",
                    result.Errors.Select(x => x.Description).ToArray()));

            return Ok(new ResponseDTO(true, "사용자 이름을 성공적으로 변경했습니다", user.Email!));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ResponseDTO(false, $"서버에러: {ex.Message}", string.Empty));
        }
    }

    // 비밀번호 재설정 (2. change password)
    //--> POST
    //--> api/account/reset-password
    [AllowAnonymous]
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO resetPasswordDTO)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(new ResponseModel(ResponseCode.Error, "Invalid model", string.Empty));

            var user = await userManager.FindByEmailAsync(resetPasswordDTO.Email);

            if (user is null)
                return BadRequest(new ResponseModel(ResponseCode.Error, "사용자를 찾을 수 없습니다.", resetPasswordDTO.Email));

            var result =
                await userManager.ResetPasswordAsync(user, resetPasswordDTO.Token, resetPasswordDTO.NewPassword);

            if (!result.Succeeded)
                return BadRequest(new ResponseModel(ResponseCode.Error, "비밀번호 변경에 실패하였습니다.",
                    result.Errors.Select(x => x.Description).ToArray()));

            return Ok(new ResponseModel(ResponseCode.OK, "비밀번호 변경에 성공하였습니다.", user.Email!));
        }
        catch (Exception ex)
        {
            return BadRequest(new ResponseModel(ResponseCode.Error, ex.Message, string.Empty));
        }
    }

    // 이메일 확인 링크 발송 (1. send email)
    //--> POST
    //--> api/account/confirm-send-mail
    [HttpPost("confirm-send-mail")]
    public async Task<IActionResult> ConfirmSendEmail(ConfirmEmailDTO confirmEmailDTO)
    {
        try
        {
            var user = await userManager.FindByEmailAsync(confirmEmailDTO.Email);

            if (user is null)
                return BadRequest(new ResponseModel(ResponseCode.Error, "사용자를 찾을 수 없습니다.", confirmEmailDTO.Email));

            var token = await userManager.GenerateEmailConfirmationTokenAsync(user);

            var resetLink =
                $"{confirmEmailDTO.ReplayUrl}/confirm-reply-email?email={user.Email}&token={WebUtility.UrlEncode(token)}";

            await emailService.SendEmailAsync(user.Email!, "이메일 확인 메일",
                $"<h1>이메일 확인 링크 <a href='{resetLink}'> 클릭하세요.</a>.</h1>");
            return Ok(new AuthResponseDTO
            {
                IsSuccess = true,
                Message = "이메일 확인을 위한 메일링크를 보냈습니다. 메일을 확인하여 보세요."
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new ResponseModel(ResponseCode.Error, ex.Message, string.Empty));
        }
    }

    // 이메일 수신 확인 (2. confirm reply email)
    //--> POST
    //--> api/account/confirm-reply-email
    [HttpPost("confirm-reply-email")]
    public async Task<IActionResult> ConfirmReplyEmail(ConfirmReplyDTO confirmReplyDTO)
    {
        var user = await userManager.FindByEmailAsync(confirmReplyDTO.Email);

        if (user is null)
            return BadRequest(new ResponseModel(ResponseCode.Error, "잘못된 사용자 또는 잘못된 토큰입니다.", string.Empty));

        confirmReplyDTO.Token = confirmReplyDTO.Token.Replace(" ", "+");

        var result = await userManager.ConfirmEmailAsync(user, confirmReplyDTO.Token);


        if (result.Succeeded) return Ok(new AuthResponseDTO { IsSuccess = true, Message = "이메일 확인에 성공했습니다" });

        return BadRequest(new AuthResponseDTO { IsSuccess = false, Message = "이메일 확인에 실패했습니다" });
    }

    // 비밀번호 확인
    //--> POST
    //--> api/account/check-password
    [HttpPost("check-password")]
    public async Task<IActionResult> CheckPassword([FromBody] SignInDTO signInDTO)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(new ResponseModel(ResponseCode.Error, "Invalid model", string.Empty));

            var user = await userManager.FindByEmailAsync(signInDTO.Email);

            if (user is null)
                return BadRequest(new ResponseModel(ResponseCode.Error, "사용자를 찾을 수 없습니다.", signInDTO.Email));

            var result = await userManager.CheckPasswordAsync(user, signInDTO.Password);

            if (!result)
                return BadRequest(new ResponseModel(ResponseCode.Error, "비밀번호가 일치하지 않습니다.", string.Empty));

            return Ok(new ResponseModel(ResponseCode.OK, "비밀번호가 일치합니다.", user.Email!));
        }
        catch (Exception ex)
        {
            return BadRequest(new ResponseModel(ResponseCode.Error, ex.Message, string.Empty));
        }
    }

    // 비밀번호 변경
    //-->  PUT
    //--> api/account/changepassword
    [HttpPut("changepassword")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO changePasswordDTO)
    {
        try
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (changePasswordDTO.Email is null || changePasswordDTO.CurrentPassword is null ||
                changePasswordDTO.NewPassword is null)
                return BadRequest(new AuthResponseDTO
                {
                    IsSuccess = false,
                    Message = "이메일 또는 현재 / 변결할 비밀 번호에 잘못된 입력이 있습니다."
                });


            var user = await userManager.FindByEmailAsync(changePasswordDTO.Email);

            if (user is null || user.PasswordHash is null)
                return BadRequest(new AuthResponseDTO { IsSuccess = false, Message = "사용자를 찾을 수 없습니다" });

            var passwordVerificationResult =
                userManager.PasswordHasher.VerifyHashedPassword(user, user.PasswordHash,
                    changePasswordDTO.CurrentPassword);

            if (passwordVerificationResult == PasswordVerificationResult.Failed)
                return BadRequest(new AuthResponseDTO { IsSuccess = false, Message = "현재 비밀번호가 일치하지 않습니다" });

            var result = await userManager.ChangePasswordAsync(user, changePasswordDTO.CurrentPassword!,
                changePasswordDTO.NewPassword!);

            if (!result.Succeeded)
                return BadRequest(new AuthResponseDTO
                {
                    IsSuccess = false,
                    Message = string.Join(", ", result.Errors.Select(x => x.Description).ToArray())
                });

            return Ok(new AuthResponseDTO { IsSuccess = true, Message = "비밀번호를 성공적으로 변경했습니다" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new AuthResponseDTO { IsSuccess = false, Message = $"서버에러: {ex.Message}" });
        }
    }

    // 회원정보 수정
    //--> PUT api/account/update
    [Authorize(Roles = "Admin, User, Writer")]
    [HttpPut("update")]
    public async Task<IActionResult> UpdateAccount([FromBody] UpdateAccountDTO updateAccountDTO)
    {
        try
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (updateAccountDTO.Email is null)
                return BadRequest(new ResponseModel(ResponseCode.Error, "이메일을 입력하여 주세요.", string.Empty));

            var user = await userManager.FindByEmailAsync(updateAccountDTO.Email);

            if (user is null)
                return NotFound(new ResponseModel(ResponseCode.Error, "사용자를 찾을 수 없습니다", string.Empty));

            user.FullName = updateAccountDTO.FullName;
            user.PhoneNumber = updateAccountDTO.PhoneNumber;

            var result = await userManager.UpdateAsync(user);

            if (!result.Succeeded)
                return BadRequest(new ResponseModel(ResponseCode.Error, "회원정보 수정에 실패했습니다",
                    result.Errors.Select(x => x.Description).ToArray()));

            return Ok(new ResponseModel(ResponseCode.OK, "회원정보를 성공적으로 수정했습니다", string.Empty));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ResponseModel(ResponseCode.Error, $"서버에러: {ex.Message}", string.Empty));
        }
    }

    // 계정 삭제 기능 (관리자 용)
    //--> DELETE api/account/delete
    [Authorize(Roles = "Admin")]
    [HttpDelete("delete")]
    public async Task<IActionResult> DeleteAccount([FromBody] DeleteAccountDTO deleteAccountDTO)
    {
        try
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = await userManager.FindByEmailAsync(deleteAccountDTO.Email);

            if (user is null)
                return NotFound(new AuthResponseDTO
                {
                    IsSuccess = false,
                    Message = "사용자를 찾을 수 없습니다"
                });

            var result = await userManager.DeleteAsync(user);

            if (!result.Succeeded)
                return BadRequest(new AuthResponseDTO
                {
                    IsSuccess = false,
                    Message = "계정 삭제에 실패했습니다"
                });

            return Ok(new AuthResponseDTO
            {
                IsSuccess = true,
                Message = "계정을 성공적으로 삭제했습니다"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new AuthResponseDTO
            {
                IsSuccess = false,
                Message = $"서버에러: {ex.Message}"
            });
        }
    }


    // 계정 삭제 기능 (사용자용)
    //--> DELETE api/account/cancel-account
    [Authorize]
    [HttpDelete("cancel-account")]
    public async Task<IActionResult> DeleteMyAccount([FromBody] DeleteAccountDTO deleteAccountDTO)
    {
        try
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = await userManager.FindByEmailAsync(deleteAccountDTO.Email);
            if (user is null)
                return NotFound(new AuthResponseDTO
                {
                    IsSuccess = false,
                    Message = "사용자를 찾을 수 없습니다"
                });

            var rs = await userManager.CheckPasswordAsync(user, deleteAccountDTO.Password);

            if (!rs)
                return Unauthorized(new AuthResponseDTO
                {
                    IsSuccess = false,
                    Message = "비밀번호가 일치하지 않습니다"
                });

            var result = await userManager.DeleteAsync(user);

            if (!result.Succeeded)
                return BadRequest(new AuthResponseDTO
                {
                    IsSuccess = false,
                    Message = "계정 삭제에 실패했습니다"
                });

            return Ok(new AuthResponseDTO
            {
                IsSuccess = true,
                Message = "계정을 성공적으로 삭제했습니다"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new AuthResponseDTO
            {
                IsSuccess = false,
                Message = $"서버에러: {ex.Message}"
            });
        }
    }
}
