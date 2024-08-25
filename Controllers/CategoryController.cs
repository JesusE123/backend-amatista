using backend_amatista.Models;
using backendAmatista.Models;
using backendAmatista.Models.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backendAmatista.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly DbamatistaContext _dbamatistaContext;
        public CategoryController(DbamatistaContext dbamatistaContext)
        {
            _dbamatistaContext = dbamatistaContext;
        }

        [HttpGet]
        [Route("List")]
        public async Task<IActionResult> ListCategories()
        {
            var categories = await _dbamatistaContext.Categories
                .Select(c => new {c.IdCategory, c.Name})
                .ToListAsync();

            return Ok(categories);
            

        }

        [HttpPut]
        [Route("Edit/{id:int}")]
        public async Task<IActionResult> DeleteCategory([FromBody] CategoryDTO category ,int id)
        {
            try
            {
                var findCategory = await _dbamatistaContext.Categories.FirstOrDefaultAsync(c => c.IdCategory == id);
                if (findCategory == null)
                {
                    return BadRequest("Product not found");
                }

                //update propertys of product
                findCategory.Name = category.Name;
              

                await _dbamatistaContext.SaveChangesAsync();
                return Ok("category has update Succesfully");

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryDTO category)
        {
            if (category == null)
            {
                BadRequest("Invalid data");
            }

            var newCategory = new Category
            {
                Name = category.Name,
                Active = category.Active,
            };

            _dbamatistaContext.Categories.Add(newCategory);
            await _dbamatistaContext.SaveChangesAsync();

            return Ok(newCategory);
        }
    }
}
