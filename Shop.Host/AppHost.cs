var builder = DistributedApplication.CreateBuilder(args);



var postgres = builder.AddPostgres("postgres")
    .WithDataVolume(isReadOnly: false)
    .WithLifetime(ContainerLifetime.Persistent)
    .WithPgAdmin(pgAdmin => pgAdmin.WithHostPort(5050));

var redis = builder.AddRedis("redis")
    .WithDataVolume(isReadOnly: false)
    .WithLifetime(ContainerLifetime.Persistent)
    .WithRedisInsight();

var postgresdb = postgres.AddDatabase("postgresdb");

var identity1 = builder.AddProject<Projects.Shop_APIIdentity>("shop-apiidentity1")
    .WaitFor(postgres)
    .WithReference(postgresdb)
    .WithEnvironment("Version", "1");

builder.AddProject<Projects.Shop_APIGateway>("shop-apigateway")
    .WithReference(identity1)
    .WaitFor(identity1);

builder.Build().Run();
