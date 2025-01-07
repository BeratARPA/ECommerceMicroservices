using Microsoft.EntityFrameworkCore;
using Product.Infrastructure.Data;
using Product.Infrastructure.Interfaces;

namespace Product.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ProductDbContext _context;

        public ProductRepository(ProductDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product.Domain.Entities.Product>> GetAllAsync()
        {
            return await _context.Products.ToListAsync();
        }

        public async Task<Product.Domain.Entities.Product?> GetByIdAsync(int id)
        {
            return await _context.Products.FindAsync(id);
        }

        public async Task<Product.Domain.Entities.Product?> GetByNameAsync(string name)
        {
            return await _context.Products.FirstOrDefaultAsync(p => p.Name == name);
        }

        public async Task AddAsync(Product.Domain.Entities.Product product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Product.Domain.Entities.Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Product.Domain.Entities.Product product)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }
    }
}
