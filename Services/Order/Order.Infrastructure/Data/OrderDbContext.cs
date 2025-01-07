using Microsoft.EntityFrameworkCore;

namespace Order.Infrastructure.Data
{
    public class OrderDbContext : DbContext
    {
        public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options) { }

        public DbSet<Order.Domain.Entities.Order> Orders { get; set; }
    }
}
