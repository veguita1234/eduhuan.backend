using EduHuan.backend.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
        options.JsonSerializerOptions.MaxDepth = 10000;
    });

// CORS Policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", policyBuilder =>
    {
        policyBuilder.AllowAnyOrigin()
                     .AllowAnyHeader()
                     .AllowAnyMethod();
    });
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("Connection")),
    ServiceLifetime.Scoped);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Use CORS middleware before authorization and other middlewares
app.UseCors("AllowAllOrigins");
app.UseAuthorization();

app.MapControllers();

app.Run();
