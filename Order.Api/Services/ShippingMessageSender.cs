using Orders.Api.Models;
using RabbitMQ.Client;

namespace Orders.Api.Services
{
    public class ShippingMessageSender : IShippingMessageSender
    {
        private readonly IConnection _connection;

        public ShippingMessageSender(IConnection connection)
        {
            _connection = connection;
        }

        public async Task SendMessageAsync(Order order)
        {
            using var channel = await _connection.CreateChannelAsync();
            await channel.QueueDeclareAsync(queue: "shipping_queue",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);
            var message = System.Text.Json.JsonSerializer.Serialize(order);
            var bytes = System.Text.Encoding.UTF8.GetBytes(message);
            await channel.BasicPublishAsync(exchange: "",
                                 routingKey: "shipping_queue",
                                 body: bytes);
        }
    }
}
