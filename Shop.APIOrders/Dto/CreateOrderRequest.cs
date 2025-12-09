namespace Shop.APIOrders.Dto
{
    public class CreateOrderRequest
    {
        public required string ShippingAddress { get; set; }
        public required List<OrderItemRequest> Items { get; set; }
    }

    public class OrderItemRequest
    {
        public required string ProductId { get; set; }
        public int Quantity { get; set; }
    }
}