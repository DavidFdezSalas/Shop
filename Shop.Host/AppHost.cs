var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithDataVolume(isReadOnly: false)
    .WithLifetime(ContainerLifetime.Persistent)
    .WithPgAdmin(pgAdmin => pgAdmin.WithHostPort(5050));

var identitydb = postgres.AddDatabase("identitydb");
var productsdb = postgres.AddDatabase("productsdb");
var ordersdb = postgres.AddDatabase("ordersdb");

var mailServer = builder.AddContainer("maildev", "maildev/maildev:latest")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithEndpoint(port: 1025, targetPort: 1025, name: "smtp")
    .WithHttpEndpoint(port: 1080, targetPort: 1080, name: "web");

var redis = builder.AddRedis("redis")
    .WithDataVolume(isReadOnly: false)
    .WithLifetime(ContainerLifetime.Persistent)
    .WithRedisInsight();

var rabbitmq = builder.AddRabbitMQ("rabbitmq")
    .WithDataVolume(isReadOnly: false)
    .WithLifetime(ContainerLifetime.Persistent)
    .WithManagementPlugin();

var jwtSettings = builder.Configuration.GetSection("JwtSettings");

var identity = builder.AddProject<Projects.Shop_APIIdentity>("shop-apiidentity")
    .WaitFor(postgres)
    .WithReference(identitydb)
    .WaitFor(rabbitmq)
    .WithReference(rabbitmq)
    .WithEnvironment("JwtSettings__Key", jwtSettings["Key"])
    .WithEnvironment("JwtSettings__Issuer", jwtSettings["Issuer"])
    .WithEnvironment("JwtSettings__Audience", jwtSettings["Audience"])
    .WithEnvironment("JwtSettings__ExpirationInMinutes", jwtSettings["ExpirationInMinutes"]);

var products = builder.AddProject<Projects.Shop_APIProducts>("shop-apiproducts")
    .WaitFor(postgres)
    .WithReference(productsdb)
    .WithEnvironment("JwtSettings__Key", jwtSettings["Key"])
    .WithEnvironment("JwtSettings__Issuer", jwtSettings["Issuer"])
    .WithEnvironment("JwtSettings__Audience", jwtSettings["Audience"])
    .WithEnvironment("JwtSettings__ExpirationInMinutes", jwtSettings["ExpirationInMinutes"]);

var orders = builder.AddProject<Projects.Shop_APIOrders>("shop-apiorders")
    .WaitFor(postgres)
    .WithReference(ordersdb)
    .WaitFor(rabbitmq)
    .WithReference(rabbitmq)
    .WithEnvironment("JwtSettings__Key", jwtSettings["Key"])
    .WithEnvironment("JwtSettings__Issuer", jwtSettings["Issuer"])
    .WithEnvironment("JwtSettings__Audience", jwtSettings["Audience"])
    .WithEnvironment("JwtSettings__ExpirationInMinutes", jwtSettings["ExpirationInMinutes"]);

var gateway = builder.AddProject<Projects.Shop_APIGateway>("shop-apigateway")
    .WithReference(identity)
    .WaitFor(identity)
    .WithReference(products)
    .WaitFor(products)
    .WithReference(orders)
    .WaitFor(orders)
    .WithReference(redis)
    .WaitFor(redis)
    .WithEnvironment("JwtSettings__Key", jwtSettings["Key"])
    .WithEnvironment("JwtSettings__Issuer", jwtSettings["Issuer"])
    .WithEnvironment("JwtSettings__Audience", jwtSettings["Audience"])
    .WithEnvironment("JwtSettings__ExpirationInMinutes", jwtSettings["ExpirationInMinutes"]);

var web = builder.AddNpmApp("shop-web", "../Shop.Web", "dev").WithReference(gateway).WaitFor(gateway);

builder.AddProject<Projects.Shop_Notifications>("shop-notifications")
    .WaitFor(rabbitmq)
    .WithReference(rabbitmq);

builder.Build().Run();
