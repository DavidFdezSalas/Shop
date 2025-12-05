using Microsoft.EntityFrameworkCore;
using Shop.APIProducts.Data;
using Shop.APIProducts.Dto.Categories;
using CategoryModel = Shop.APIProducts.Models.Categories;

namespace Shop.APIProducts.Services.Categories
{
    public class CategoryService : ICategoryService
    {
        private readonly ProductDbContext _context;
        private readonly ILogger<CategoryService> _logger;

        public CategoryService(ProductDbContext context, ILogger<CategoryService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<CategoryResponse> CreateCategoryAsync(CreateCategoryRequest request)
        {
            var category = new CategoryModel
            {
                Name = request.Name,
                Description = request.Description,
                CreatedAt = DateTime.UtcNow
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Categoría creada: {CategoryName} con ID: {CategoryId}", category.Name, category.Id);

            return new CategoryResponse
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                CreatedAt = category.CreatedAt,
                ProductCount = 0
            };
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var category = await _context.Categories
         .Include(c => c.Products)
         .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                return false;
            }

            if (category.Products.Any())
            {
                _logger.LogWarning("No se puede eliminar la categoría {CategoryId} porque tiene {ProductCount} productos asociados",
                    id, category.Products.Count);
                return false;
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Categoría eliminada: {CategoryName} con ID: {CategoryId}", category.Name, category.Id);

            return true;
        }

        public async Task<IEnumerable<CategoryResponse>> GetAllCategoriesAsync()
        {
            var categories = await _context.Categories
                .Include(c => c.Products)
                .ToListAsync();

            return categories.Select(c => new CategoryResponse
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                CreatedAt = c.CreatedAt,
                ProductCount = c.Products.Count()
            });
        }

        public async Task<CategoryResponse?> GetCategoryByIdAsync(int id)
        {
            var category = await _context.Categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                return null;
            }

            return new CategoryResponse
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                CreatedAt = category.CreatedAt,
                ProductCount = category.Products.Count
            };
        }

        public async Task<bool> UpdateCategoryAsync(int id, UpdateCategoryRequest request)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return false;
            }

            category.Name = request.Name;
            category.Description = request.Description;

            _context.Categories.Update(category);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Categoría actualizada: {CategoryName} con ID: {CategoryId}", category.Name, category.Id);

            return true;
        }
    }
}
