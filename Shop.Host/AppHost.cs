var builder = DistributedApplication.CreateBuilder(args);



var postgres = builder.AddPostgres("postgres")
    .WithDataVolume(isReadOnly: false)
    .WithLifetime(ContainerLifetime.Persistent)
    .WithPgAdmin(pgAdmin => pgAdmin.WithHostPort(5050));

var redis = builder.AddRedis("redis")
    .WithDataVolume(isReadOnly: false)
    .WithLifetime(ContainerLifetime.Persistent)
    .WithRedisInsight;

var postgresdb = postgres.AddDatabase("postgresdb");

var identity1 = builder.AddProject<Projects.Shop_APIIdentity>("shop-apiidentity1")
    .WaitFor(postgres)
    .WithReference(postgresdb)
    .WithEnvironment("Version", "1");

var identity2 = builder.AddProject<Projects.Shop_APIIdentity>("shop-apiidentity2")
    .WaitFor(postgres)
    .WithReference(postgresdb)
    .WithEnvironment("Version", "2");

builder.AddProject<Projects.Shop_APIGateway>("shop-apigateway")
    .WithReference(identity1)
    .WithReference(identity2)
    .WaitFor(identity1)
    .WaitFor(identity2);

builder.Build().Run();
