using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.UseRouting();

// Health check endpoint
app.MapGet("/health", () => Results.Ok("Healthy"));

// API Info endpoint
app.MapGet("/api/info", () => Results.Ok(new
{
    Name = "AutoDocOps API",
    Version = "1.0.0",
    Status = "Running",
    Timestamp = DateTime.UtcNow
}));

// Documentation endpoints
app.MapGet("/api/documentation", () => Results.Ok(new
{
    Message = "AutoDocOps Documentation Service",
    Endpoints = new[]
    {
        "/health",
        "/api/info",
        "/api/documentation",
        "/api/documentation/generate"
    }
}));

app.MapPost("/api/documentation/generate", (DocumentationRequest request) =>
{
    return Results.Ok(new
    {
        Message = "Documentation generation started",
        ProjectName = request.ProjectName,
        Status = "Processing",
        Id = Guid.NewGuid()
    });
});

app.Run();

public record DocumentationRequest(string ProjectName, string? Description = null);

// Make Program class accessible for integration tests
public partial class Program { }

