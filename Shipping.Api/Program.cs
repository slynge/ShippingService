
using Shipping.Api.Data;

namespace Shipping.Api;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.AddServiceDefaults();

        builder.AddRabbitMQClient("messaging");
        builder.AddNpgsqlDbContext<ShippingContext>("postgresdb");
        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        var app = builder.Build();

        app.MapDefaultEndpoints();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();


        app.MapControllers();

        if (app.Environment.IsDevelopment())
        {
            // Ensure database is created and seeded
            using var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ShippingContext>();
            await context.Database.EnsureCreatedAsync();
        }

        app.Run();
    }
}
