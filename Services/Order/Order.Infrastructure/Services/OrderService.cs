using ECommerce.Shared.DTOs;
using ECommerce.Shared.Messaging;
using Microsoft.Extensions.Caching.Distributed;
using Order.Domain.DTOs;
using Order.Infrastructure.Interfaces;
using System.Text.Json;
using ECommerce.Shared.Events;

namespace Order.Infrastructure.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductHttpClient _productClient;
        private readonly IDistributedCache _cache;
        private readonly IEventBus _eventBus;

        public OrderService(
            IOrderRepository orderRepository, 
            IProductHttpClient productClient,
            IDistributedCache cache,
            IEventBus eventBus)
        {
            _orderRepository = orderRepository;
            _productClient = productClient;
            _cache = cache;
            _eventBus = eventBus;
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
            var product = await _productClient.GetProductAsync(orderDto.ProductId);
            if (product == null)
                return new ResponseBaseDTO { Success = false, Message = "Ürün bulunamadı." };

            if (product.Stock < orderDto.Quantity)
                return new ResponseBaseDTO { Success = false, Message = "Yetersiz stok." };

            var totalPrice = product.Price * orderDto.Quantity;

            var order = new Order.Domain.Entities.Order
            {
                ProductId = orderDto.ProductId,
                Quantity = orderDto.Quantity,
                TotalPrice = totalPrice,
                CreatedAt = DateTime.UtcNow,
                IsCancelled = false
            };

            await _orderRepository.AddAsync(order);

            // Event publish
            _eventBus.Publish(
                new OrderCreatedEvent 
                { 
                    OrderId = order.Id,
                    ProductId = order.ProductId,
                    Quantity = order.Quantity,
                    CreatedAt = order.CreatedAt
                },
                RabbitMQSettings.ExchangeName,
                RabbitMQSettings.OrderCreatedQueueName
            );

            // Cache'i temizle
            string cacheKey = $"all_orders";
            await _cache.RemoveAsync(cacheKey);

            return new ResponseBaseDTO { Success = true, Message = "Sipariş başarıyla oluşturuldu." };
        }

        public async Task<ResponseBaseDTO> CancelOrderAsync(int orderId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
                return new ResponseBaseDTO { Success = false, Message = "Sipariş bulunamadı." };

            if (order.IsCancelled)
                return new ResponseBaseDTO { Success = false, Message = "Sipariş zaten iptal edilmiş." };

            order.IsCancelled = true;
            await _orderRepository.UpdateAsync(order);

            // Event publish
            _eventBus.Publish(
                new OrderCancelledEvent 
                { 
                    OrderId = order.Id,
                    ProductId = order.ProductId,
                    ReturnQuantity = order.Quantity,
                    CancelledAt = DateTime.UtcNow
                },
                RabbitMQSettings.ExchangeName,
                RabbitMQSettings.OrderCancelledQueueName
            );

            // Cache'i temizle          
            string cacheAllOrdersKey = "all_orders";
            await _cache.RemoveAsync(cacheAllOrdersKey);

            string cacheOrderKey = $"product_{orderId}";
            await _cache.RemoveAsync(cacheOrderKey);

            return new ResponseBaseDTO { Success = true, Message = "Sipariş başarıyla iptal edildi." };
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
