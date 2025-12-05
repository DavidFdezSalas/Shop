namespace Shop.APIProducts.Dto.Categories
{
    public class UpdateCategoryRequest
    {
        public required string Name { get; set; }
        public string? Description { get; set; }
    }
}