using backend_amatista.Models;
using backendAmatista.Models;
using BackendAmatista.Models;
using BackendAmatista.Models.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackendAmatista.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly DbamatistaContext _dbamatistaContext;
        public ProductController(DbamatistaContext dbamatistaContext)
        {
            _dbamatistaContext = dbamatistaContext;
        }
        [HttpGet]
        [Route("List")]
        public async Task<IActionResult> ListProducts([FromQuery] string? query, [FromQuery] int? limit)
        {
            var productsQuery = _dbamatistaContext.Products
                .Join(
                    _dbamatistaContext.Categories,
                    product => product.IdCategory,
                    category => category.IdCategory,
                    (product, category) => new
                    {
                        id = product.IdProduct,
                        Name = product.Name,
                        Price = product.Price,
                        Item = product.Item,
                        Stock = product.Stock,
                        CategoryName = category.Name
                    }
                );

            // Filtrar por nombre de producto si se proporciona el parámetro
            if (!string.IsNullOrEmpty(query))
            {
                productsQuery = productsQuery.Where(p =>
                    p.Name.Contains(query) ||
                    p.Item.Contains(query)
                );
            }

            // Aplicar límite si se proporciona
            if (limit.HasValue && limit.Value > 0)
            {
                productsQuery = productsQuery.Take(limit.Value);
            }

            var productsWithCategories = await productsQuery.ToListAsync();

            return Ok(productsWithCategories);
        }


        [HttpGet]
        [Route("GetProduct/{id:int}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            var findProduct = await _dbamatistaContext.Products
       .Where(p => p.IdProduct == id)
       .Join(
           _dbamatistaContext.Categories,
           product => product.IdCategory,
           category => category.IdCategory,
           (product, category) => new
           {
               id = product.IdProduct,
               Name = product.Name,
               Price = product.Price,
               Item = product.Item,
               Stock = product.Stock,
               CategoryName = category.Name
           }
       )
       .FirstOrDefaultAsync();

            if (findProduct == null)
            {
                return BadRequest("Product not found");
            }

            return Ok(findProduct);
        }

        [HttpDelete]
        [Route("Delete/{id:int}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            // find id product
            var findProduct = await _dbamatistaContext.Products.FirstOrDefaultAsync(p => p.IdProduct == id);

            // verify if product is null
            if (findProduct == null)
            {
                return NotFound("Product not found");
            }

            // delete product and save changes
            _dbamatistaContext.Products.Remove(findProduct);
            await _dbamatistaContext.SaveChangesAsync();

            return Ok("Product deleted successfully");
        }
        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> CreateProduct([FromBody] ProductDTO productDTO)
        {
            if (productDTO == null)
            {
                return BadRequest("Invalid product data.");
            }

            // Check if the category exists
            var category = await _dbamatistaContext.Categories
                .FirstOrDefaultAsync(c => c.IdCategory == productDTO.ID_Category);

            if (category == null)
            {
                return BadRequest("Category not found.");
            }

            // Create the product
            var product = new Product
            {
                Name = productDTO.Name,
                Price = productDTO.Price,
                Item = productDTO.Item,
                Stock = productDTO.Stock,
                IdCategory = productDTO.ID_Category,
                Active = productDTO.Active
            };

            // Add the product to the database
            _dbamatistaContext.Products.Add(product);
            await _dbamatistaContext.SaveChangesAsync();

            return Ok(product);
        }

        [HttpPut]
        [Route("Edit/{id:int}")]
        public async Task<IActionResult> EditProduct([FromBody] ProductDTO product, int id)
        {
            try
            {
                var findProduct = await _dbamatistaContext.Products.FirstOrDefaultAsync(c => c.IdProduct == id);
                if (findProduct == null)
                {
                    return BadRequest("Product not found");
                }

                //update propertys of product
                findProduct.Name = product.Name;
                findProduct.Price = product.Price;
                findProduct.Item = product.Item;
                findProduct.Active = product.Active;
                findProduct.Stock = product.Stock;
                findProduct.IdCategory = product.ID_Category;

                await _dbamatistaContext.SaveChangesAsync();
                return Ok("Product update Succesfully");

            } catch (DbUpdateException ex)
            {
                
                var innerException = ex.InnerException;
                Console.WriteLine(innerException?.Message);
                throw; 
            }

        }
    }
}