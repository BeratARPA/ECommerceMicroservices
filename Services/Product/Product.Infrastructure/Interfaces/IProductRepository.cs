namespace Product.Infrastructure.Interfaces
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product.Domain.Entities.Product>> GetAllAsync();
        Task<Product.Domain.Entities.Product?> GetByIdAsync(int id);
        Task<Product.Domain.Entities.Product?> GetByNameAsync(string name);
        Task AddAsync(Product.Domain.Entities.Product product);
        Task UpdateAsync(Product.Domain.Entities.Product product);
        Task DeleteAsync(Product.Domain.Entities.Product product);
    }
}
