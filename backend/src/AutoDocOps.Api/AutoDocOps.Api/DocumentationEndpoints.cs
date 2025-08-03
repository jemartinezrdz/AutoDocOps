
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

