using Order.Domain.DTOs;

namespace Order.Infrastructure.Interfaces
{
    public interface IProductHttpClient
    {
        Task<ProductDTO?> GetProductAsync(int productId);
        Task<bool> UpdateProductStockAsync(int productId, int newStock);
    }
}
