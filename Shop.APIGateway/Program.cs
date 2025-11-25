using RedisRateLimiting;
using Shop.APIGateway.Extensions;
using Shop.ServiceDefaults;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddYarpReverseProxy(builder.Configuration);

builder.Services.AddRateLimiter(rateLimiterOptions =>
{
    rateLimiterOptions.AddPolicy("open", context =>
    {
        var redis = context.RequestServices.GetRequiredService<IConnectionMultiplexer>();
        var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        return RedisRateLimitPartition.GetFixedWindowRateLimiter(
    $"ip:{ipAddress}",
    _ => new RedisFixedWindowRateLimiterOptions
    {
        ConnectionMultiplexerFactory = () => redis,
        PermitLimit = 5,
        Window = TimeSpan.FromMinutes(1)

    });
    });
});


// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.AddRedisClient("redis");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthorization();

app.UseRateLimiter();

app.MapReverseProxy();

app.MapControllers();

app.MapDefaultEndpoints();

app.Run();
