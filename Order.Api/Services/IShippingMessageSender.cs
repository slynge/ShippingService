using Orders.Api.Models;

namespace Orders.Api.Services
{
    public interface IShippingMessageSender
    {
        Task SendMessageAsync(Order order);
    }
}