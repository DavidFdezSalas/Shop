using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shop.APIOrders.Dto;
using Shop.APIOrders.Models;
using Shop.APIOrders.Services;
using System.Security.Claims;

namespace Shop.APIOrders.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(IOrderService orderService, ILogger<OrdersController> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }

        // POST: api/v1/Orders
        [HttpPost]
        public async Task<ActionResult<OrderResponse>> CreateOrder([FromBody] CreateOrderRequest request)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { message = "Usuario no autenticado." });
                }

                var order = await _orderService.CreateOrderAsync(userId, request);

                return CreatedAtAction(
                    nameof(GetOrderById),
                    new { id = order.Id },
                    order);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Error al crear orden: {Message}", ex.Message);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al crear orden");
                return StatusCode(500, new { message = "Error al procesar la orden." });
            }
        }

        // GET: api/v1/Orders/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderResponse>> GetOrderById(string id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "Usuario no autenticado." });
            }

            var order = await _orderService.GetOrderByIdAsync(id, userId);

            if (order == null)
            {
                return NotFound(new { message = $"Orden con ID {id} no encontrada." });
            }

            return Ok(order);
        }

        // GET: api/v1/Orders/my-orders
        [HttpGet("my-orders")]
        public async Task<ActionResult<IEnumerable<OrderResponse>>> GetMyOrders()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "Usuario no autenticado." });
            }

            var orders = await _orderService.GetUserOrdersAsync(userId);
            return Ok(orders);
        }

        // GET: api/v1/Orders (Solo Admin)
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<OrderResponse>>> GetAllOrders()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return Ok(orders);
        }

        // PATCH: api/v1/Orders/{id}/status (Solo Admin)
        [HttpPatch("{id}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateOrderStatus(string id, [FromBody] UpdateOrderStatusRequest request)
        {
            var result = await _orderService.UpdateOrderStatusAsync(id, request.Status);

            if (!result)
            {
                return NotFound(new { message = $"Orden con ID {id} no encontrada." });
            }

            return NoContent();
        }

        // POST: api/v1/Orders/{id}/cancel
        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> CancelOrder(string id)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { message = "Usuario no autenticado." });
                }

                var result = await _orderService.CancelOrderAsync(id, userId);

                if (!result)
                {
                    return NotFound(new { message = $"Orden con ID {id} no encontrada." });
                }

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Error al cancelar orden {OrderId}: {Message}", id, ex.Message);
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
