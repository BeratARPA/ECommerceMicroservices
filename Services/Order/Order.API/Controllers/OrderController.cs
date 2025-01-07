using Microsoft.AspNetCore.Mvc;
using Order.Domain.DTOs;
using Order.Infrastructure.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Order.API.Controllers
{
    public class OrderController : BaseController
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<IActionResult> GetGetAllOrders()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            if (orders == null)
                return NotFound();

            return Ok(orders);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDTO orderDto)
        {
            var response = await _orderService.CreateOrderAsync(orderDto);
            if (!response.Success)
                return BadRequest(response.Message);

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
                return NotFound();

            return Ok(order);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> CancelOrder(int id)
        {
            var response = await _orderService.CancelOrderAsync(id);
            if (!response.Success)
                return BadRequest(response.Message);

            return Ok(response);
        }
    }
}
