using Asp.Versioning;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shop.APIProducts.Dto.Products;
using Shop.APIProducts.Services.Products;

namespace Shop.APIProducts.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(IProductService productService, ILogger<ProductsController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        // GET: api/v1/Products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductResponse>>> GetAllProducts()
        {
            var products = await _productService.GetAllProductsAsync();
            return Ok(products);
        }

        // GET: api/v1/Products/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductResponse>> GetProductById(string id)
        {
            var product = await _productService.GetProductByIdAsync(id);

            if (product == null)
            {
                return NotFound(new { message = $"Producto con ID {id} no encontrado." });
            }

            return Ok(product);
        }

        // GET: api/v1/Products/category/{categoryId}
        [HttpGet("category/{categoryId}")]
        public async Task<ActionResult<IEnumerable<ProductResponse>>> GetProductsByCategory(int categoryId)
        {
            var products = await _productService.GetProductsByCategoryAsync(categoryId);
            return Ok(products);
        }

        // POST: api/v1/Products
        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<ProductResponse>> CreateProduct(
            [FromBody] CreateProductRequest request,
            [FromServices] IValidator<CreateProductRequest> validator)
        {
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var product = await _productService.CreateProductAsync(request);

            return CreatedAtAction(
                nameof(GetProductById),
                new { id = product.Id },
                product);
        }

        // PUT: api/v1/Products/{id}
        [HttpPut("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> UpdateProduct(
            string id,
            [FromBody] UpdateProductRequest request,
            [FromServices] IValidator<UpdateProductRequest> validator)
        {
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var result = await _productService.UpdateProductAsync(id, request);

            if (!result)
            {
                return NotFound(new { message = $"Producto con ID {id} no encontrado." });
            }

            return NoContent();
        }

        // DELETE: api/v1/Products/{id}
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DeleteProduct(string id)
        {
            var result = await _productService.DeleteProductAsync(id);

            if (!result)
            {
                return NotFound(new { message = $"Producto con ID {id} no encontrado." });
            }

            return NoContent();
        }
    }
}
