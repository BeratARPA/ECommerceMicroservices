namespace Order.Infrastructure.Interfaces
{
    public interface IOrderRepository
    {
        Task AddAsync(Order.Domain.Entities.Order order);
        Task<IEnumerable<Order.Domain.Entities.Order?>> GetAllAsync();
        Task<Order.Domain.Entities.Order?> GetByIdAsync(int id);
        Task UpdateAsync(Order.Domain.Entities.Order order);
    }
}
