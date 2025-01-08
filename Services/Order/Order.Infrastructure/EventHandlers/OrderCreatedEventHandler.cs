using ECommerce.Shared.Events;
using ECommerce.Shared.Messaging;
using Order.Infrastructure.Interfaces;

namespace Order.Infrastructure.EventHandlers
{
    public class OrderCreatedEventHandler : IEventHandler<OrderCreatedEvent>
    {
        private readonly IProductHttpClient _productClient;

        public OrderCreatedEventHandler(IProductHttpClient productClient)
        {
            _productClient = productClient;
        }

        public async Task HandleAsync(OrderCreatedEvent @event)
        {
            var product = await _productClient.GetProductAsync(@event.ProductId);
            if (product != null)
            {
                var newStock = product.Stock - @event.Quantity;
                await _productClient.UpdateProductStockAsync(@event.ProductId, newStock);
            }
        }
    }
}
