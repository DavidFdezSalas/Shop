namespace Shop.APIProducts.Dto.Products
{
    public class CreateProductRequest
    {
        public required string Name { get; set; }
        public required string Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public int? CategoriesId { get; set; }
        public string? ImageUrl { get; set; }
    }
}