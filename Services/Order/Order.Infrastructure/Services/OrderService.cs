using ECommerce.Shared.DTOs;
using Microsoft.Extensions.Caching.Distributed;
using Order.Domain.DTOs;
using Order.Infrastructure.Interfaces;
using System.Text.Json;

namespace Order.Infrastructure.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IDistributedCache _cache;

        public OrderService(IOrderRepository orderRepository, IDistributedCache cache)
        {
            _orderRepository = orderRepository;
            _cache = cache;
        }

        public async Task<IEnumerable<Order.Domain.Entities.Order>> GetAllOrdersAsync()
        {
            string cacheKey = "all_orders";
            var cachedData = await _cache.GetStringAsync(cacheKey);

            if (!string.IsNullOrEmpty(cachedData))
            {
                return JsonSerializer.Deserialize<IEnumerable<Order.Domain.Entities.Order>>(cachedData);
            }

            var orders = await _orderRepository.GetAllAsync();

            // Cache verilerini ayarla (10 dakika geçerli)
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
            };
            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(orders), options);

            return orders;
        }

        public async Task<ResponseBaseDTO> CreateOrderAsync(CreateOrderDTO orderDto)
        {
            // TODO: Stok kontrolü Product API ile yapılmalı.

            var order = new Order.Domain.Entities.Order
            {
                ProductId = orderDto.ProductId,
                Quantity = orderDto.Quantity,
                TotalPrice = orderDto.Quantity * 100, // Örnek fiyat
                CreatedAt = DateTime.UtcNow,
                IsCancelled = false
            };

            await _orderRepository.AddAsync(order);

            return new ResponseBaseDTO { Success = true, Message = "Sipariş başarıyla oluşturuldu." };
        }

        public async Task<ResponseBaseDTO> CancelOrderAsync(int orderId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
                return new ResponseBaseDTO { Success = false, Message = "Sipariş bulunamadı." };

            order.IsCancelled = true;
            await _orderRepository.UpdateAsync(order);

            return new ResponseBaseDTO { Success = true, Message = "Sipariş iptal edildi." };
        }

        public async Task<Order.Domain.Entities.Order?> GetOrderByIdAsync(int id)
        {
            string cacheKey = $"order_{id}";
            var cachedData = await _cache.GetStringAsync(cacheKey);

            if (!string.IsNullOrEmpty(cachedData))
            {
                return JsonSerializer.Deserialize<Order.Domain.Entities.Order>(cachedData);
            }

            var order = await _orderRepository.GetByIdAsync(id);

            if (order != null)
            {
                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                };
                await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(order), options);
            }

            return order;
        }       
    }
}
