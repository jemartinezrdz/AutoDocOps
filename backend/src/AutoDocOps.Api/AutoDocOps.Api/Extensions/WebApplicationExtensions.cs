using AutoDocOps.Api.DTOs;
using AutoDocOps.Api.Endpoints;

namespace AutoDocOps.Api.Extensions;

/// <summary>
/// Extensiones para la configuración de la aplicación web
/// </summary>
public static class WebApplicationExtensions
{
    /// <summary>
    /// Configura los endpoints de la API
    /// </summary>
    /// <param name="app">Aplicación web</param>
    /// <returns>Aplicación web configurada</returns>
    public static WebApplication ConfigureEndpoints(this WebApplication app)
    {
        // Endpoint de información de la API
        app.MapGet("/api/info", () => new ApiInfoResponse
        {
            Service = "AutoDocOps API",
            Version = "1.0.0",
            Status = "Running",
            Timestamp = DateTime.UtcNow
        })
        .WithName("GetApiInfo")
        .WithTags("General")
        .WithOpenApi()
        .WithSummary("Obtiene información de la API")
        .WithDescription("Retorna información básica sobre la API AutoDocOps");

        // Mapear endpoints de documentación con IA
        app.MapDocumentationEndpoints();

        // Grupo de endpoints para proyectos
        var projectsGroup = app.MapGroup("/api/projects")
            .WithTags("Projects")
            .RequireAuthorization();

        projectsGroup.MapGet("/", () => "Lista de proyectos")
            .WithName("GetProjects")
            .WithOpenApi();

        projectsGroup.MapPost("/", () => "Crear proyecto")
            .WithName("CreateProject")
            .WithOpenApi();

        // Grupo de endpoints para gestión de documentación
        var docsGroup = app.MapGroup("/api/docs")
            .WithTags("Documentation Management")
            .RequireAuthorization();

        docsGroup.MapGet("/{projectId:guid}", (Guid projectId) => $"Documentación del proyecto {projectId}")
            .WithName("GetDocumentation")
            .WithOpenApi();

        docsGroup.MapPost("/{projectId:guid}/generate", (Guid projectId) => $"Generar documentación para {projectId}")
            .WithName("GenerateDocumentation")
            .WithOpenApi();

        return app;
    }

    /// <summary>
    /// Configura headers de seguridad
    /// </summary>
    /// <param name="app">Aplicación web</param>
    /// <returns>Aplicación web configurada</returns>
    public static WebApplication UseSecurityHeaders(this WebApplication app)
    {
        app.Use(async (context, next) =>
        {
            // HSTS
            context.Response.Headers.Append("Strict-Transport-Security", "max-age=31536000; includeSubDomains");
            
            // CSP
            context.Response.Headers.Append("Content-Security-Policy", 
                "default-src 'self'; script-src 'self' 'unsafe-inline'; style-src 'self' 'unsafe-inline'; img-src 'self' data: https:;");
            
            // Otros headers de seguridad
            context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
            context.Response.Headers.Append("X-Frame-Options", "DENY");
            context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
            context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");

            await next();
        });

        return app;
    }

    /// <summary>
    /// Aplica migraciones de base de datos
    /// </summary>
    /// <param name="app">Aplicación web</param>
    /// <returns>Tarea asíncrona</returns>
    public static async Task ApplyMigrationsAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        
        // Aquí se aplicarían las migraciones cuando se configure Entity Framework
        // var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        // await context.Database.MigrateAsync();
        
        await Task.CompletedTask;
    }
}

