using Orders.Api.data;
using Orders.Api.Models;

namespace Orders.Api.Services;

public class Worker : BackgroundService {
	private readonly IServiceProvider _serviceProvider;
	private readonly ILogger<Worker> _logger;

	public Worker(IServiceProvider serviceProvider, ILogger<Worker> logger) {
		_serviceProvider = serviceProvider;
		_logger = logger;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
		while (!stoppingToken.IsCancellationRequested) {
			await Task.Delay(5000, stoppingToken);
			await using (var scope = _serviceProvider.CreateAsyncScope()) {
				var ordersDb = scope.ServiceProvider.GetRequiredService<OrdersContext>();
				var shippingMessageSender = scope.ServiceProvider.GetRequiredService<IShippingMessageSender>();
				var outboxMessages = ordersDb.OutboxMessages.ToList();
				outboxMessages = outboxMessages.FindAll(message => message.ProcessedAtUTC is null);
				outboxMessages.Sort((message1, message2) => message1.CreatedAtUTC.CompareTo(message2.CreatedAtUTC));
				try {
					OutboxMessage outboxMessage = outboxMessages[0];
					await shippingMessageSender.SendMessageAsync(outboxMessage.Payload);
					outboxMessage.ProcessedAtUTC = DateTime.UtcNow;
					ordersDb.OutboxMessages.Update(outboxMessage);
					await ordersDb.SaveChangesAsync(stoppingToken);
				}
				catch (Exception exception) {
					_logger.LogError(exception.Message);
				}
			}
		}
	}
}