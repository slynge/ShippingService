var builder = DistributedApplication.CreateBuilder(args);

var username = builder.AddParameter("username", "guest");
var password = builder.AddParameter("password", "guest");
var rabbitmq = builder.AddRabbitMQ("messaging", username, password)
    .WithLifetime(ContainerLifetime.Persistent)
    .WithManagementPlugin();

var pgpassword = builder.AddParameter("postgresql-password", true);
var postgres = builder.AddPostgres("postgres", password: pgpassword)
    .WithPgWeb()
    .WithDataVolume(isReadOnly: false)
    .WithLifetime(ContainerLifetime.Persistent);
var postgresdb = postgres.AddDatabase("postgresdb");

builder.AddProject<Projects.Orders_Api>("order-api")
    .WithReference(rabbitmq)
    .WaitFor(rabbitmq)
    .WithReference(postgresdb)
    .WaitFor(postgresdb);

builder.AddProject<Projects.Shipping_Api>("shipping")
    .WithReference(rabbitmq)
    .WaitFor(rabbitmq)
    .WithReference(postgresdb)
    .WaitFor(postgresdb);

builder.AddProject<Projects.Shipping_Api>("shipping-api");

builder.AddProject<Projects.Shipping_RabbitMQ>("shipping-rabbitmq");

builder.Build().Run();
