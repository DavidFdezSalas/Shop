namespace Shop.APIOrders.Models
{
    public class OrderItem
    {
        public int Id { get; set; }

        public string OrderId { get; set; } = string.Empty;

        public required string ProductId { get; set; }

        public required string ProductName { get; set; }

        public decimal Price { get; set; }

        public int Quantity { get; set; }

        public decimal Subtotal => Price * Quantity;

        public Order? Order { get; set; }
    }
}