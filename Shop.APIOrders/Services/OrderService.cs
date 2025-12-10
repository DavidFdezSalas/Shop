using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shop.APIOrders.Data;
using Shop.APIOrders.Dto;
using Shop.APIOrders.Models;
using System.Net.Http.Json;
using System.Net.Http.Headers;

namespace Shop.APIOrders.Services
{
    public class OrderService : IOrderService
    {
        private readonly OrderDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<OrderService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public OrderService(
            OrderDbContext context,
            IHttpClientFactory httpClientFactory,
            IPublishEndpoint publishEndpoint,
            ILogger<OrderService> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
            _publishEndpoint = publishEndpoint;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<OrderResponse> CreateOrderAsync(string userId, CreateOrderRequest request)
        {
            // Validar que hay items
            if (request.Items == null || !request.Items.Any())
            {
                throw new InvalidOperationException("La orden debe tener al menos un producto.");
            }

            var httpClient = _httpClientFactory.CreateClient("ProductsApi");

            var authHeader = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].FirstOrDefault();
            if (!string.IsNullOrEmpty(authHeader))
            {
                if (authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    var token = authHeader.Substring("Bearer ".Length).Trim();
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }
                else
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authHeader);
                }
            }

            var orderItems = new List<OrderItem>();
            decimal totalAmount = 0;

            // Validar cada producto y obtener información
            foreach (var item in request.Items)
            {
                // Llamar al API de productos para obtener detalles y validar stock
                var productResponse = await httpClient.GetAsync($"/api/v1/Products/{item.ProductId}");

                if (!productResponse.IsSuccessStatusCode)
                {
                    throw new InvalidOperationException($"Producto con ID {item.ProductId} no encontrado.");
                }

                var product = await productResponse.Content.ReadFromJsonAsync<ProductDto>();

                if (product == null)
                {
                    throw new InvalidOperationException($"Error al obtener información del producto {item.ProductId}.");
                }

                if (product.Stock < item.Quantity)
                {
                    throw new InvalidOperationException($"Stock insuficiente para el producto '{product.Name}'. Disponible: {product.Stock}, Solicitado: {item.Quantity}");
                }

                // Crear OrderItem con snapshot del producto
                var orderItem = new OrderItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Price = product.Price,
                    Quantity = item.Quantity
                };

                orderItems.Add(orderItem);
                totalAmount += orderItem.Subtotal;
            }

            // Crear la orden
            var order = new Order
            {
                UserId = userId,
                TotalAmount = totalAmount,
                ShippingAddress = request.ShippingAddress,
                Status = OrderStatus.Pending,
                OrderItems = orderItems,
                CreatedAt = DateTime.UtcNow
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Orden creada: {OrderId} para usuario {UserId} con total {TotalAmount:C}", 
                order.Id, userId, totalAmount);

            // TODO: Publicar evento OrderCreatedEvent a RabbitMQ (siguiente paso)

            return MapToResponse(order);
        }

        public async Task<OrderResponse?> GetOrderByIdAsync(string orderId, string userId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);

            if (order == null)
            {
                return null;
            }

            return MapToResponse(order);
        }

        public async Task<IEnumerable<OrderResponse>> GetUserOrdersAsync(string userId)
        {
            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            return orders.Select(MapToResponse);
        }

        public async Task<IEnumerable<OrderResponse>> GetAllOrdersAsync()
        {
            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            return orders.Select(MapToResponse);
        }

        public async Task<bool> UpdateOrderStatusAsync(string orderId, OrderStatus newStatus)
        {
            var order = await _context.Orders.FindAsync(orderId);

            if (order == null)
            {
                return false;
            }

            var oldStatus = order.Status;
            order.Status = newStatus;
            order.UpdatedAt = DateTime.UtcNow;

            _context.Orders.Update(order);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Estado de orden {OrderId} cambiado de {OldStatus} a {NewStatus}", 
                orderId, oldStatus, newStatus);

            // TODO: Publicar evento OrderStatusChangedEvent (siguiente paso)

            return true;
        }

        public async Task<bool> CancelOrderAsync(string orderId, string userId)
        {
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);

            if (order == null)
            {
                return false;
            }

            // Solo se pueden cancelar órdenes en estado Pending o Processing
            if (order.Status != OrderStatus.Pending && order.Status != OrderStatus.Processing)
            {
                throw new InvalidOperationException($"No se puede cancelar una orden en estado {order.Status}");
            }

            order.Status = OrderStatus.Cancelled;
            order.UpdatedAt = DateTime.UtcNow;

            _context.Orders.Update(order);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Orden {OrderId} cancelada por usuario {UserId}", orderId, userId);

            return true;
        }

        private static OrderResponse MapToResponse(Order order)
        {
            return new OrderResponse
            {
                Id = order.Id,
                UserId = order.UserId,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                ShippingAddress = order.ShippingAddress,
                CreatedAt = order.CreatedAt,
                UpdatedAt = order.UpdatedAt,
                Items = order.OrderItems.Select(oi => new OrderItemResponse
                {
                    Id = oi.Id,
                    ProductId = oi.ProductId,
                    ProductName = oi.ProductName,
                    Price = oi.Price,
                    Quantity = oi.Quantity,
                    Subtotal = oi.Subtotal
                }).ToList()
            };
        }

        // DTO interno para deserializar la respuesta de Products API
        private class ProductDto
        {
            public string Id { get; set; } = string.Empty;
            public string Name { get; set; } = string.Empty;
            public decimal Price { get; set; }
            public int Stock { get; set; }
        }
    }
}