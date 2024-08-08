using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ViVaBM.API.Data;
using ViVaBM.API.Models;

namespace ViVaBM.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController(VivabmDbContext context) : ControllerBase
    {
        private readonly VivabmDbContext _context = context;

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

        // POST
        //? api/category
        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
            await _context.Categories.AddAsync(category); // .Add(category);
            var result = await _context.SaveChangesAsync();
            if (result == 0) return BadRequest(new { message = "Failed to create category" });
            return Ok(new { message = "Category created successfully" });
        }

        // PUT
        //? api/category/1
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

