using Microsoft.EntityFrameworkCore;
using Shop.APIProducts.Data;
using Shop.APIProducts.Dto.Products;
using ProductModel = Shop.APIProducts.Models.Products;

namespace Shop.APIProducts.Services.Products
{
    public class ProductService : IProductService
    {
        private readonly ProductDbContext _context;
        private readonly ILogger<ProductService> _logger;

        public ProductService(ProductDbContext context, ILogger<ProductService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<ProductResponse>> GetAllProductsAsync()
        {
            var products = await _context.Products
                .Include(p => p.Categories)
                .ToListAsync();

            return products.Select(p => MapToResponse(p));
        }

        public async Task<ProductResponse?> GetProductByIdAsync(string id)
        {
            var product = await _context.Products
                .Include(p => p.Categories)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return null;
            }

            return MapToResponse(product);
        }

        public async Task<IEnumerable<ProductResponse>> GetProductsByCategoryAsync(int categoryId)
        {
            var products = await _context.Products
                .Include(p => p.Categories)
                .Where(p => p.CategoriesId == categoryId)
                .ToListAsync();

            return products.Select(p => MapToResponse(p));
        }

        public async Task<ProductResponse> CreateProductAsync(CreateProductRequest request)
        {
            var product = new ProductModel
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                Stock = request.Stock,
                CategoriesId = request.CategoriesId,
                ImageUrl = request.ImageUrl,
                CreatedAt = DateTime.UtcNow
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Producto creado: {ProductName} con ID: {ProductId}", product.Name, product.Id);

            // Cargar la categoría para el response
            await _context.Entry(product)
                .Reference(p => p.Categories)
                .LoadAsync();

            return MapToResponse(product);
        }

        public async Task<bool> UpdateProductAsync(string id, UpdateProductRequest request)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return false;
            }

            product.Name = request.Name;
            product.Description = request.Description;
            product.Price = request.Price;
            product.Stock = request.Stock;
            product.CategoriesId = request.CategoriesId;
            product.ImageUrl = request.ImageUrl;
            product.UpdatedAt = DateTime.UtcNow;

            _context.Products.Update(product);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Producto actualizado: {ProductName} con ID: {ProductId}", product.Name, product.Id);

            return true;
        }

        public async Task<bool> DeleteProductAsync(string id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return false;
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Producto eliminado: {ProductName} con ID: {ProductId}", product.Name, product.Id);

            return true;
        }

        private static ProductResponse MapToResponse(ProductModel product)
        {
            return new ProductResponse
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                CategoriesId = product.CategoriesId,
                CategoryName = product.Categories?.Name,
                ImageUrl = product.ImageUrl,
                CreatedAt = product.CreatedAt,
                UpdatedAt = product.UpdatedAt
            };
        }
    }
}
