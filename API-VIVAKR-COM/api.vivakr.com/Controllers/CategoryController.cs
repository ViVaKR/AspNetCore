using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ViVaKR.API.Data;
using ViVaKR.API.Models;

namespace ViVaKR.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController(VivaKRDbContext context) : ControllerBase
    {
        private readonly VivaKRDbContext _context = context;

        // GET
        //? api/category/list
        [HttpGet("list")]
        public async Task<IEnumerable<Category>> GetAll()
        {
            var categories = await _context.Categories.ToListAsync();
            return categories;
        }

        // GET
        //? api/category/1
        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> GetById(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return NotFound(new { message = "Category not found" });

            return Ok(category);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Category category)
        {
            try
            {
                if (category == null || string.IsNullOrWhiteSpace(category.Name))
                {
                    return BadRequest(new { message = "카테고리 이름은 필수입니다." });
                }

                var existingCategory = await _context.Categories.AnyAsync(c => c.Name == category.Name);
                if (existingCategory)
                {
                    return BadRequest(new { message = "이미 존재하는 카테고리 이름입니다." });
                }

                await _context.Categories.AddAsync(category);
                var result = await _context.SaveChangesAsync();
                if (result == 0)
                {
                    return BadRequest(new { message = "카테고리 생성에 실패했습니다." });
                }

                return Ok(new { id = category.Id, name = category.Name, message = "카테고리가 성공적으로 생성되었습니다." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"서버 오류: {ex.Message}" });
            }
        }

        // PUT
        //? api/category/1
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Category category)
        {
            if (id != category.Id || id != category.Id) return BadRequest(new { message = "Invalid category" });

            _context.Entry(category).State = EntityState.Modified;
            var result = await _context.SaveChangesAsync();
            if (result == 0) return BadRequest(new { message = "Failed to update category" });
            return Ok(new { message = "Category updated successfully" });
        }

        // DELETE
        //? api/category/1
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return NotFound(new { message = "Category not found" });

            _context.Categories.Remove(category);
            var result = await _context.SaveChangesAsync();
            if (result == 0) return BadRequest(new { message = "Failed to delete category" });
            return Ok(new { message = "Category deleted successfully" });
        }
    }
}

