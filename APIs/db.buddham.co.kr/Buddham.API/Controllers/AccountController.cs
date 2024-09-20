using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Buddham.API.Models;
using Buddham.SharedLib.Contracts;
using Buddham.SharedLib.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Buddham.API.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController(
        UserManager<AppUser> userManager,
        IEmailService emailService,
        IOptions<JWTConfig> jWTConfig
        ) : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager = userManager;
        private readonly JWTConfig _jWTConfig = jWTConfig.Value;
        private readonly IEmailService _emailService = emailService;

        // 회원 전체 목록
        //--> GET api/account/users
        [Authorize(Roles = "Admin")]
        [HttpGet("users")]
        public async Task<ActionResult<IEnumerable<UserDetailDTO>>> GetUsers()
        {
            var users = await _userManager.Users.ToListAsync();
            var userDetailDTOs = new List<UserDetailDTO>();
            foreach (var user in users)
            {
                userDetailDTOs.Add(new UserDetailDTO()
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

        //--> SignUp 회원가입
        //--> POST api/account/signup
        [AllowAnonymous]
        [HttpPost("signup")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerDTO)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);

                if (string.IsNullOrEmpty(registerDTO.Email)) return BadRequest(new ResponseModel(ResponseCode.Error, "(서버 메시지) 이메일을 입력하여 주세요", string.Empty));

                if (_userManager.Users.Any(x => x.Email!.Equals(registerDTO.Email)))
                {
                    return BadRequest(new ResponseModel(ResponseCode.Error, "이미등록된 회원입니다.", registerDTO.Email));
                }

                var user = new AppUser
                {
                    UserName = registerDTO.Email,
                    Email = registerDTO.Email,
                    FullName = registerDTO.FullName
                };

                IdentityResult result = await _userManager.CreateAsync(user, registerDTO.Password);

                if (!result.Succeeded)
                    return BadRequest(
                        new ResponseModel(ResponseCode.Error,
                        "(서버메시지) 회원등록에 실패했습니다",
                        result.Errors.Select(x => x.Description).ToArray()));

                var tempUser = await _userManager.FindByEmailAsync(registerDTO.Email);

                if (tempUser is null)
                    return BadRequest(new ResponseModel(ResponseCode.Error, "사용자 등록에 문제가 발생하여 사용자를 찾을 수 없습니다", string.Empty));

                try
                {
                    if (registerDTO.Roles is null) // 만일 사용자가 역할을 선택하지 않았다면
                        await _userManager.AddToRoleAsync(tempUser, "User"); // 사용자 역할을 부여한다.
                    else
                        foreach (var role in registerDTO.Roles)
                            await _userManager.AddToRoleAsync(user, role);
                }
                catch (Exception ex)
                {
                    return BadRequest(new ResponseModel(ResponseCode.Error, $"역할 부여에 실패했습니다: {ex.Message}", string.Empty));
                }

                return Ok(new AuthResponseDTO()
                {
                    IsSuccess = true,
                    Message = "회원등록에 성공했습니다"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new AuthResponseDTO()
                {
                    IsSuccess = false,
                    Message = $"회원등록에 실패했습니다: {ex.Message}"
                });
            }

        }

        //--> Login 로그인
        //--> POST api/account/signin
        [AllowAnonymous]
        [HttpPost("signin")]
        public async Task<IActionResult> SignIn([FromBody] SignInDTO signInDTO)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var user = await _userManager.FindByEmailAsync(signInDTO.Email);
                if (user is null)
                {
                    return BadRequest(new AuthResponseDTO
                    {
                        IsSuccess = false,
                        Message = "사용자를 찾을 수 없습니다"
                    });
                }

                var result = await _userManager.CheckPasswordAsync(user, signInDTO.Password);

                if (!result)
                {
                    return Unauthorized(new AuthResponseDTO
                    {
                        IsSuccess = false,
                        Message = "비밀번호가 일치하지 않습니다"
                    });
                }

                string token = GenerateToken(user);
                var refreshToken = GenerateRefreshToken(); // Refresh Token 생성
                _ = int.TryParse(_jWTConfig.RefreshTokenValidityIn.ToString(), out int refreshTokenValidityIn); // Refresh Token 유효기간 설정

                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(refreshTokenValidityIn);

                await _userManager.UpdateAsync(user); // Refresh Token 저장

                // await _emailService.SendEmailAsync(signInDTO.Email, "로그인 제목", "로그인 환영 부제목");

                return Ok(new AuthResponseDTO()
                {
                    IsSuccess = true,
                    Message = "로그인에 성공했습니다",
                    Token = token,
                    RefreshToken = refreshToken
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new AuthResponseDTO
                {
                    IsSuccess = false,
                    Message = $"로그인에 실패했습니다: {ex.Message}"
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
        /// 토큰 갱신, Refresh Token
        /// --> POST api/account/refresh-token
        /// </summary>
        /// <param name="tokenDTO">  </param>
        /// <returns></returns> <summary>
        [AllowAnonymous]
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(TokenDTO tokenDTO)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(new AuthResponseDTO { IsSuccess = false, Message = "잘못된 요청입니다. [!ModelState.IsValid]" });

                ClaimsPrincipal? principal = GetPrincipalFromExpiredToken(tokenDTO.Token); // 토큰 검증

                var user = await _userManager.FindByEmailAsync(tokenDTO.Email); // 사용자 찾기

                // Refresh Token 검증
                if (principal is null
                || user is null
                || user.RefreshToken != tokenDTO.RefreshToken
                || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                    return BadRequest(new AuthResponseDTO
                    {
                        IsSuccess = false,
                        Message = "잘못된 요청입니다. [Refresh Token 검증 실패]"
                    });

                var newJwtToken = GenerateToken(user); // JWT Token 생성
                var newRefreshToken = GenerateRefreshToken(); // Refresh Token 생성

                _ = int.TryParse(_jWTConfig.RefreshTokenValidityIn.ToString(), out int refreshTokenValidityIn); // Refresh Token 유효기간 설정

                user.RefreshToken = newRefreshToken; // Refresh Token 저장
                user.RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(refreshTokenValidityIn); // Refresh Token 저장

                await _userManager.UpdateAsync(user);

                return Ok(new AuthResponseDTO
                {
                    IsSuccess = true,
                    Token = newJwtToken,
                    RefreshToken = newRefreshToken,
                    Message = "토큰이 성공적으로 갱신되었습니다"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new AuthResponseDTO { IsSuccess = false, Message = ex.Message });
            }
        }

        private ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
        {
            var tokenParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jWTConfig.Key) ?? throw new InvalidOperationException("Secret Key is not found 208")),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenParameters, out SecurityToken securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("잘못된 토큰 입니다. 220");

            return principal;
        }

        //--> 회원정보 상세
        //--> GET api/account/detail
        [Authorize(Roles = "Admin, User, Writer")]
        [HttpGet("detail")]
        public async Task<ActionResult> GetUserDetail()
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (currentUserId is null) return NotFound(new ResponseModel(ResponseCode.Error, "사용자를 찾을 수 없습니다", string.Empty));
            var user = await _userManager.FindByIdAsync(currentUserId);
            if (user is null) return NotFound(new ResponseModel(ResponseCode.Error, "사용자를 찾을 수 없습니다", string.Empty));
            return Ok(new UserDetailDTO()
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

        // 비밀번호 잃어버렸을 때 확인 메일 보내기 (forget (1). Send)
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

                var passwordResetLink = $"https://sutra.buddham.co.kr/reset-password?email={user.Email}&token={WebUtility.UrlEncode(token)}";

                await _emailService.SendEmailAsync(user.Email!, "Reset Your Password", $"Reset your password by <a href='{passwordResetLink}'>clicking here</a>.");

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

        // 비밀번호 재설정 (forget (2). Reset)
        [AllowAnonymous]
        //--> POST api/account/resetpassword
        [HttpPost("resetpassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO resetPasswordDTO)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(resetPasswordDTO.Email);
                // resetPasswordDTO.Token = WebUtility.UrlDecode(resetPasswordDTO.Token);

                if (user is null)
                    return NotFound(
                        new AuthResponseDTO
                        {
                            IsSuccess = false,
                            Message = "사용자를 찾을 수 없습니다"
                        }
                    );

                var result = await _userManager.ResetPasswordAsync(user, resetPasswordDTO.Token, resetPasswordDTO.NewPassword);

                if (!result.Succeeded)
                    return BadRequest(
                        new AuthResponseDTO
                        {
                            IsSuccess = false,
                            Message = string.Join(", ", result.Errors.Select(x => x.Description).ToArray())
                        }
                    );

                return Ok(
                    new AuthResponseDTO
                    {
                        IsSuccess = true,
                        Message = "비밀번호를 성공적으로 변경했습니다"
                    }
                );
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

        // 이메일 확인을 위한 링크 보내기 (confirm email (1). Send)
        //--> POST api/account/confirm-my-email
        [HttpPost("confirm-my-email")]
        public async Task<IActionResult> ConfrimMyEmail(ConfirmEmailDTO confirmEmailDTO)
        {
            try
            {
                AppUser? user = await _userManager.FindByEmailAsync(confirmEmailDTO.Email);

                if (user is null)
                {
                    return Ok(new AuthResponseDTO
                    {
                        IsSuccess = false,
                        Message = "사용자를 찾을 수 없습니다"
                    });
                }

                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                var conformEmailLink = $"https://sutra.buddham.co.kr/confirm-replay-email?email={user.Email}&token={WebUtility.UrlEncode(token)}";

                await _emailService.SendEmailAsync(user.Email!, "Confirm Email", $"이메일 확인 링크 <a href='{conformEmailLink}'>clicking here</a>.");

                return Ok(new AuthResponseDTO
                {
                    IsSuccess = true,
                    Message = "이메일 확인을 위한 메일링크를 보냈습니다. 메일을 확인하여 보세요."
                });

            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel(ResponseCode.Error, $"서버에러: {ex.Message}", string.Empty));
            }
        }

        // 이메일 확인을 위한 링크 보내기 (confirm email (2). Send)
        // [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("confirm-replay-email")]
        public async Task<IActionResult> ConfirmEmail(ConfirmReplayDTO confirmReplayDTO)
        {
            var user = await _userManager.FindByEmailAsync(confirmReplayDTO.Email);

            if (user is null || confirmReplayDTO.Token == null) return NotFound(new AuthResponseDTO { IsSuccess = false, Message = "잘못된 토큰입니다." });

            confirmReplayDTO.Token = confirmReplayDTO.Token.Replace(" ", "+");

            var result = await _userManager.ConfirmEmailAsync(user, confirmReplayDTO.Token);

            if (result.Succeeded) return Ok(new AuthResponseDTO { IsSuccess = true, Message = "이메일 확인에 성공했습니다" });

            return BadRequest(new AuthResponseDTO { IsSuccess = false, Message = "이메일 확인에 실패했습니다" });
        }

        // 비밀번호 확인
        //--> POST api/account/checkpassword
        [HttpPost("checkpassword")]
        public async Task<IActionResult> CheckPassword([FromBody] SignInDTO signInDTO)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(signInDTO.Email);

                if (user is null)
                    return NotFound(new ResponseModel(ResponseCode.Error, "사용자를 찾을 수 없습니다", string.Empty));

                var result = await _userManager.CheckPasswordAsync(user, signInDTO.Password);

                if (!result)
                    return Unauthorized(new ResponseModel(ResponseCode.Error, "비밀번호가 일치하지 않습니다", string.Empty));

                return Ok(new ResponseModel(ResponseCode.OK, "비밀번호가 일치합니다", string.Empty));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel(ResponseCode.Error, $"서버에러: {ex.Message}", string.Empty));
            }
        }

        // 로그인 완료 후 나의 비밀번호 변경
        //--> PUT api/account/changepassword
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
        [HttpPut("update")]
        public async Task<IActionResult> UpdateAccount([FromBody] UpdateAccountDTO updateAccountDTO)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);

                if (updateAccountDTO.Email is null)
                    return BadRequest(new ResponseModel(ResponseCode.Error, "이메일을 입력하여 주세요.", string.Empty));

                var user = await _userManager.FindByEmailAsync(updateAccountDTO.Email);

                if (user is null)
                    return NotFound(new ResponseModel(ResponseCode.Error, "사용자를 찾을 수 없습니다", string.Empty));

                user.FullName = updateAccountDTO.FullName;
                user.PhoneNumber = updateAccountDTO.PhoneNumber;

                var result = await _userManager.UpdateAsync(user);

                if (!result.Succeeded)
                    return BadRequest(new ResponseModel(ResponseCode.Error, "회원정보 수정에 실패했습니다", result.Errors.Select(x => x.Description).ToArray()));

                return Ok(new ResponseModel(ResponseCode.OK, "회원정보를 성공적으로 수정했습니다", string.Empty));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel(ResponseCode.Error, $"서버에러: {ex.Message}", string.Empty));
            }
        }

        // 계정 삭제 기능 (관리자 용)
        //--> DELETE api/account/delete
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
        //--> DELETE api/account/cancel-my-account
        [HttpDelete("cancel-my-account")]
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

        // Generate Token
        private string GenerateToken(AppUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jWTConfig.Key);
            var roles = _userManager.GetRolesAsync(user).Result;
            List<Claim> claims =
            [
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Name, user.FullName ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.NameId, user.Id ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Aud, _jWTConfig.Audience),
                new Claim(JwtRegisteredClaimNames.Iss, _jWTConfig.Issuer),
            ];

            foreach (var role in roles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                // Expires = DateTime.UtcNow.AddMinutes(1),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
