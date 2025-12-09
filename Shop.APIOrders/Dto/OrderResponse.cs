using Shop.APIOrders.Models;

namespace Shop.APIOrders.Dto
{
    public class OrderResponse
    {
        public required string Id { get; set; }
        public required string UserId { get; set; }
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; }
        public string? ShippingAddress { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<OrderItemResponse> Items { get; set; } = new();
    }

    public class OrderItemResponse
    {
        public int Id { get; set; }
        public required string ProductId { get; set; }
        public required string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal Subtotal { get; set; }
    }
}