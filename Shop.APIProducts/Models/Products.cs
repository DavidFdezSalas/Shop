namespace Shop.APIProducts.Models
{
    public class Products
    {

        public string Id { get; set; } = Guid.NewGuid().ToString();

        public required string Name { get; set; }
 
        public required string Description { get; set; }

        public decimal Price { get; set; }

        public int Stock { get; set; }

        public int? CategoriesId { get; set; }

        public string? ImageUrl { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public Categories? Categories { get; set; }
    }
}
