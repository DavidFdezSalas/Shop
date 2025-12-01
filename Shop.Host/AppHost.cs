var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithDataVolume(isReadOnly: false)
    .WithLifetime(ContainerLifetime.Persistent)
    .WithPgAdmin(pgAdmin => pgAdmin.WithHostPort(5050));

var mailServer = builder.AddContainer("maildev", "maildev/maildev:latest")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithEndpoint(port: 1025, targetPort:1025, name: "smtp")
    .WithHttpEndpoint(port: 1080, targetPort: 1080, name: "web");

var redis = builder.AddRedis("redis")
    .WithDataVolume(isReadOnly: false)
    .WithLifetime(ContainerLifetime.Persistent)
    .WithRedisInsight();

var rabbitmq = builder.AddRabbitMQ("rabbitmq")
    .WithDataVolume(isReadOnly: false)
    .WithLifetime(ContainerLifetime.Persistent)
    .WithManagementPlugin();

var postgresdb = postgres.AddDatabase("postgresdb");

var identity1 = builder.AddProject<Projects.Shop_APIIdentity>("shop-apiidentity1")
    .WaitFor(postgres)
    .WithReference(postgresdb)
    .WaitFor(rabbitmq)
    .WithReference(rabbitmq);

builder.AddProject<Projects.Shop_APIGateway>("shop-apigateway")
    .WithReference(identity1)
    .WaitFor(identity1)
    .WithReference(redis)
    .WaitFor(redis);

builder.AddProject<Projects.Shop_Notifications>("shop-notifications")
    .WaitFor(rabbitmq)
    .WithReference(rabbitmq);

builder.Build().Run();
