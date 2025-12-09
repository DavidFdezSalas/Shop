using Shop.APIOrders.Models;

namespace Shop.APIOrders.Dto
{
    public class UpdateOrderStatusRequest
    {
        public OrderStatus Status { get; set; }
    }
}