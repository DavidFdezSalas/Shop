using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);



var postgres = builder.AddPostgres("postgres")
    .WithDataVolume(isReadOnly: false)
    .WithLifetime(ContainerLifetime.Persistent)
    .WithPgAdmin(pgAdmin => pgAdmin.WithHostPort(5050));

var postgresdb = postgres.AddDatabase("postgresdb");

builder.AddProject<Projects.Shop_APIIdentity>("shop-apiidentity")
    .WaitFor(postgres)
    .WithReference(postgresdb);

builder.Build().Run();
