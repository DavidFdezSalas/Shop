using Shop.APIOrders.Dto;
using Shop.APIOrders.Models;

namespace Shop.APIOrders.Services
{
    public interface IOrderService
    {
        Task<OrderResponse> CreateOrderAsync(string userId, CreateOrderRequest request);
        Task<OrderResponse?> GetOrderByIdAsync(string orderId, string userId);
        Task<IEnumerable<OrderResponse>> GetUserOrdersAsync(string userId);
        Task<IEnumerable<OrderResponse>> GetAllOrdersAsync(); // Solo Admin
        Task<bool> UpdateOrderStatusAsync(string orderId, OrderStatus newStatus);
        Task<bool> CancelOrderAsync(string orderId, string userId);
    }
}