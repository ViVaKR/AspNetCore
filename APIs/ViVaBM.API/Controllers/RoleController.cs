using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ViVaBM.API.DTOs;
using ViVaBM.API.Models;

namespace ViVaBM.API.Controllers;

[ApiController]
[Route("api/role")]
public class RoleController(
    RoleManager<IdentityRole> roleManager,
    UserManager<AppUser> userManager
) : ControllerBase
{
    private readonly RoleManager<IdentityRole> _roleManager = roleManager;
    private readonly UserManager<AppUser> _userManager = userManager;

    //--> GET
    //--> api/role/list
    [HttpGet("list")]
    public async Task<ActionResult<IEnumerable<RoleResponseDTO>>> GetRoles()
    {
        try
        {
            var roles = await _roleManager.Roles.Select(x => new RoleResponseDTO
            {
                Id = x.Id,
                Name = x.Name,
                TotalUsers = 0,
                NormalizedName = x.NormalizedName,
                ConcurrencyStamp = x.ConcurrencyStamp
            }).ToListAsync();

            if (roles == null || roles.Count == 0)
                return NotFound(new ResponseModel(ResponseCode.Error, "역할이 존재하지 않습니다.", string.Empty));


            // roles.ForEach(role =>
            // {
            //     if (string.IsNullOrEmpty(role.Name))
            //         return;
            //     IList<AppUser> users = _userManager.GetUsersInRoleAsync(role.Name!).Result;
            //     role.TotalUsers = users.Count;
            // });

            return Ok(roles);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel(ResponseCode.Error, ex.Message, string.Empty));
        }
    }

    //--> POST
    //--> api/role/create
    [HttpPost("create")]
    public async Task<ActionResult<ResponseModel>> CreateRole([FromBody] RoleRequestDTO roleRequestDTO)
    {
        try
        {
            if (string.IsNullOrEmpty(roleRequestDTO.RoleName))
                return BadRequest(new ResponseModel(ResponseCode.Error, "역할 정보가 누락되었습니다.", string.Empty));

            bool isRoleExist = await _roleManager.RoleExistsAsync(roleRequestDTO.RoleName!);

            if (isRoleExist)
                return BadRequest(new ResponseModel(ResponseCode.Error, "이미 존재하는 역할입니다.", string.Empty));

            var role = new IdentityRole(roleRequestDTO.RoleName);

            var result = await _roleManager.CreateAsync(role);

            if (result.Succeeded)
                return Ok(new ResponseModel(ResponseCode.Success, "역할이 생성되었습니다.", string.Empty));
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
    [HttpDelete("delete/{id}")]
    public async Task<ActionResult<ResponseModel>> DeleteRole(string id)
    {
        try
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest(new ResponseModel(ResponseCode.Error, "역할 정보가 누락되었습니다.", string.Empty));

            var role = await _roleManager.FindByIdAsync(id);

            if (role == null)
                return NotFound(new ResponseModel(ResponseCode.Error, "삭제할 역할이 존재하지 않습니다.", string.Empty));

            var result = await _roleManager.DeleteAsync(role);

            if (result.Succeeded)
                return Ok(new ResponseModel(ResponseCode.Success, "역할이 삭제되었습니다.", string.Empty));
            else
                return BadRequest(new ResponseModel(ResponseCode.Error, result.Errors.FirstOrDefault()?.Description ?? "알수 없는 이유로 역할 삭제에 실패하였습니다.", string.Empty));
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel(ResponseCode.Error, ex.Message, string.Empty));
        }
    }

    //--> Assign Role to User
    //--> api/role/assign
    [HttpPost("assign")]
    public async Task<ActionResult<ResponseModel>> AssignRoleToUser([FromBody] AssignRoleRequestDTO assignRoleRequestDTO)
    {
        try
        {
            if (string.IsNullOrEmpty(assignRoleRequestDTO.UserId) || string.IsNullOrEmpty(assignRoleRequestDTO.RoleId))
                return BadRequest(new ResponseModel(ResponseCode.Error, "역할 또는 사용자 정보가 누락되었습니다.", string.Empty));

            var user = await _userManager.FindByIdAsync(assignRoleRequestDTO.UserId);

            if (user is null)
                return NotFound(new ResponseModel(ResponseCode.Error, "사용자 정보가 존재하지 않습니다.", string.Empty));

            var role = await _roleManager.FindByNameAsync(assignRoleRequestDTO.RoleId);

            if (role is null)
                return NotFound(new ResponseModel(ResponseCode.Error, "역할 정보가 존재하지 않습니다.", string.Empty));

            var result = await _userManager.AddToRoleAsync(user, role.Name!);

            if (result.Succeeded)
                return Ok(new ResponseModel(ResponseCode.Success, "역할이 사용자에게 할당되었습니다.", string.Empty));
            else
                return BadRequest(new ResponseModel(ResponseCode.Error, result.Errors.FirstOrDefault()?.Description ?? "알수 없는 이유로 역할 할당에 실패하였습니다.", string.Empty));
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel(ResponseCode.Error, ex.Message, string.Empty));
        }
    }
}
