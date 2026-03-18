using Microsoft.EntityFrameworkCore;

namespace Orders.Api.data
{
    public class OrdersContext : DbContext
    {
        public OrdersContext(DbContextOptions options) :base(options)
        {
            
        }

        public DbSet<Models.Order> Orders { get; set; }
    }
}
