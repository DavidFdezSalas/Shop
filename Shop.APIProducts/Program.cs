using Asp.Versioning;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Shop.APIProducts.Data;
using Shop.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddControllers();

builder.Services.AddOpenApi();

builder.AddNpgsqlDbContext<ProductDbContext>("productsdb");

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1);
    options.ReportApiVersions = true;
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader(),
        new HeaderApiVersionReader("X-Api-Version"));
})
.AddMvc()
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'V";
    options.SubstituteApiVersionInUrl = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "v1");
    });

    // ===== MIGRACIONES AUTOMÁTICAS =====
    // Esto crea/actualiza las tablas automáticamente al arrancar
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ProductDbContext>();
    await context.Database.MigrateAsync();
    await ProductsSeeder.SeedCategoriesAndProductsAsync(context, app.Logger);
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.MapDefaultEndpoints();

app.Run();
