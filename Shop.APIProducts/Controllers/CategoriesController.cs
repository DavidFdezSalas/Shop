
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.APIProducts.Data;
using Shop.APIProducts.Models;

namespace Shop.APIProducts.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize]
    public class CategoriesController : ControllerBase
    {
        private readonly ProductDbContext _context;
        private readonly ILogger<CategoriesController> _logger;

        public CategoriesController(ProductDbContext context, ILogger<CategoriesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Categories>>> GetCategories()
        {
            var categories = await _context.Categories
                .Include(c => c.Products)
                .ToListAsync();

            return Ok(categories);
        }
    }
}
