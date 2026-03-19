using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shipping.Api.Data;
using Shipping.Api.Models;

namespace Shipping.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShippingController : ControllerBase
    {
        private readonly ShippingContext _context;

        public ShippingController(ShippingContext context)
        {
            _context = context;
        }

        // GET: api/Shipping
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ShippingOrder>>> GetShippingOrders()
        {
            return await _context.ShippingOrders.ToListAsync();
        }

        // GET: api/Shipping/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ShippingOrder>> GetShippingOrder(Guid id)
        {
            var shippingOrder = await _context.ShippingOrders.FindAsync(id);

            if (shippingOrder == null)
            {
                return NotFound();
            }

            return shippingOrder;
        }


        [HttpPut]
        public async Task<IActionResult> UpdateShipOrder(Guid id, ShippingStatus status)
        {
            var shippingOrder = await _context.ShippingOrders.FindAsync(id);
            if (shippingOrder == null)
            {
                return NotFound();
            }
            shippingOrder.Status = status;
            _context.Entry(shippingOrder).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ShippingOrderExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }

        // DELETE: api/Shipping/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteShippingOrder(Guid id)
        {
            var shippingOrder = await _context.ShippingOrders.FindAsync(id);
            if (shippingOrder == null)
            {
                return NotFound();
            }

            _context.ShippingOrders.Remove(shippingOrder);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ShippingOrderExists(Guid id)
        {
            return _context.ShippingOrders.Any(e => e.ShippingId == id);
        }
    }
}
