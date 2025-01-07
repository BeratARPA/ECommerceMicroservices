using Microsoft.EntityFrameworkCore;
using Order.Infrastructure.Data;
using Order.Infrastructure.Interfaces;

namespace Order.Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly OrderDbContext _context;

        public OrderRepository(OrderDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Order.Domain.Entities.Order order)
        {
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Order.Domain.Entities.Order?>> GetAllAsync()
        {
            return await _context.Orders.ToListAsync(); ;
        }

        public async Task<Order.Domain.Entities.Order?> GetByIdAsync(int id)
        {
            return await _context.Orders.FindAsync(id);
        }

        public async Task UpdateAsync(Order.Domain.Entities.Order order)
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
        }
    }
}
