using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ViVaBM.API.DTOs;
using ViVaBM.API.Helpers;
using ViVaBM.API.Models;

namespace ViVaBM.API.Controllers;

//--> api/account
[Route("api/[controller]")]
[ApiController]
public class AccountController(UserManager<AppUser> userManager) : ControllerBase
{
    private readonly UserManager<AppUser> _userManager = userManager;

    //--> 회원 전체 목록
    //--> GET
    //--> api/account/list
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

    // 회원가입
    //--> POST
    //--> api/account/signup
    [HttpPost("signup")]
    public async Task<IActionResult> SignUp([FromBody] SignUpDTO signUpDTO)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(new ResponseModel(ResponseCode.Error, "Invalid model", string.Empty));

            if (string.IsNullOrEmpty(signUpDTO.Email) || string.IsNullOrEmpty(signUpDTO.Password))
                return BadRequest(new ResponseModel(ResponseCode.Error, "Email or password is empty", string.Empty));

            var check = _userManager.Users.Any(x => x.Email!.Equals(signUpDTO.Email));

            if (check)
                return BadRequest(new ResponseModel(ResponseCode.Error, "이미등록된 회원입니다.", signUpDTO.Email));

            var user = new AppUser
            {
                UserName = signUpDTO.Email,
                Email = signUpDTO.Email,
                FullName = signUpDTO.FullName
            };

            var result = await _userManager.CreateAsync(user, signUpDTO.Password);

            if (!result.Succeeded)
            {
                return BadRequest(new ResponseModel(ResponseCode.Error, "회원등록에 실패하였습니다.",
                result.Errors.Select(x => x.Description).ToArray()));
            }

            var tempUser = await _userManager.FindByEmailAsync(signUpDTO.Email);

            if (tempUser is null)
                return BadRequest(new ResponseModel(ResponseCode.Error, "사용자 등록에 문제가 발생하여 사용자를 찾을 수 없습니다.", string.Empty));

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
    [HttpPost("signin")]
    public async Task<IActionResult> SignIn([FromBody] SignInDTO signInDTO)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(new ResponseModel(ResponseCode.Error, "Invalid model", string.Empty));

            if (string.IsNullOrEmpty(signInDTO.Email) || string.IsNullOrEmpty(signInDTO.Password))
                return BadRequest(new ResponseModel(ResponseCode.Error, "Email or password is empty", string.Empty));

            var user = await _userManager.FindByEmailAsync(signInDTO.Email);

            if (user is null)
                return BadRequest(new ResponseModel(
                    ResponseCode.Error,
                    "사용자를 찾을 수 없습니다.",
                    signInDTO.Email));

            var result = await _userManager.CheckPasswordAsync(user, signInDTO.Password);

            if (!result)
                return BadRequest(new ResponseModel(ResponseCode.Error, "비밀번호가 일치하지 않습니다.", string.Empty));

            string token = GenerateToken(user);

            var refreshToken = GenerateRefreshToken();

            var rs = int.TryParse(AuthSettings.RefreshTokenValidityIn.ToString(), out int refreshTokenValidity);

            if (!rs)
                return BadRequest(new ResponseModel(ResponseCode.Error, "RefreshTokenValidityIn 설정값이 잘못되었습니다.", string.Empty));

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
                Message = $"로그인 실패: {ex.Message}"
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
        var key = Encoding.ASCII.GetBytes(AuthSettings.PrivateKey);
        var roles = _userManager.GetRolesAsync(user).Result;

        List<Claim> claims =
        [
            new Claim(JwtRegisteredClaimNames.Email, user.Email!),
            new Claim(JwtRegisteredClaimNames.Name, user.FullName!),
            new Claim(JwtRegisteredClaimNames.NameId, user.Id),
            new Claim(JwtRegisteredClaimNames.Aud, AuthSettings.Audience),
            new Claim(JwtRegisteredClaimNames.Iss, AuthSettings.Issuer)
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
    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken(TokenDTO tokenDTO)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(new ResponseModel(ResponseCode.Error, "Invalid model", string.Empty));

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

            var rs = int.TryParse(AuthSettings.RefreshTokenValidityIn.ToString(), out int refreshTokenValidityIn);

            if (!rs)
                return BadRequest(new ResponseModel(ResponseCode.Error, "RefreshTokenValidityIn 설정값이 잘못되었습니다.", string.Empty));
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

    private static ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
    {
        var tokenParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AuthSettings.PrivateKey)),
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
    //--> GET aapi/account/details
    [HttpGet("details")]
    public async Task<IActionResult> GetDetail()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId is null) return NotFound(new ResponseModel(ResponseCode.Error, "User not found", string.Empty));

        var user = await _userManager.FindByIdAsync(userId);

        if (user is null) return NotFound(new ResponseModel(ResponseCode.Error, "User not found", string.Empty));

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
}
