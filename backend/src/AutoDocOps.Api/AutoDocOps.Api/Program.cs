using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Serilog;
using Serilog.Events;
using Microsoft.AspNetCore.HttpOverrides;
using System.Net;
using Microsoft.OpenApi.Models;

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .Enrich.WithEnvironmentName()
    .Enrich.WithMachineName()
    .Enrich.WithProcessId()
    .Enrich.WithThreadId()
    .Filter.ByExcluding(logEvent => 
        logEvent.Properties.ContainsKey("password") ||
        logEvent.Properties.ContainsKey("secret") ||
        logEvent.Properties.ContainsKey("token") ||
        logEvent.MessageTemplate.Text.Contains("password", StringComparison.OrdinalIgnoreCase) ||
        logEvent.MessageTemplate.Text.Contains("secret", StringComparison.OrdinalIgnoreCase))
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

// Use Serilog
builder.Host.UseSerilog();

// Configure JSON serialization with Source Generators
builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    options.SerializerOptions.PropertyNameCaseInsensitive = true;
});

// Add services to the container
builder.Services.AddEndpointsApiExplorer();

// Configure Swagger/OpenAPI
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo 
    { 
        Title = "AutoDocOps API", 
        Version = "v1",
        Description = "AutoDocOps Documentation Generation API",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "AutoDocOps Team",
            Email = "support@autodocops.com"
        }
    });
    
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme (Example: 'Bearer 12345abcdef')",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Configure JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var supabaseJwtSecret = builder.Configuration["Supabase:JwtSecret"];
        if (!string.IsNullOrEmpty(supabaseJwtSecret))
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["Supabase:Url"],
                ValidAudience = "authenticated",
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(supabaseJwtSecret)),
                ClockSkew = TimeSpan.FromMinutes(5)
            };
        }
    });

builder.Services.AddAuthorization();

// Configure CORS with security headers
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(corsBuilder =>
    {
        var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() 
            ?? new[] { "http://localhost:3000", "https://localhost:3000" };
            
        corsBuilder
            .WithOrigins(allowedOrigins)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .WithExposedHeaders("api-version")
            .SetIsOriginAllowedToAllowWildcardSubdomains();
    });
});

// Add health checks
builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("DefaultConnection") ?? "");

// Configure forwarded headers for reverse proxy
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

var app = builder.Build();

// Configure forwarded headers
app.UseForwardedHeaders();

// Security headers
app.Use(async (context, next) =>
{
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Append("X-Frame-Options", "DENY");
    context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
    context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
    context.Response.Headers.Append("Permissions-Policy", "geolocation=(), microphone=(), camera=()");
    
    if (context.Request.IsHttps)
    {
        context.Response.Headers.Append("Strict-Transport-Security", "max-age=31536000; includeSubDomains");
    }
    
    // CSP header
    context.Response.Headers.Append("Content-Security-Policy", 
        "default-src 'self'; script-src 'self' 'unsafe-inline'; style-src 'self' 'unsafe-inline'; img-src 'self' data: https:; font-src 'self'; connect-src 'self'; frame-ancestors 'none'");
        
    await next();
});

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => 
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "AutoDocOps API v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.UseRouting();

// Health check endpoint
app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        var response = new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(entry => new
            {
                name = entry.Key,
                status = entry.Value.Status.ToString(),
                duration = entry.Value.Duration.TotalMilliseconds
            }),
            totalDuration = report.TotalDuration.TotalMilliseconds
        };
        
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(JsonSerializer.Serialize(response, AppJsonSerializerContext.Default.Object));
    }
});

// API Info endpoint
app.MapGet("/api/info", () => Results.Ok(new ApiInfoResponse
{
    Name = "AutoDocOps API",
    Version = "1.0.0",
    Status = "Running",
    Timestamp = DateTime.UtcNow,
    Environment = app.Environment.EnvironmentName
}));

// Documentation endpoints
app.MapGet("/api/v1/documentation", () => Results.Ok(new DocumentationInfoResponse
{
    Message = "AutoDocOps Documentation Service",
    Version = "1.0",
    Endpoints = new[]
    {
        "/health",
        "/api/info",
        "/api/v1/documentation",
        "/api/v1/documentation/generate"
    }
}));

app.MapPost("/api/v1/documentation/generate", 
    (DocumentationRequest request) =>
{
    return Results.Ok(new DocumentationGenerationResponse
    {
        Message = "Documentation generation started",
        ProjectName = request.ProjectName,
        Status = "Processing",
        Id = Guid.NewGuid(),
        Version = "1.0"
    });
});

try
{
    Log.Information("Starting AutoDocOps API");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

// JSON Source Generator Context
[JsonSerializable(typeof(DocumentationRequest))]
[JsonSerializable(typeof(ApiInfoResponse))]
[JsonSerializable(typeof(DocumentationInfoResponse))]
[JsonSerializable(typeof(DocumentationGenerationResponse))]
[JsonSerializable(typeof(object))]
[JsonSerializable(typeof(string[]))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{
}

// DTOs
public record DocumentationRequest(string ProjectName, string? Description = null);

public record ApiInfoResponse
{
    public required string Name { get; init; }
    public required string Version { get; init; }
    public required string Status { get; init; }
    public required DateTime Timestamp { get; init; }
    public required string Environment { get; init; }
}

public record DocumentationInfoResponse
{
    public required string Message { get; init; }
    public required string Version { get; init; }
    public required string[] Endpoints { get; init; }
}

public record DocumentationGenerationResponse
{
    public required string Message { get; init; }
    public required string ProjectName { get; init; }
    public required string Status { get; init; }
    public required Guid Id { get; init; }
    public required string Version { get; init; }
}

// Make Program class accessible for integration tests
public partial class Program { }

