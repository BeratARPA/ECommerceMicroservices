using ECommerce.Shared.DTOs;
using Product.Domain.DTOs;

namespace Product.Infrastructure.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<Product.Domain.Entities.Product>> GetAllProductsAsync();
        Task<Product.Domain.Entities.Product?> GetProductByIdAsync(int id);
        Task<ResponseBaseDTO> CreateProductAsync(CreateProductDTO productDto);
        Task<ResponseBaseDTO> UpdateStockAsync(int productId, int stock);
        Task<ResponseBaseDTO> DeleteProductAsync(int productId);
    }
}
