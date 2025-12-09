namespace Shop.APIOrders.Models
{
    public class Order
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public required string UserId { get; set; }

        public decimal TotalAmount { get; set; }

        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        public string? ShippingAddress { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}