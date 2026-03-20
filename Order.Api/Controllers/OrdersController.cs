using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Orders.Api.data;
using Orders.Api.Models;
using Orders.Api.Services;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Orders.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase {
    private readonly OrdersContext _context;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(
        OrdersContext context,
        ILogger<OrdersController> logger
    ) {
        _context = context;
        _logger = logger;
    }

    // POST: api/Orders
    [HttpPost]
    public async Task<ActionResult<Order>> PostOrder(Order order) {
        order.Id = Guid.NewGuid();
        string json = JsonSerializer.Serialize(order);
        OutboxMessage outboxMessage = new OutboxMessage(
            id: order.Id,
            type: string.Empty,
            payload: json,
            createdAtUtc: DateTime.UtcNow,
            processedAtUtc: null
        );
        await _context.Orders.AddAsync(order);
        await _context.OutboxMessages.AddAsync(outboxMessage);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Gemmer ordre i databasens tabeller");
        return CreatedAtAction("PostOrder", new { id = order.Id }, order);
    }

    // DELETE: api/Orders/5
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteOrder(Guid id) {
        var order = await _context.Orders.FindAsync(id);
        if (order is null) {
            return NotFound();
        }

        _context.Orders.Remove(order);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool OrderExists(Guid id) {
        return _context.Orders.Any(e => e.Id == id);
    }
}