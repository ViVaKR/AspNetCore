using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ViVaKR.API.Data;
using ViVaKR.API.DTOs;
using ViVaKR.API.Models;

namespace ViVaKR.API.Controllers;

[ApiController]
[Route("api/role")]
public class RoleController(
    RoleManager<IdentityRole> roleManager,
    UserManager<AppUser> userManager,
    VivaKRDbContext context
) : ControllerBase
{
    //--> 회원 상세 정보
    // GET api/role/user-list
    [Authorize(Roles = "Admin")]
    [HttpGet("user-group-list")]
    public async Task<ActionResult<IEnumerable<RoleResponseDTO>>> GetRolesGroups()
    {
        try
        {
            var roles = await roleManager.Roles
                .GroupJoin(
                    context.UserRoles, // AspNetUserRoles 테이블
                    role => role.Id,
                    userRole => userRole.RoleId,
                    (role, userRoles) => new RoleResponseDTO
                    {
                        Id = role.Id,
                        Name = role.Name,
                        TotalUsers = userRoles.Count(), // 해당 역할에 속한 사용자 수
                        NormalizedName = role.NormalizedName,
                        ConcurrencyStamp = role.ConcurrencyStamp
                    }).ToListAsync();

            if (roles.Count == 0)
                return NotFound(new ResponseModel(ResponseCode.Error, "역할이 존재하지 않습니다.", string.Empty));

            return Ok(roles);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                new ResponseModel(ResponseCode.Error, ex.Message, string.Empty));
        }
    }

    //--> GET
    //--> api/role/list
    [Authorize(Roles = "Admin")]
    [HttpGet("list")]
    public async Task<ActionResult<IEnumerable<RoleResponseDTO>>> GetRoles()
    {
        try
        {
            var roles = await roleManager.Roles.Select(x => new RoleResponseDTO
            {
                Id = x.Id,
                Name = x.Name,
                TotalUsers = 0,
                NormalizedName = x.NormalizedName,
                ConcurrencyStamp = x.ConcurrencyStamp
            }).ToListAsync();

            if (roles.Count == 0)
                return NotFound(new ResponseModel(ResponseCode.Error, "역할이 존재하지 않습니다.", string.Empty));
            return Ok(roles);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel(ResponseCode.Error, ex.Message, string.Empty));
        }
    }

    //--> POST
    //--> api/role/create
    [Authorize]
    [HttpPost("create")]
    public async Task<ActionResult<ResponseModel>> CreateRole([FromBody] RoleRequestDTO roleRequestDTO)
    {
        try
        {
            if (string.IsNullOrEmpty(roleRequestDTO.RoleName))
                return BadRequest(new ResponseModel(ResponseCode.Error, "역할 정보가 누락되었습니다.", string.Empty));

            var isRoleExist = await roleManager.RoleExistsAsync(roleRequestDTO.RoleName!);

            if (isRoleExist)
                return BadRequest(new ResponseModel(ResponseCode.Error, "이미 존재하는 역할입니다.", string.Empty));

            var role = new IdentityRole(roleRequestDTO.RoleName);

            var result = await roleManager.CreateAsync(role);

            if (result.Succeeded)
                return Ok(new ResponseModel(ResponseCode.OK, "역할이 생성되었습니다.", string.Empty));
            else
                return BadRequest(new ResponseModel(ResponseCode.Error, result.Errors.FirstOrDefault()?.Description ?? "알수 없는 이유로 역할 생성에 실패하였습니다.", string.Empty));
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel(ResponseCode.Error, ex.Message, string.Empty));
        }
    }

    //--> DELETE
    //--> api/role/delete/{id}
    [Authorize(Roles = "Admin")]
    [HttpDelete("delete/{id}")]
    public async Task<ActionResult<ResponseModel>> DeleteRole(string id)
    {
        try
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest(new ResponseModel(ResponseCode.Error, "역할 정보가 누락되었습니다.", string.Empty));

            var role = await roleManager.FindByIdAsync(id);

            if (role == null)
                return NotFound(new ResponseModel(ResponseCode.Error, "삭제할 역할이 존재하지 않습니다.", string.Empty));

            var result = await roleManager.DeleteAsync(role);

            if (result.Succeeded)
                return Ok(new ResponseModel(ResponseCode.OK, "역할이 삭제되었습니다.", string.Empty));
            else
                return BadRequest(new ResponseModel(ResponseCode.Error, result.Errors.FirstOrDefault()?.Description ?? "알수 없는 이유로 역할 삭제에 실패하였습니다.", string.Empty));
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel(ResponseCode.Error, ex.Message, string.Empty));
        }
    }

    //--> api/role/remove-role-from-user
    [Authorize(Roles = "Admin")]
    [HttpDelete("remove")]
    public async Task<ActionResult<ResponseModel>> RemoveRoleFromUser([FromBody] AssignRoleRequestDTO assignRoleRequestDTO)
    {
        try
        {
            if (string.IsNullOrEmpty(assignRoleRequestDTO.UserId) || string.IsNullOrEmpty(assignRoleRequestDTO.RoleId))
                return BadRequest(new ResponseModel(ResponseCode.Error, "역할 또는 사용자 정보가 누락되었습니다.", string.Empty));

            var user = await userManager.FindByIdAsync(assignRoleRequestDTO.UserId);
            if (user is null)
                return NotFound(new ResponseModel(ResponseCode.Error, "사용자 정보가 존재하지 않습니다.", string.Empty));

            var role = await roleManager.FindByIdAsync(assignRoleRequestDTO.RoleId);
            if (role is null)
                return NotFound(new ResponseModel(ResponseCode.Error, "역할 정보가 존재하지 않습니다.", string.Empty));

            var result = await userManager.RemoveFromRoleAsync(user, role.Name!);
            if (result.Succeeded)
                return Ok(new ResponseModel(ResponseCode.OK, "역할이 사용자에게서 제거되었습니다.", string.Empty));
            else
                return BadRequest(new ResponseModel(ResponseCode.Error, result.Errors.FirstOrDefault()?.Description ?? "역할 제거에 실패했습니다.", string.Empty));
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel(ResponseCode.Error, ex.Message, string.Empty));
        }
    }

    //--> api/role/assign
    [Authorize(Roles = "Admin")]
    [HttpPost("assign")]
    public async Task<ActionResult<ResponseModel>> AssignRoleToUser([FromBody] AssignRoleRequestDTO assignRoleRequestDTO)
    {
        try
        {
            if (string.IsNullOrEmpty(assignRoleRequestDTO.UserId) || string.IsNullOrEmpty(assignRoleRequestDTO.RoleId))
                return BadRequest(new ResponseModel(ResponseCode.Error, "역할 또는 사용자 정보가 누락되었습니다.", string.Empty));

            var user = await userManager.FindByIdAsync(assignRoleRequestDTO.UserId);

            if (user is null)
                return NotFound(new ResponseModel(ResponseCode.Error, "사용자 정보가 존재하지 않습니다.", string.Empty));

            var role = await roleManager.FindByIdAsync(assignRoleRequestDTO.RoleId);

            if (role is null)
                return NotFound(new ResponseModel(ResponseCode.Error, "역할 정보가 존재하지 않습니다.", string.Empty));

            var result = await userManager.AddToRoleAsync(user, role.Name!);

            if (result.Succeeded)
                return Ok(new ResponseModel(ResponseCode.OK, "역할이 사용자에게 할당되었습니다.", string.Empty));
            else
                return BadRequest(new ResponseModel(ResponseCode.Error, result.Errors.FirstOrDefault()?.Description ?? "알수 없는 이유로 역할 할당에 실패하였습니다.", string.Empty));
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel(ResponseCode.Error, ex.Message, string.Empty));
        }
    }
}
