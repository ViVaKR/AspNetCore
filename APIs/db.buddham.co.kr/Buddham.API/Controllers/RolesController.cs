using Buddham.API.Models;
using Buddham.SharedLib.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Buddham.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RolesController(
    RoleManager<IdentityRole> roleManager,
    UserManager<AppUser> userManager
) : ControllerBase
{
    private readonly RoleManager<IdentityRole> _roleManager = roleManager;
    private readonly UserManager<AppUser> _userManager = userManager;

    //--> GET api/roles
    [Authorize]
    [HttpGet]
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
            if (roles.Count == 0) return NotFound("역할이 존재하지 않습니다.");
            roles.ForEach(role =>
            {
                var users = _userManager.GetUsersInRoleAsync(role.Name!).Result;
                role.TotalUsers = users.Count;
            });

            return Ok(roles);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    // POST api/roles
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> CreateRole([FromBody] CreateRoleDTO createRoleDTO)
    {
        if (string.IsNullOrEmpty(createRoleDTO.RoleName))
            return BadRequest(new { message = "롤 이름은 필수입니다." });

        bool exist = await _roleManager.RoleExistsAsync(createRoleDTO.RoleName);

        if (exist) return BadRequest(new { message = "이미 존재하는 롤입니다." });

        var role = new IdentityRole(createRoleDTO.RoleName);

        var result = await _roleManager.CreateAsync(role);

        if (result.Succeeded) return Ok(new { message = "롤 생성 성공!" });

        return BadRequest(new { message = "롤 생성 실패!" });
    }

    // DELETE api/roles/{id}
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRole(string id)
    {
        var role = await _roleManager.FindByIdAsync(id);

        if (role == null) return NotFound(new { message = "Role not found" });

        var result = await _roleManager.DeleteAsync(role);

        if (result.Succeeded)
            return Ok(new { message = "Role deleted successfully" });

        return BadRequest(new { message = "Role deletion failed" });
    }

    // Assign Role to User
    [Authorize(Roles = "Admin")]
    [HttpPost("assign")]
    public async Task<IActionResult> AssignRoleToUser([FromBody] AssignRoleDTO assignRoleDTO)
    {
        var user = await _userManager.FindByIdAsync(assignRoleDTO.UserId);

        if (user == null) return NotFound(new { message = $"사용자 ( {assignRoleDTO.UserId} )가 존재하지 않습니다." });

        var role = await _roleManager.FindByIdAsync(assignRoleDTO.RoleId);

        if (role == null) return NotFound(new { message = $"역할 ( {assignRoleDTO.RoleId} )이 존재하지 않습니다." });

        var result = await _userManager.AddToRoleAsync(user, role.Name!);

        if (result.Succeeded) return Ok(new { message = $"사용자 ( {user.UserName} )에게 역할 ( {role.Name} ) 할당 완료!" });

        return BadRequest(new { message = "역할 할당 실패" });
    }
}
