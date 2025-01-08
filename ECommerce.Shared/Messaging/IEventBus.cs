namespace ECommerce.Shared.Messaging
{
    public interface IEventBus
    {
        void Publish<T>(T @event, string exchangeName, string routingKey) where T : class;
        void Subscribe<T, TH>(string exchangeName, string queueName)
            where T : class
            where TH : IEventHandler<T>;
    }
}
