using ECommerce.Shared.Events;
using ECommerce.Shared.Messaging;
using Order.Infrastructure.Interfaces;

namespace Order.Infrastructure.EventHandlers
{
    public class OrderCancelledEventHandler : IEventHandler<OrderCancelledEvent>
    {
        private readonly IProductHttpClient _productClient;

        public OrderCancelledEventHandler(IProductHttpClient productClient)
        {
            _productClient = productClient;
        }

        public async Task HandleAsync(OrderCancelledEvent @event)
        {
            var product = await _productClient.GetProductAsync(@event.ProductId);
            if (product != null)
            {
                // Stok iadesi
                var newStock = product.Stock + @event.ReturnQuantity;
                await _productClient.UpdateProductStockAsync(@event.ProductId, newStock);
            }
        }
    }
}
