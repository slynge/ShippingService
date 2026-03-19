
using Scalar.AspNetCore;
using Shipping.Api.Data;
using Shipping.Api.Services;

namespace Shipping.Api;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.AddServiceDefaults();

        builder.AddRabbitMQClient("messaging");
        builder.AddNpgsqlDbContext<ShippingContext>("shippingdb");
        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();
        builder.Services.AddHostedService<ShippingMessageReceiver>();

        var app = builder.Build();

        app.MapDefaultEndpoints();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference();
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
