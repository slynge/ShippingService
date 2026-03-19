using Microsoft.EntityFrameworkCore;
using Shipping.Api.Models;

namespace Shipping.Api.Data
{
    public class ShippingContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<ShippingOrder> ShippingOrders { get; set; }
    }
}
