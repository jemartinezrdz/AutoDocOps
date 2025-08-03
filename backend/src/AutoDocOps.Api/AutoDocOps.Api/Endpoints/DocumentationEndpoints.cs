using Microsoft.AspNetCore.Mvc;
using AutoDocOps.Infrastructure.Services;

namespace AutoDocOps.Api.Endpoints;

/// <summary>
/// Endpoints para funcionalidades de documentación automática
/// </summary>
public static class DocumentationEndpoints
{
    /// <summary>
    /// Configura los endpoints de documentación
    /// </summary>
    /// <param name="app">Aplicación web</param>
    /// <returns>Aplicación web configurada</returns>
    public static WebApplication MapDocumentationEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/documentation")
            .WithTags("Documentation")
            .WithOpenApi();

        // Endpoint para analizar código .NET
        group.MapPost("/analyze-dotnet", AnalyzeDotNetCode)
            .WithName("AnalyzeDotNetCode")
            .WithSummary("Analiza código .NET y genera documentación OpenAPI")
            .WithDescription("Recibe código fuente .NET y genera especificación OpenAPI 3.1 completa");

        // Endpoint para analizar esquema SQL Server
        group.MapPost("/analyze-sqlserver", AnalyzeSqlServerSchema)
            .WithName("AnalyzeSqlServerSchema")
            .WithSummary("Analiza esquema SQL Server y genera documentación")
            .WithDescription("Extrae esquema de base de datos SQL Server y genera documentación completa");

        // Endpoint para generar guías de uso
        group.MapPost("/generate-guides", GenerateUsageGuides)
            .WithName("GenerateUsageGuides")
            .WithSummary("Genera guías de uso desde especificación OpenAPI")
            .WithDescription("Crea guías de uso detalladas basadas en especificación OpenAPI");

        // Endpoint para generar colección Postman
        group.MapPost("/generate-postman", GeneratePostmanCollection)
            .WithName("GeneratePostmanCollection")
            .WithSummary("Genera colección Postman desde OpenAPI")
            .WithDescription("Convierte especificación OpenAPI a colección Postman v2.1");

        // Endpoint para generar SDK TypeScript
        group.MapPost("/generate-typescript-sdk", GenerateTypeScriptSDK)
            .WithName("GenerateTypeScriptSDK")
            .WithSummary("Genera SDK TypeScript desde OpenAPI")
            .WithDescription("Crea SDK completo en TypeScript basado en especificación OpenAPI");

        // Endpoint para generar SDK C#
        group.MapPost("/generate-csharp-sdk", GenerateCSharpSDK)
            .WithName("GenerateCSharpSDK")
            .WithSummary("Genera SDK C# desde OpenAPI")
            .WithDescription("Crea SDK completo en C# basado en especificación OpenAPI");

        // Endpoint para chat semántico
        group.MapPost("/semantic-chat", SemanticChat)
            .WithName("SemanticChat")
            .WithSummary("Chat semántico sobre documentación")
            .WithDescription("Responde preguntas sobre documentación usando IA y búsqueda semántica");

        // Endpoint para generar diagrama ER
        group.MapPost("/generate-er-diagram", GenerateERDiagram)
            .WithName("GenerateERDiagram")
            .WithSummary("Genera diagrama ER desde esquema de base de datos")
            .WithDescription("Crea diagrama ER en formato Mermaid basado en esquema SQL Server");

        // Endpoint para generar diccionario de datos
        group.MapPost("/generate-data-dictionary", GenerateDataDictionary)
            .WithName("GenerateDataDictionary")
            .WithSummary("Genera diccionario de datos desde esquema")
            .WithDescription("Crea diccionario de datos completo basado en esquema de base de datos");

        return app;
    }

    /// <summary>
    /// Analiza código .NET y genera documentación OpenAPI
    /// </summary>
    private static async Task<IResult> AnalyzeDotNetCode(
        [FromBody] AnalyzeDotNetRequest request,
        [FromServices] IOpenAIService openAIService,
        [FromServices] ICodeAnalysisService codeAnalysisService,
        [FromServices] ILogger<Program> logger)
    {
        try
        {
            logger.LogInformation("Iniciando análisis de código .NET");

            // Analizar código con servicio de análisis
            var codeMetadata = await codeAnalysisService.AnalyzeDotNetCodeAsync(request.SourceCode);

            // Generar documentación OpenAPI con IA
            var openApiSpec = await openAIService.AnalyzeDotNetApiAsync(request.SourceCode, request.Language);

            var response = new AnalyzeDotNetResponse(
                Success: true,
                OpenApiSpecification: openApiSpec,
                Metadata: codeMetadata,
                Message: "Análisis completado exitosamente"
            );

            logger.LogInformation("Análisis de código .NET completado");
            return Results.Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error analizando código .NET");
            return Results.Problem(
                title: "Error en análisis",
                detail: ex.Message,
                statusCode: 500);
        }
    }

    /// <summary>
    /// Analiza esquema SQL Server y genera documentación
    /// </summary>
    private static async Task<IResult> AnalyzeSqlServerSchema(
        [FromBody] AnalyzeSqlServerRequest request,
        [FromServices] IOpenAIService openAIService,
        [FromServices] ICodeAnalysisService codeAnalysisService,
        [FromServices] ILogger<Program> logger)
    {
        try
        {
            logger.LogInformation("Iniciando análisis de esquema SQL Server");

            // Validar cadena de conexión
            var isValidConnection = await codeAnalysisService.ValidateConnectionStringAsync(request.ConnectionString);
            if (!isValidConnection)
            {
                return Results.BadRequest(new { Message = "Cadena de conexión inválida" });
            }

            // Extraer esquema
            var schemaResult = await codeAnalysisService.ExtractSqlServerSchemaAsync(request.ConnectionString);

            // Generar documentación con IA
            var documentation = await openAIService.AnalyzeSqlServerSchemaAsync(schemaResult.SchemaDefinition, request.Language);

            var response = new AnalyzeSqlServerResponse(
                Success: true,
                Documentation: documentation,
                SchemaResult: schemaResult,
                Message: "Análisis de esquema completado exitosamente"
            );

            logger.LogInformation("Análisis de esquema SQL Server completado");
            return Results.Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error analizando esquema SQL Server");
            return Results.Problem(
                title: "Error en análisis",
                detail: ex.Message,
                statusCode: 500);
        }
    }

    /// <summary>
    /// Genera guías de uso desde especificación OpenAPI
    /// </summary>
    private static async Task<IResult> GenerateUsageGuides(
        [FromBody] GenerateGuidesRequest request,
        [FromServices] IOpenAIService openAIService,
        [FromServices] ILogger<Program> logger)
    {
        try
        {
            logger.LogInformation("Generando guías de uso");

            var guides = await openAIService.GenerateUsageGuidesAsync(request.OpenApiSpecification, request.Language);

            var response = new GenerateGuidesResponse(
                Success: true,
                UsageGuides: guides,
                Message: "Guías de uso generadas exitosamente"
            );

            logger.LogInformation("Guías de uso generadas");
            return Results.Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error generando guías de uso");
            return Results.Problem(
                title: "Error generando guías",
                detail: ex.Message,
                statusCode: 500);
        }
    }

    /// <summary>
    /// Genera colección Postman desde OpenAPI
    /// </summary>
    private static async Task<IResult> GeneratePostmanCollection(
        [FromBody] GeneratePostmanRequest request,
        [FromServices] IOpenAIService openAIService,
        [FromServices] ILogger<Program> logger)
    {
        try
        {
            logger.LogInformation("Generando colección Postman");

            var collection = await openAIService.GeneratePostmanCollectionAsync(request.OpenApiSpecification, request.BaseUrl);

            var response = new GeneratePostmanResponse(
                Success: true,
                PostmanCollection: collection,
                Message: "Colección Postman generada exitosamente"
            );

            logger.LogInformation("Colección Postman generada");
            return Results.Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error generando colección Postman");
            return Results.Problem(
                title: "Error generando colección",
                detail: ex.Message,
                statusCode: 500);
        }
    }

    /// <summary>
    /// Genera SDK TypeScript desde OpenAPI
    /// </summary>
    private static async Task<IResult> GenerateTypeScriptSDK(
        [FromBody] GenerateTypeScriptSDKRequest request,
        [FromServices] IOpenAIService openAIService,
        [FromServices] ILogger<Program> logger)
    {
        try
        {
            logger.LogInformation("Generando SDK TypeScript");

            var sdk = await openAIService.GenerateTypeScriptSDKAsync(request.OpenApiSpecification, request.PackageName);

            var response = new GenerateSDKResponse(
                Success: true,
                SDKCode: sdk,
                Message: "SDK TypeScript generado exitosamente"
            );

            logger.LogInformation("SDK TypeScript generado");
            return Results.Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error generando SDK TypeScript");
            return Results.Problem(
                title: "Error generando SDK",
                detail: ex.Message,
                statusCode: 500);
        }
    }

    /// <summary>
    /// Genera SDK C# desde OpenAPI
    /// </summary>
    private static async Task<IResult> GenerateCSharpSDK(
        [FromBody] GenerateCSharpSDKRequest request,
        [FromServices] IOpenAIService openAIService,
        [FromServices] ILogger<Program> logger)
    {
        try
        {
            logger.LogInformation("Generando SDK C#");

            var sdk = await openAIService.GenerateCSharpSDKAsync(request.OpenApiSpecification, request.Namespace);

            var response = new GenerateSDKResponse(
                Success: true,
                SDKCode: sdk,
                Message: "SDK C# generado exitosamente"
            );

            logger.LogInformation("SDK C# generado");
            return Results.Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error generando SDK C#");
            return Results.Problem(
                title: "Error generando SDK",
                detail: ex.Message,
                statusCode: 500);
        }
    }

    /// <summary>
    /// Chat semántico sobre documentación
    /// </summary>
    private static async Task<IResult> SemanticChat(
        [FromBody] SemanticChatRequest request,
        [FromServices] IOpenAIService openAIService,
        [FromServices] IEmbeddingService embeddingService,
        [FromServices] ILogger<Program> logger)
    {
        try
        {
            logger.LogInformation("Procesando consulta de chat semántico");

            // TODO: Implementar búsqueda semántica en base de datos
            // Por ahora, usar contexto proporcionado directamente
            var context = request.Context ?? "No hay contexto disponible";

            var answer = await openAIService.AnswerQuestionAsync(request.Question, context, request.Language);

            var response = new SemanticChatResponse(
                Success: true,
                Answer: answer,
                Context: context,
                Message: "Respuesta generada exitosamente"
            );

            logger.LogInformation("Consulta de chat semántico procesada");
            return Results.Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error en chat semántico");
            return Results.Problem(
                title: "Error en chat",
                detail: ex.Message,
                statusCode: 500);
        }
    }

    /// <summary>
    /// Genera diagrama ER desde esquema de base de datos
    /// </summary>
    private static async Task<IResult> GenerateERDiagram(
        [FromBody] GenerateERDiagramRequest request,
        [FromServices] IOpenAIService openAIService,
        [FromServices] ILogger<Program> logger)
    {
        try
        {
            logger.LogInformation("Generando diagrama ER");

            var diagram = await openAIService.GenerateERDiagramAsync(request.SchemaDefinition, request.Language);

            var response = new GenerateERDiagramResponse(
                Success: true,
                MermaidCode: diagram,
                Message: "Diagrama ER generado exitosamente"
            );

            logger.LogInformation("Diagrama ER generado");
            return Results.Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error generando diagrama ER");
            return Results.Problem(
                title: "Error generando diagrama",
                detail: ex.Message,
                statusCode: 500);
        }
    }

    /// <summary>
    /// Genera diccionario de datos desde esquema
    /// </summary>
    private static async Task<IResult> GenerateDataDictionary(
        [FromBody] GenerateDataDictionaryRequest request,
        [FromServices] IOpenAIService openAIService,
        [FromServices] ILogger<Program> logger)
    {
        try
        {
            logger.LogInformation("Generando diccionario de datos");

            var dictionary = await openAIService.GenerateDataDictionaryAsync(request.SchemaDefinition, request.Language);

            var response = new GenerateDataDictionaryResponse(
                Success: true,
                DataDictionary: dictionary,
                Message: "Diccionario de datos generado exitosamente"
            );

            logger.LogInformation("Diccionario de datos generado");
            return Results.Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error generando diccionario de datos");
            return Results.Problem(
                title: "Error generando diccionario",
                detail: ex.Message,
                statusCode: 500);
        }
    }
}

#region DTOs de Request y Response

/// <summary>
/// Request para análisis de código .NET
/// </summary>
public record AnalyzeDotNetRequest(string SourceCode, string Language = "es");

/// <summary>
/// Response para análisis de código .NET
/// </summary>
public record AnalyzeDotNetResponse(
    bool Success,
    string OpenApiSpecification,
    CodeMetadata Metadata,
    string Message);

/// <summary>
/// Request para análisis de esquema SQL Server
/// </summary>
public record AnalyzeSqlServerRequest(string ConnectionString, string Language = "es");

/// <summary>
/// Response para análisis de esquema SQL Server
/// </summary>
public record AnalyzeSqlServerResponse(
    bool Success,
    string Documentation,
    DatabaseSchemaResult SchemaResult,
    string Message);

/// <summary>
/// Request para generar guías de uso
/// </summary>
public record GenerateGuidesRequest(string OpenApiSpecification, string Language = "es");

/// <summary>
/// Response para generar guías de uso
/// </summary>
public record GenerateGuidesResponse(bool Success, string UsageGuides, string Message);

/// <summary>
/// Request para generar colección Postman
/// </summary>
public record GeneratePostmanRequest(string OpenApiSpecification, string BaseUrl);

/// <summary>
/// Response para generar colección Postman
/// </summary>
public record GeneratePostmanResponse(bool Success, string PostmanCollection, string Message);

/// <summary>
/// Request para generar SDK TypeScript
/// </summary>
public record GenerateTypeScriptSDKRequest(string OpenApiSpecification, string PackageName);

/// <summary>
/// Request para generar SDK C#
/// </summary>
public record GenerateCSharpSDKRequest(string OpenApiSpecification, string Namespace);

/// <summary>
/// Response para generar SDK
/// </summary>
public record GenerateSDKResponse(bool Success, string SDKCode, string Message);

/// <summary>
/// Request para chat semántico
/// </summary>
public record SemanticChatRequest(string Question, string? Context = null, string Language = "es");

/// <summary>
/// Response para chat semántico
/// </summary>
public record SemanticChatResponse(bool Success, string Answer, string Context, string Message);

/// <summary>
/// Request para generar diagrama ER
/// </summary>
public record GenerateERDiagramRequest(string SchemaDefinition, string Language = "es");

/// <summary>
/// Response para generar diagrama ER
/// </summary>
public record GenerateERDiagramResponse(bool Success, string MermaidCode, string Message);

/// <summary>
/// Request para generar diccionario de datos
/// </summary>
public record GenerateDataDictionaryRequest(string SchemaDefinition, string Language = "es");

/// <summary>
/// Response para generar diccionario de datos
/// </summary>
public record GenerateDataDictionaryResponse(bool Success, string DataDictionary, string Message);

#endregion

