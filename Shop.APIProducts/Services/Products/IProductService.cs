using Shop.APIProducts.Dto.Products;

namespace Shop.APIProducts.Services.Products
{
    public interface IProductService
    {
        Task<IEnumerable<ProductResponse>> GetAllProductsAsync();
        Task<ProductResponse?> GetProductByIdAsync(string id);
        Task<IEnumerable<ProductResponse>> GetProductsByCategoryAsync(int categoryId);
        Task<ProductResponse> CreateProductAsync(CreateProductRequest request);
        Task<bool> UpdateProductAsync(string id, UpdateProductRequest request);
        Task<bool> DeleteProductAsync(string id);
    }
}
