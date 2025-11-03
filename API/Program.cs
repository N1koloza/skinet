using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDbContext<StoreContext>(options =>
{
    var connString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseNpgsql(connString, npgsqlOptions =>
    {
        // optional: enable retry on failure for transient errors
        npgsqlOptions.EnableRetryOnFailure(maxRetryCount: 5);
    });
});

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapControllers();

app.Run();
