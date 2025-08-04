using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.ComponentModel.DataAnnotations;
using AutoDocOps.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add logging
builder.Services.AddLogging();

// Add HTTP client
builder.Services.AddHttpClient();

// Add OpenAI services
builder.Services.AddOpenAIServices(builder.Configuration);

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
    service = "AutoDocOps API",
    version = "1.0.0",
    status = "Running",
    timestamp = DateTime.UtcNow
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
        "/api/documentation/analyze-dotnet",
        "/api/documentation/analyze-sqlserver",
        "/api/documentation/semantic-chat",
        "/api/documentation/generate-guides",
        "/api/documentation/generate-postman",
        "/api/documentation/generate-typescript-sdk",
        "/api/documentation/generate-csharp-sdk",
        "/api/documentation/generate-er-diagram",
        "/api/documentation/generate-data-dictionary"
    }
}));

app.MapPost("/api/documentation/analyze-dotnet", async (AnalyzeDotNetRequest request, IOpenAIService openAIService) =>
{
    try
    {
        // Validar entrada
        if (string.IsNullOrWhiteSpace(request.SourceCode))
        {
            return Results.BadRequest(new { success = false, message = "El código fuente es requerido" });
        }

        var openApiSpec = await openAIService.AnalyzeDotNetApiAsync(request.SourceCode, request.Language ?? "es");
        
        return Results.Ok(new
        {
            success = true,
            openApiSpecification = openApiSpec,
            metadata = new { language = request.Language, timestamp = DateTime.UtcNow },
            message = "Análisis completado exitosamente"
        });
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { success = false, message = ex.Message });
    }
    catch (Exception ex)
    {
        return Results.Problem(new { success = false, message = "Error interno del servidor", details = ex.Message });
    }
});

app.MapPost("/api/documentation/analyze-sqlserver", async (AnalyzeSqlServerRequest request, IOpenAIService openAIService) =>
{
    try
    {
        // Validar entrada
        if (string.IsNullOrWhiteSpace(request.ConnectionString))
        {
            return Results.BadRequest(new { success = false, message = "La cadena de conexión es requerida" });
        }

        // Simular análisis de base de datos (en una implementación real, se conectaría a SQL Server)
        var schemaDefinition = "CREATE TABLE Users (Id INT PRIMARY KEY, Name NVARCHAR(100));";
        var documentation = await openAIService.AnalyzeSqlServerSchemaAsync(schemaDefinition, request.Language ?? "es");
        
        return Results.Ok(new
        {
            success = true,
            documentation = documentation,
            message = "Análisis de SQL Server completado exitosamente"
        });
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { success = false, message = ex.Message });
    }
    catch (Exception ex)
    {
        return Results.Problem(new { success = false, message = "Error conectando a la base de datos", details = ex.Message });
    }
});

app.MapPost("/api/documentation/semantic-chat", async (SemanticChatRequest request, IOpenAIService openAIService) =>
{
    try
    {
        var answer = await openAIService.AnswerQuestionAsync(request.Question, request.Context, request.Language ?? "es");
        
        return Results.Ok(new
        {
            success = true,
            answer = answer,
            context = request.Context,
            message = "Respuesta generada exitosamente"
        });
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { success = false, message = ex.Message });
    }
    catch (Exception ex)
    {
        return Results.Problem(new { success = false, message = "Error procesando la consulta", details = ex.Message });
    }
});

app.MapPost("/api/documentation/generate-guides", async (GenerateGuidesRequest request, IOpenAIService openAIService) =>
{
    try
    {
        var usageGuides = await openAIService.GenerateUsageGuidesAsync(request.OpenApiSpecification, request.Language ?? "es");
        
        return Results.Ok(new
        {
            success = true,
            usageGuides = usageGuides,
            message = "Guías de uso generadas exitosamente"
        });
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { success = false, message = ex.Message });
    }
    catch (Exception ex)
    {
        return Results.Problem(new { success = false, message = "Error generando guías", details = ex.Message });
    }
});

app.MapPost("/api/documentation/generate-postman", async (GeneratePostmanRequest request, IOpenAIService openAIService) =>
{
    try
    {
        var postmanCollection = await openAIService.GeneratePostmanCollectionAsync(request.OpenApiSpecification, request.BaseUrl);
        
        return Results.Ok(new
        {
            success = true,
            postmanCollection = postmanCollection,
            message = "Colección de Postman generada exitosamente"
        });
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { success = false, message = ex.Message });
    }
    catch (Exception ex)
    {
        return Results.Problem(new { success = false, message = "Error generando colección", details = ex.Message });
    }
});

app.MapPost("/api/documentation/generate-typescript-sdk", async (GenerateTypeScriptSDKRequest request, IOpenAIService openAIService) =>
{
    try
    {
        var sdkCode = await openAIService.GenerateTypeScriptSDKAsync(request.OpenApiSpecification, request.PackageName);
        
        return Results.Ok(new
        {
            success = true,
            sdkCode = sdkCode,
            message = "SDK de TypeScript generado exitosamente"
        });
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { success = false, message = ex.Message });
    }
    catch (Exception ex)
    {
        return Results.Problem(new { success = false, message = "Error generando SDK", details = ex.Message });
    }
});

app.MapPost("/api/documentation/generate-csharp-sdk", async (GenerateCSharpSDKRequest request, IOpenAIService openAIService) =>
{
    try
    {
        var sdkCode = await openAIService.GenerateCSharpSDKAsync(request.OpenApiSpecification, request.Namespace);
        
        return Results.Ok(new
        {
            success = true,
            sdkCode = sdkCode,
            message = "SDK de C# generado exitosamente"
        });
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { success = false, message = ex.Message });
    }
    catch (Exception ex)
    {
        return Results.Problem(new { success = false, message = "Error generando SDK", details = ex.Message });
    }
});

app.MapPost("/api/documentation/generate-er-diagram", async (GenerateERDiagramRequest request, IOpenAIService openAIService) =>
{
    try
    {
        var mermaidCode = await openAIService.GenerateERDiagramAsync(request.SchemaDefinition, request.Language ?? "es");
        
        return Results.Ok(new
        {
            success = true,
            mermaidCode = mermaidCode,
            message = "Diagrama ER generado exitosamente"
        });
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { success = false, message = ex.Message });
    }
    catch (Exception ex)
    {
        return Results.Problem(new { success = false, message = "Error generando diagrama", details = ex.Message });
    }
});

app.MapPost("/api/documentation/generate-data-dictionary", async (GenerateDataDictionaryRequest request, IOpenAIService openAIService) =>
{
    try
    {
        var dataDictionary = await openAIService.GenerateDataDictionaryAsync(request.SchemaDefinition, request.Language ?? "es");
        
        return Results.Ok(new
        {
            success = true,
            dataDictionary = dataDictionary,
            message = "Diccionario de datos generado exitosamente"
        });
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { success = false, message = ex.Message });
    }
    catch (Exception ex)
    {
        return Results.Problem(new { success = false, message = "Error generando diccionario", details = ex.Message });
    }
});

app.Run();

// Make the implicit Program class public so test projects can access it
public partial class Program { }

// Request DTOs
public record AnalyzeDotNetRequest(
    [Required] string SourceCode, 
    string? Language = "es");

public record AnalyzeSqlServerRequest(
    [Required] string ConnectionString, 
    string? Language = "es");

public record SemanticChatRequest(
    [Required] string Question, 
    [Required] string Context, 
    string? Language = "es");

public record GenerateGuidesRequest(
    [Required] string OpenApiSpecification, 
    string? Language = "es");

public record GeneratePostmanRequest(
    [Required] string OpenApiSpecification, 
    [Required] string BaseUrl);

public record GenerateTypeScriptSDKRequest(
    [Required] string OpenApiSpecification, 
    [Required] string PackageName);

public record GenerateCSharpSDKRequest(
    [Required] string OpenApiSpecification, 
    [Required] string Namespace);

public record GenerateERDiagramRequest(
    [Required] string SchemaDefinition, 
    string? Language = "es");

public record GenerateDataDictionaryRequest(
    [Required] string SchemaDefinition, 
    string? Language = "es");

public record DocumentationRequest(string ProjectName, string? Description = null);

