using ECommerce.Shared.DTOs;
using Product.Domain.DTOs;
using Product.Infrastructure.Interfaces;

namespace Product.Infrastructure.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<IEnumerable<Product.Domain.Entities.Product>> GetAllProductsAsync()
        {
            return await _productRepository.GetAllAsync();
        }

        public async Task<Product.Domain.Entities.Product?> GetProductByIdAsync(int id)
        {
            return await _productRepository.GetByIdAsync(id);
        }

        public async Task<ResponseBaseDTO> CreateProductAsync(CreateProductDTO productDto)
        {
            var existingProduct = await _productRepository.GetByNameAsync(productDto.Name);
            if (existingProduct != null)
                return new ResponseBaseDTO { Success = false, Message = "Ürün adı benzersiz olmalı." };

            var product = new Product.Domain.Entities.Product
            {
                Name = productDto.Name,
                Stock = productDto.Stock,
                Price = productDto.Price
            };

            await _productRepository.AddAsync(product);
            return new ResponseBaseDTO { Success = true, Message = "Ürün başarıyla oluşturuldu." };
        }

        public async Task<ResponseBaseDTO> UpdateStockAsync(int productId, int stock)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null)
                return new ResponseBaseDTO { Success = false, Message = "Ürün bulunamadı." };

            if (stock < 0)
                return new ResponseBaseDTO { Success = false, Message = "Stok miktarı sıfırdan küçük olamaz." };

            product.Stock = stock;
            await _productRepository.UpdateAsync(product);

            return new ResponseBaseDTO { Success = true, Message = "Stok güncellendi." };
        }

        public async Task<ResponseBaseDTO> DeleteProductAsync(int productId)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null)
                return new ResponseBaseDTO { Success = false, Message = "Ürün bulunamadı." };

            await _productRepository.DeleteAsync(product);
            return new ResponseBaseDTO { Success = true, Message = "Ürün silindi." };
        }
    }
}
