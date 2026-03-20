using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shipping.Api.Data;
using Shipping.Api.Models;

namespace Shipping.Api.Services;

public class ShippingMessageReceiver : IHostedService {
    private readonly ILogger<ShippingMessageReceiver> _logger;
    private readonly IConnection _connection;
    private IChannel _channel;
    private readonly IServiceScopeFactory _scopeFactory;

    public ShippingMessageReceiver(IConnection connection, ILogger<ShippingMessageReceiver> logger, IServiceScopeFactory scopeFactory) {
        _connection = connection;
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    public async Task StartAsync(CancellationToken cancellationToken) {
        _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);
        await _channel.QueueDeclareAsync(queue: "shipping_queue",
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null, 
            cancellationToken: cancellationToken);
        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (model, ea) => {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var order = JsonSerializer.Deserialize<Order>(message);
            _logger.LogInformation("Received message: {Message}", message);
            using var scope = _scopeFactory.CreateScope();
            var shippingContext = scope.ServiceProvider.GetRequiredService<ShippingContext>();
            var shippingOrder = new ShippingOrder(
                shippingId: Guid.NewGuid(),
                orderId: order?.Id,
                shippingAddress: order?.ShippingAddress ?? string.Empty,
                status: ShippingStatus.Pending
            );
            await shippingContext.ShippingOrders.AddAsync(shippingOrder, cancellationToken);
            await shippingContext.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Creating shipping order for OrderId: {OrderId}", shippingOrder.OrderId);
            await Task.CompletedTask;
        };
        await _channel.BasicConsumeAsync(queue: "shipping_queue",
            autoAck: true,
            consumer: consumer, 
            cancellationToken: cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken) {
        await _channel.CloseAsync(cancellationToken: cancellationToken);
        _logger.LogInformation("Stopping ShippingMessageReceiver...");
    }
}