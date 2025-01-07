using Microsoft.EntityFrameworkCore;

namespace Product.Infrastructure.Data
{
    public class ProductDbContext : DbContext
    {
        public ProductDbContext(DbContextOptions<ProductDbContext> options) : base(options) { }

        public DbSet<Product.Domain.Entities.Product> Products { get; set; }
    }
}
