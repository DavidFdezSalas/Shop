namespace Shop.APIProducts.Models
{

    public class Categories
    {

        public int Id { get; set; }

        public required string Name { get; set; }

        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Products> Products { get; set; } = new List<Products>();
    }
}
