using Microsoft.EntityFrameworkCore;
using Shipping.Api.Models;

namespace Shipping.Api.Data;

public class ShippingContext : DbContext{
    public ShippingContext(DbContextOptions options) : base(options) {
    }
    
    public DbSet<ShippingOrder> ShippingOrders { get; set; }
}