using RedisRateLimiting;
using Shop.APIGateway.Extensions;
using Shop.ServiceDefaults;
using Shop.ServiceDefaults.Authentication;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Configuration.AddUserSecrets(typeof(Program).Assembly, true);

// Registrar AUTH antes de YARP para que las políticas existan cuando YARP lea la configuración
// JWT
builder.Services.AddJwtAuthentication(builder.Configuration);

// Ahora registrar YARP
builder.Services.AddYarpReverseProxy(builder.Configuration);

// Add services to the container.
builder.Services.AddControllers();

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
                PermitLimit = 250,
                Window = TimeSpan.FromMinutes(1)
            });
    });
});

builder.Services.AddOpenApi();

builder.AddRedisClient("redis");

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthentication();

app.UseAuthorization();

app.UseRateLimiter();

app.MapReverseProxy();

app.Run();
