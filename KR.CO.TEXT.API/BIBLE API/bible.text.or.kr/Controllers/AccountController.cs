using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Bible.API.DTOs;
using Bible.API.Interfaces;
using Bible.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Bible.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController(
    UserManager<AppUser> userManager,
    IEmailService emailService,
    IOptions<JWTConfig> jwtConfig
) : ControllerBase
{
    private readonly UserManager<AppUser> _userManager = userManager;
    private readonly IEmailService _emailService = emailService;
    private readonly JWTConfig _jwtConfig = jwtConfig.Value;

    //--> 회원 전체 목록
    //--> GET
    //--> api/account/list
    [Authorize(Roles = "Admin")]
    [HttpGet("list")]
    public async Task<ActionResult<IEnumerable<UserDetailDTO>>> GetList()
    {
        var users = await _userManager.Users.ToListAsync();
        var userDetailDTOs = new List<UserDetailDTO>();
        foreach (var user in users)
        {
            userDetailDTOs.Add(new UserDetailDTO
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                EmailConfirmed = user.EmailConfirmed,
                Roles = [.. await _userManager.GetRolesAsync(user)],
                PhoneNumber = user.PhoneNumber,
                TwoFactorEnabled = user.TwoFactorEnabled,
                PhoneNumberConformed = user.PhoneNumberConfirmed,
                AccessFailedCount = user.AccessFailedCount
            });
        }
        return Ok(userDetailDTOs);
    }

    //--> 회원 상세 정보
    //--> GET
    //--> api/account/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<UserDetailDTO>> GetUserById(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is null) return NotFound(new ResponseDTO(false, "사용자가 없습니다.", string.Empty));

        return Ok(new UserDetailDTO
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            EmailConfirmed = user.EmailConfirmed,
            Roles = [.. await _userManager.GetRolesAsync(user)],
            PhoneNumber = user.PhoneNumber,
            TwoFactorEnabled = user.TwoFactorEnabled,
            PhoneNumberConformed = user.PhoneNumberConfirmed,
            AccessFailedCount = user.AccessFailedCount
        });
    }

    // //--> 회원정보 수정
    // //--> PUT api/account/update
    // [Authorize]
    // [HttpPut("update")]
    // public async Task<IActionResult> UpdateFullName(int id, [FromBody] )
    // {

    // }
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
                return BadRequest(new ResponseDTO(false, "잘못된 데이터 입니다.", string.Empty));

            if (string.IsNullOrEmpty(signUpDTO.Email) || string.IsNullOrEmpty(signUpDTO.Password))
                return BadRequest(new ResponseDTO(false, "잘못된 이메일 또는 비밀번호 입니다.", string.Empty));

            var check = _userManager.Users.Any(x => x.Email!.Equals(signUpDTO.Email));

            if (check)
                return BadRequest(new ResponseDTO(false, "이미등록된 회원입니다.", signUpDTO.Email));

            var user = new AppUser
            {
                UserName = signUpDTO.Email,
                Email = signUpDTO.Email,
                FullName = signUpDTO.FullName
            };

            var result = await _userManager.CreateAsync(user, signUpDTO.Password);

            if (!result.Succeeded)
            {
                return BadRequest(new ResponseDTO(false, "회원등록에 실패하였습니다.",
                result.Errors.Select(x => x.Description).ToArray()));
            }

            var tempUser = await _userManager.FindByEmailAsync(signUpDTO.Email);

            if (tempUser is null)
                return BadRequest(new ResponseDTO(false, "사용자 등록에 문제가 발생하여 사용자를 찾을 수 없습니다.", string.Empty));

            try
            {
                if (signUpDTO.Roles is null)
                    await _userManager.AddToRoleAsync(tempUser, "User");
                else
                    foreach (var role in signUpDTO.Roles)
                        await _userManager.AddToRoleAsync(tempUser, role);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseDTO(false, "역할 추가에 실패하였습니다.", ex.Message));
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

            var user = await _userManager.FindByEmailAsync(signInDTO.Email);

            if (user is null)
                return BadRequest(new AuthResponseDTO
                {
                    IsSuccess = false,
                    Token = string.Empty,
                    Message = "사용자를 찾을 수 없습니다.",
                    RefreshToken = string.Empty
                });

            var result = await _userManager.CheckPasswordAsync(user, signInDTO.Password);

            if (!result)
                return BadRequest(new AuthResponseDTO
                {
                    IsSuccess = false,
                    Token = string.Empty,
                    Message = "비밀번호가 일치하지 않습니다.",
                    RefreshToken = string.Empty
                });

            string token = GenerateToken(user);

            var refreshToken = GenerateRefreshToken();

            var rs = int.TryParse(_jwtConfig.RefreshTokenValidityIn.ToString(), out int refreshTokenValidity);

            if (!rs)
                return BadRequest(new AuthResponseDTO
                {
                    IsSuccess = false,
                    Token = string.Empty,
                    Message = "RefreshTokenValidityIn 설정값이 잘못되었습니다.",
                    RefreshToken = string.Empty
                });
            // return BadRequest(new ResponseDTO(false, "RefreshTokenValidityIn 설정값이 잘못되었습니다.", string.Empty));

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(refreshTokenValidity);

            await _userManager.UpdateAsync(user);

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


    /// <summary>
    /// 토큰 발급
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    private string GenerateToken(AppUser user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        var key = Encoding.ASCII.GetBytes(_jwtConfig.Key!);
        var roles = _userManager.GetRolesAsync(user).Result;

        List<Claim> claims =
        [
            new Claim(JwtRegisteredClaimNames.Email, user.Email!),
            new Claim(JwtRegisteredClaimNames.Name, user.FullName!),
            new Claim(JwtRegisteredClaimNames.NameId, user.Id),
            new Claim(JwtRegisteredClaimNames.Aud, _jwtConfig.Audience),
            new Claim(JwtRegisteredClaimNames.Iss, _jwtConfig.Issuer)
        ];

        foreach (var role in roles)
            claims.Add(new Claim(ClaimTypes.Role, role));

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(365),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
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
                return BadRequest(new ResponseDTO(false, "Invalid model", string.Empty));

            ClaimsPrincipal? principal = GetPrincipalFromExpiredToken(tokenDTO.Token);

            var user = await _userManager.FindByEmailAsync(tokenDTO.Email);

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

            var rs = int.TryParse(_jwtConfig.RefreshTokenValidityIn.ToString(), out int refreshTokenValidityIn);

            if (!rs)
                return BadRequest(new ResponseDTO(false, "RefreshTokenValidityIn 설정값이 잘못되었습니다.", string.Empty));
            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(refreshTokenValidityIn);

            await _userManager.UpdateAsync(user);

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
        var tokenParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.Key)),
            ValidateLifetime = false
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenParameters, out SecurityToken securityToken);

        if (securityToken is not JwtSecurityToken jwtSecurityToken
        || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
        StringComparison.InvariantCultureIgnoreCase))
            throw new SecurityTokenException("잘못된 토큰 입니다.");

        return principal;
    }

    // 회원 상세 정보
    //--> GET api/account/detail
    [Authorize]
    [HttpGet("detail")]
    public async Task<ActionResult> GetUserDetail()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null) return NotFound(new ResponseDTO(false, "사용자가 없습니다.", string.Empty));
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null) return NotFound(new ResponseDTO(false, "사용자가 없습니다.", string.Empty));

        return Ok(new UserDetailDTO
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            EmailConfirmed = user.EmailConfirmed,
            Roles = [.. await _userManager.GetRolesAsync(user)],
            PhoneNumber = user.PhoneNumber,
            TwoFactorEnabled = user.TwoFactorEnabled,
            PhoneNumberConformed = user.PhoneNumberConfirmed,
            AccessFailedCount = user.AccessFailedCount
        });
    }

    // 비밀번호 분실 복구(1. send email)
    //--> POST
    //--> api/account/forget-password
    [AllowAnonymous]
    [HttpPost("forget-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgetPasswordDTO forgetPasswordDTO)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(forgetPasswordDTO.Email);

            if (user is null)
                return BadRequest(new ResponseDTO(false, "사용자를 찾을 수 없습니다.", forgetPasswordDTO.Email));

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var resetLink = $"https://bible.text.or.kr/reset-password?email={user.Email}&token={WebUtility.UrlEncode(token)}";

            await _emailService.SendEmailAsync(user.Email!,
            "비밀번호 재설정",
            $"<h1>비밀번호 재설정</h1><p>비밀번호 재설정을 위해 아래 링크를 클릭하세요.</p><a href='{resetLink}'>비밀번호 재설정</a>");

            return Ok(new ResponseDTO(true, "비밀번호 재설정을 위한 이메일이 발송되었습니다.", user.Email!));
        }
        catch (Exception ex)
        {
            return BadRequest(new ResponseDTO(false, $"== {ex.Message} ==", string.Empty));
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

            if (updateUserNameDTO.Email is null || updateUserNameDTO.NewUserName is null)
                return BadRequest(new ResponseDTO(false, "이메일 또는 새로운 사용자 이름을 입력하여 주세요.", string.Empty));

            var user = await _userManager.FindByEmailAsync(updateUserNameDTO.Email);

            if (user is null)
                return NotFound(new ResponseDTO(false, "사용자를 찾을 수 없습니다", updateUserNameDTO.Email));

            user.FullName = updateUserNameDTO.NewUserName;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
                return BadRequest(new ResponseDTO(false, "사용자 이름 변경에 실패했습니다", result.Errors.Select(x => x.Description).ToArray()));

            return Ok(new ResponseDTO(true, "사용자 이름을 성공적으로 변경했습니다", user.Email!));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ResponseDTO(false, $"서버에러: {ex.Message}", string.Empty));
        }
    }

    [AllowAnonymous]
    //--> POST api/account/forgetpassword
    [HttpPost("forgetpwd")]
    public async Task<IActionResult> ForgetPwd(ForgetPasswordDTO forgetPasswordDTO)
    {
        try
        {
            AppUser? user = await _userManager.FindByEmailAsync(forgetPasswordDTO.Email);

            if (user is null)
            {
                return Ok(new AuthResponseDTO
                {
                    IsSuccess = false,
                    Message = "사용자를 찾을 수 없습니다"
                });
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var passwordResetLink = $"http://bible.text.or.kr/reset-password?email={user.Email}&token={WebUtility.UrlEncode(token)}";

            await _emailService.SendEmailAsync(user.Email!, "Reset Your Password", $"<h1>Reset your password by <a href='{passwordResetLink}'>clicking here</a>.</h1>");

            return Ok(new AuthResponseDTO
            {
                IsSuccess = true,
                Message = "비밀번호 초기화를 위한 링크를 보냈습니다. 이메일을 확인하여 보세요."
            });

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
                return BadRequest(new ResponseDTO(false, "Invalid model", string.Empty));

            var user = await _userManager.FindByEmailAsync(resetPasswordDTO.Email);

            if (user is null)
                return BadRequest(new ResponseDTO(false, "사용자를 찾을 수 없습니다.", resetPasswordDTO.Email));

            var result = await _userManager.ResetPasswordAsync(user, resetPasswordDTO.Token, resetPasswordDTO.NewPassword);

            if (!result.Succeeded)
                return BadRequest(new ResponseDTO(false, "비밀번호 변경에 실패하였습니다.", result.Errors.Select(x => x.Description).ToArray()));

            return Ok(new ResponseDTO(true, "비밀번호 변경에 성공하였습니다.", user.Email!));
        }
        catch (Exception ex)
        {
            return BadRequest(new ResponseDTO(false, ex.Message, string.Empty));
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
            AppUser? user = await _userManager.FindByEmailAsync(confirmEmailDTO.Email);

            if (user is null)
                return BadRequest(new ResponseDTO(false,
                "사용자를 찾을 수 없습니다.",
                confirmEmailDTO.Email));

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            var resetLink = $"https://bible.text.or.kr/confirm-reply-email?email={user.Email}&token={WebUtility.UrlEncode(token)}";
            await _emailService.SendEmailAsync(user.Email!, "Confirm Email", $"이메일 확인 링크 <a href='{resetLink}'>clicking here</a>.");

            return Ok(new AuthResponseDTO
            {
                IsSuccess = true,
                Message = "이메일 확인을 위한 메일링크를 보냈습니다. 메일을 확인하여 보세요."
            });

        }
        catch (Exception ex)
        {
            return BadRequest(new ResponseDTO(false, ex.Message, string.Empty));
        }
    }

    // 이메일 수신 확인 (2. confirm reply email)
    //--> POST
    //--> api/account/confirm-reply-email
    [HttpPost("confirm-reply-email")]
    public async Task<IActionResult> ConfirmReplyEmail(ConfirmReplyDTO confirmReplyDTO)
    {

        var user = await _userManager.FindByEmailAsync(confirmReplyDTO.Email);

        if (user is null || confirmReplyDTO.Token is null)
            return BadRequest(new ResponseDTO(false, "잘못된 사용자 또는 잘못된 토큰입니다.", string.Empty));

        confirmReplyDTO.Token = confirmReplyDTO.Token.Replace(" ", "+");

        var result = await _userManager.ConfirmEmailAsync(user, confirmReplyDTO.Token);


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
                return BadRequest(new ResponseDTO(false, "Invalid model", string.Empty));

            var user = await _userManager.FindByEmailAsync(signInDTO.Email);

            if (user is null)
                return BadRequest(new ResponseDTO(false, "사용자를 찾을 수 없습니다.", signInDTO.Email));

            var result = await _userManager.CheckPasswordAsync(user, signInDTO.Password);

            if (!result)
                return BadRequest(new ResponseDTO(false, "비밀번호가 일치하지 않습니다.", string.Empty));

            return Ok(new ResponseDTO(true, "비밀번호가 일치합니다.", user.Email!));

        }
        catch (Exception ex)
        {
            return BadRequest(new ResponseDTO(false, ex.Message, string.Empty));
        }
    }

    // 비밀번호 변경
    //-->  PUT
    //--> api/account/change-password
    [HttpPut("changepassword")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO changePasswordDTO)
    {
        try
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (changePasswordDTO.Email is null || changePasswordDTO.CurrentPassword is null || changePasswordDTO.NewPassword is null)
                return BadRequest(new AuthResponseDTO
                {
                    IsSuccess = false,
                    Message = "이메일 또는 현재 / 변결할 비밀 번호에 잘못된 입력이 있습니다."
                });


            AppUser? user = await _userManager.FindByEmailAsync(changePasswordDTO.Email);

            if (user is null || user.PasswordHash is null)
                return BadRequest(new AuthResponseDTO { IsSuccess = false, Message = "사용자를 찾을 수 없습니다" });

            PasswordVerificationResult passwordVerificationResult = _userManager.PasswordHasher.VerifyHashedPassword(user, user.PasswordHash, changePasswordDTO.CurrentPassword);

            if (passwordVerificationResult == PasswordVerificationResult.Failed)
                return BadRequest(new AuthResponseDTO { IsSuccess = false, Message = "현재 비밀번호가 일치하지 않습니다" });

            var result = await _userManager.ChangePasswordAsync(user, changePasswordDTO.CurrentPassword!, changePasswordDTO.NewPassword!);

            if (!result.Succeeded)
                return BadRequest(new AuthResponseDTO { IsSuccess = false, Message = string.Join(", ", result.Errors.Select(x => x.Description).ToArray()) });

            return Ok(new AuthResponseDTO { IsSuccess = true, Message = "비밀번호를 성공적으로 변경했습니다" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new AuthResponseDTO { IsSuccess = false, Message = $"서버에러: {ex.Message}" });
        }
    }

    // 회원정보 수정
    //--> PUT api/account/update
    [Authorize]
    [HttpPut("update")]
    public async Task<IActionResult> UpdateAccount([FromBody] UpdateAccountDTO updateAccountDTO)
    {
        try
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (updateAccountDTO.Email is null)
                return BadRequest(new ResponseDTO(false, "이메일을 입력하여 주세요.", string.Empty));

            var user = await _userManager.FindByEmailAsync(updateAccountDTO.Email);

            if (user is null)
                return NotFound(new ResponseDTO(false, "사용자를 찾을 수 없습니다", string.Empty));

            user.FullName = updateAccountDTO.FullName;
            user.PhoneNumber = updateAccountDTO.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
                return BadRequest(new ResponseDTO(false, "회원정보 수정에 실패했습니다", result.Errors.Select(x => x.Description).ToArray()));

            return Ok(new ResponseDTO(true, "회원정보를 성공적으로 수정했습니다", string.Empty));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ResponseDTO(false, $"서버에러: {ex.Message}", string.Empty));
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

            if (deleteAccountDTO.Email is null)
                return BadRequest(new AuthResponseDTO
                {
                    IsSuccess = false,
                    Message = "이메일을 입력하여 주세요"
                });

            var user = await _userManager.FindByEmailAsync(deleteAccountDTO.Email);

            if (user is null)
                return NotFound(new AuthResponseDTO
                {
                    IsSuccess = false,
                    Message = "사용자를 찾을 수 없습니다"
                });

            var result = await _userManager.DeleteAsync(user);

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
            if (deleteAccountDTO.Email is null)
                return BadRequest(new AuthResponseDTO
                {
                    IsSuccess = false,
                    Message = "이메일을 입력하여 주세요"
                });

            var user = await _userManager.FindByEmailAsync(deleteAccountDTO.Email);

            if (user is null)
                return NotFound(new AuthResponseDTO
                {
                    IsSuccess = false,
                    Message = "사용자를 찾을 수 없습니다"
                });

            var rs = await _userManager.CheckPasswordAsync(user, deleteAccountDTO.Password);

            if (!rs)
                return Unauthorized(new AuthResponseDTO
                {
                    IsSuccess = false,
                    Message = "비밀번호가 일치하지 않습니다"
                });

            var result = await _userManager.DeleteAsync(user);

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
