using ECommerce.Shared.DTOs;
using Microsoft.Extensions.Caching.Distributed;
using Product.Domain.DTOs;
using Product.Infrastructure.Interfaces;
using System.Text.Json;

namespace Product.Infrastructure.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IDistributedCache _cache;

        public ProductService(IProductRepository productRepository, IDistributedCache cache)
        {
            _productRepository = productRepository;
            _cache = cache;
        }

        public async Task<IEnumerable<Product.Domain.Entities.Product>> GetAllProductsAsync()
        {
            string cacheKey = "all_products";
            var cachedData = await _cache.GetStringAsync(cacheKey);

            if (!string.IsNullOrEmpty(cachedData))
            {
                return JsonSerializer.Deserialize<IEnumerable<Product.Domain.Entities.Product>>(cachedData);
            }

            var products = await _productRepository.GetAllAsync();

            // Cache verilerini ayarla (10 dakika geçerli)
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
            };
            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(products), options);

            return products;
        }

        public async Task<Product.Domain.Entities.Product?> GetProductByIdAsync(int id)
        {
            string cacheKey = $"product_{id}";
            var cachedData = await _cache.GetStringAsync(cacheKey);

            if (!string.IsNullOrEmpty(cachedData))
            {
                return JsonSerializer.Deserialize<Product.Domain.Entities.Product>(cachedData);
            }

            var product = await _productRepository.GetByIdAsync(id);

            if (product != null)
            {
                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                };
                await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(product), options);
            }

            return product;
        }

        public async Task<ResponseBaseDTO> CreateProductAsync(CreateProductDTO productDto)
        {
            var existingProduct = await _productRepository.GetByNameAsync(productDto.Name);
            if (existingProduct != null)
                return new ResponseBaseDTO { Success = false, Message = "Bu isimde bir ürün zaten mevcut." };

            if (productDto.Stock < 0)
                return new ResponseBaseDTO { Success = false, Message = "Stok miktarı sıfırdan küçük olamaz." };

            var product = new Product.Domain.Entities.Product
            {
                Name = productDto.Name,
                Stock = productDto.Stock,
                Price = productDto.Price
            };

            await _productRepository.AddAsync(product);

            // Cache'i temizle
            string cacheKey = $"all_products";
            await _cache.RemoveAsync(cacheKey);

            return new ResponseBaseDTO { Success = true, Message = "Ürün başarıyla oluşturuldu." };
        }

        // Diğer metotlar (CreateProductAsync, UpdateStockAsync, DeleteProductAsync) cache temizlemeyi içerebilir.
        public async Task<ResponseBaseDTO> UpdateStockAsync(int productId, int stock)
        {
            var response = await _productRepository.GetByIdAsync(productId);
            if (response == null)
                return new ResponseBaseDTO { Success = false, Message = "Ürün bulunamadı." };

            response.Stock = stock;
            await _productRepository.UpdateAsync(response);

            // Cache'i temizle
            string cacheAllProductsKey = "all_products";
            await _cache.RemoveAsync(cacheAllProductsKey);

            string cacheProductKey = $"product_{productId}";
            await _cache.RemoveAsync(cacheProductKey);

            return new ResponseBaseDTO { Success = true, Message = "Stok güncellendi." };
        }

        public async Task<ResponseBaseDTO> DeleteProductAsync(int productId)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null)
                return new ResponseBaseDTO { Success = false, Message = "Ürün bulunamadı." };

            await _productRepository.DeleteAsync(product);

            // Cache'i temizle
            string cacheAllProductsKey = "all_products";
            await _cache.RemoveAsync(cacheAllProductsKey);

            string cacheProductKey = $"product_{productId}";
            await _cache.RemoveAsync(cacheProductKey);

            return new ResponseBaseDTO { Success = true, Message = "Ürün silindi." };
        }
    }
}
