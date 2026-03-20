using System.Text;
using System.Text.Json;
using Orders.Api.Models;
using RabbitMQ.Client;

namespace Orders.Api.Services;

public class ShippingMessageSender : IShippingMessageSender {
    private readonly IConnection _connection;
    private readonly ILogger<ShippingMessageSender> _logger;

    public ShippingMessageSender(IConnection connection, ILogger<ShippingMessageSender> logger) {
        _connection = connection;
        _logger = logger;
    }

    public async Task SendMessageAsync(string jsonOrder) {
        try {
            using var channel = await _connection.CreateChannelAsync();
            await channel.QueueDeclareAsync(queue: "shipping_queue",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);
            var body = Encoding.UTF8.GetBytes(jsonOrder);
            await channel.BasicPublishAsync(exchange: "",
                routingKey: "shipping_queue",
                body: body);
            _logger.LogInformation("Sent message to RabbitMQ: {Message}", jsonOrder);
        }
        catch (Exception exception) {
            _logger.LogError(exception, "Failed to send message to RabbitMQ");
        }
    }
}