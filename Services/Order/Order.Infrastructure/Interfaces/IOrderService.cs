using ECommerce.Shared.DTOs;
using Order.Domain.DTOs;

namespace Order.Infrastructure.Interfaces
{
    public interface IOrderService
    {
        Task<ResponseBaseDTO> CreateOrderAsync(CreateOrderDTO orderDto);
        Task<ResponseBaseDTO> CancelOrderAsync(int orderId);
        Task<IEnumerable<Order.Domain.Entities.Order>> GetAllOrdersAsync();
        Task<Order.Domain.Entities.Order?> GetOrderByIdAsync(int id);
    }
}
