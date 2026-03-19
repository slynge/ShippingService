using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shipping.Api.Models;

namespace Shipping.Api.Services
{
    public class ShippingMessageReceiver : IHostedService
    {
        private readonly ILogger<ShippingMessageReceiver> _logger;
        private readonly IConnection _connection;
        private readonly IServiceScopeFactory _scopeFactory;

        public ShippingMessageReceiver(IConnection connection, ILogger<ShippingMessageReceiver> logger, IServiceScopeFactory scopeFactory)
        {
            _connection = connection;
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var channel = await _connection.CreateChannelAsync();
            await channel.QueueDeclareAsync(queue: "shipping_queue",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);
            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = System.Text.Encoding.UTF8.GetString(body);
                var order = System.Text.Json.JsonSerializer.Deserialize<Order>(message);
                _logger.LogInformation("Received message: {Message}", message);
                using var scope = _scopeFactory.CreateScope();
                var shippingContext = scope.ServiceProvider.GetRequiredService<Data.ShippingContext>();
                var shippingOrder = new ShippingOrder
                {
                    ShippingId = Guid.NewGuid(),
                    OrderId = order?.Id,
                    ShippingAdress = order?.ShippingAddress??string.Empty,
                    Status = ShippingStatus.Pending
                };
                shippingContext.ShippingOrders.Add(shippingOrder);
                await shippingContext.SaveChangesAsync();
                _logger.LogInformation("Creating shipping order for OrderId: {OrderId}", shippingOrder.OrderId);
                await Task.CompletedTask;
            };
            await channel.BasicConsumeAsync(queue: "shipping_queue",
                                 autoAck: true,
                                 consumer: consumer);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping ShippingMessageReceiver...");
            return Task.CompletedTask;
        }
    }
}
