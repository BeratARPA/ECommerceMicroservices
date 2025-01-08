using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace ECommerce.Shared.Messaging
{
    public class RabbitMQEventBus : IEventBus
    {
        private readonly IConnection _connection;
        private readonly IChannel _channel;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public RabbitMQEventBus(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;

            var factory = new ConnectionFactory { 
                HostName = "rabbitmq",
                Port = 5672
            };
            _connection = factory.CreateConnectionAsync().Result;
            _channel = _connection.CreateChannelAsync().Result;
        }

        public void Publish<T>(T @event, string exchangeName, string routingKey) where T : class
        {
            _channel.ExchangeDeclareAsync(exchangeName, ExchangeType.Direct);

            var message = JsonSerializer.Serialize(@event);
            var body = Encoding.UTF8.GetBytes(message);

            var properties = new BasicProperties
            {
                Persistent = true
            };

            _channel.BasicPublishAsync(
                exchange: exchangeName,
                mandatory: false,
                routingKey: routingKey,
                basicProperties: properties,
                body: body);
        }

        public void Subscribe<T, TH>(string exchangeName, string queueName)
            where T : class
            where TH : IEventHandler<T>
        {
            _channel.ExchangeDeclareAsync(exchangeName, ExchangeType.Direct);
            _channel.QueueDeclareAsync(queueName, false, false, false, null);
            _channel.QueueBindAsync(queueName, exchangeName, queueName);

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var @event = JsonSerializer.Deserialize<T>(message);

                using var scope = _serviceScopeFactory.CreateScope();
                var handler = scope.ServiceProvider.GetRequiredService<IEventHandler<T>>();
                await handler.HandleAsync(@event);
            };

            _channel.BasicConsumeAsync(queue: queueName,
                                autoAck: true,
                                consumer: consumer);
        }
    }
}
