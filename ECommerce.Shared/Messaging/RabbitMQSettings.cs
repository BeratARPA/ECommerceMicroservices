namespace ECommerce.Shared.Messaging
{
    public class RabbitMQSettings
    {
        public const string OrderCreatedQueueName = "order-created-queue";
        public const string OrderCancelledQueueName = "order-cancelled-queue";
        public const string HostName = "rabbitmq";
        public const string ExchangeName = "ecommerce-exchange";
    }
}
