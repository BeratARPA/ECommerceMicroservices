using ECommerce.Shared.Messaging;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using Order.Domain.DTOs;
using Order.Infrastructure.Interfaces;
using Order.Infrastructure.Services;

namespace Order.Tests
{
    public class OrderServiceTests
    {
        private readonly Mock<IOrderRepository> _mockOrderRepository;
        private readonly Mock<IProductHttpClient> _mockProductClient;
        private readonly Mock<IDistributedCache> _mockCache;
        private readonly Mock<IEventBus> _mockEventBus;
        private readonly OrderService _orderService;

        public OrderServiceTests()
        {
            _mockOrderRepository = new Mock<IOrderRepository>();
            _mockProductClient = new Mock<IProductHttpClient>();
            _mockCache = new Mock<IDistributedCache>();
            _mockEventBus = new Mock<IEventBus>();

            _orderService = new OrderService(
                _mockOrderRepository.Object,
                _mockProductClient.Object,
                _mockCache.Object,
                _mockEventBus.Object
            );
        }

        [Fact]
        public async Task CreateOrderAsync_WhenProductNotFound_ReturnsFailure()
        {
            // Arrange
            var orderDto = new CreateOrderDTO { ProductId = 1, Quantity = 1 };
            _mockProductClient.Setup(x => x.GetProductAsync(It.IsAny<int>()))
                .ReturnsAsync((ProductDTO)null);

            // Act
            var result = await _orderService.CreateOrderAsync(orderDto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Ürün bulunamadı.", result.Message);
        }

        [Fact]
        public async Task CreateOrderAsync_WhenInsufficientStock_ReturnsFailure()
        {
            // Arrange
            var orderDto = new CreateOrderDTO { ProductId = 1, Quantity = 10 };
            var product = new ProductDTO { Id = 1, Stock = 5, Price = 100 };

            _mockProductClient.Setup(x => x.GetProductAsync(It.IsAny<int>()))
                .ReturnsAsync(product);

            // Act
            var result = await _orderService.CreateOrderAsync(orderDto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Yetersiz stok.", result.Message);
        }

        [Fact]
        public async Task CreateOrderAsync_WhenValidOrder_ReturnsSuccess()
        {
            // Arrange
            var orderDto = new CreateOrderDTO { ProductId = 1, Quantity = 2 };
            var product = new ProductDTO { Id = 1, Stock = 5, Price = 100 };

            _mockProductClient.Setup(x => x.GetProductAsync(It.IsAny<int>()))
                .ReturnsAsync(product);

            // Act
            var result = await _orderService.CreateOrderAsync(orderDto);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Sipariş başarıyla oluşturuldu.", result.Message);

            _mockOrderRepository.Verify(x => x.AddAsync(It.IsAny<Domain.Entities.Order>()), Times.Once);
            _mockEventBus.Verify(x => x.Publish(It.IsAny<object>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            _mockCache.Verify(x => x.RemoveAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CancelOrderAsync_WhenOrderNotFound_ReturnsFailure()
        {
            // Arrange
            _mockOrderRepository.Setup(x => x.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Domain.Entities.Order)null);

            // Act
            var result = await _orderService.CancelOrderAsync(1);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Sipariş bulunamadı.", result.Message);
        }

        [Fact]
        public async Task CancelOrderAsync_WhenOrderAlreadyCancelled_ReturnsFailure()
        {
            // Arrange
            var order = new Domain.Entities.Order { Id = 1, IsCancelled = true };
            _mockOrderRepository.Setup(x => x.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(order);

            // Act
            var result = await _orderService.CancelOrderAsync(1);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Sipariş zaten iptal edilmiş.", result.Message);
        }
    }
}
