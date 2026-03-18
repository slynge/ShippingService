using Microsoft.EntityFrameworkCore;
using Shipping.Api.Models;

namespace Shipping.Api.Data
{
    public class ShippingContext : DbContext
    {
        public ShippingContext(DbContextOptions contextOptions) : base(contextOptions) { }

        public DbSet<ShippingOrder> ShippingOrders { get; set; }
    }
}
