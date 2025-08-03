using AutoDocOps.Domain.Enums;

namespace AutoDocOps.Domain.Entities;

/// <summary>
/// Representa la documentación de una API
/// </summary>
public class ApiDocumentation : BaseEntity
{
    /// <summary>
    /// Identificador del proyecto al que pertenece
    /// </summary>
    public Guid ProjectId { get; private set; }

    /// <summary>
    /// Proyecto al que pertenece esta documentación
    /// </summary>
    public Project Project { get; private set; } = null!;

    /// <summary>
    /// Nombre de la API
    /// </summary>
    public string ApiName { get; private set; } = string.Empty;

    /// <summary>
    /// Versión de la API
    /// </summary>
    public string Version { get; private set; } = string.Empty;

    /// <summary>
    /// URL base de la API
    /// </summary>
    public string BaseUrl { get; private set; } = string.Empty;

    /// <summary>
    /// Descripción de la API
    /// </summary>
    public string Description { get; private set; } = string.Empty;

    /// <summary>
    /// Especificación OpenAPI en formato JSON
    /// </summary>
    public string OpenApiSpec { get; private set; } = string.Empty;

    /// <summary>
    /// Colección de Postman en formato JSON
    /// </summary>
    public string? PostmanCollection { get; private set; }

    /// <summary>
    /// SDK de TypeScript generado
    /// </summary>
    public string? TypeScriptSDK { get; private set; }

    /// <summary>
    /// SDK de C# generado
    /// </summary>
    public string? CSharpSDK { get; private set; }

    /// <summary>
    /// Guías de uso generadas
    /// </summary>
    public string? UsageGuides { get; private set; }

    /// <summary>
    /// Idioma de la documentación
    /// </summary>
    public Language Language { get; private set; }

    /// <summary>
    /// Fecha de la última generación
    /// </summary>
    public DateTime? LastGeneratedAt { get; private set; }

    /// <summary>
    /// Embeddings para búsqueda semántica
    /// </summary>
    public float[]? Embeddings { get; private set; }

    /// <summary>
    /// Metadatos adicionales en formato JSON
    /// </summary>
    public string? Metadata { get; private set; }

    /// <summary>
    /// Constructor privado para Entity Framework
    /// </summary>
    private ApiDocumentation() { }

    /// <summary>
    /// Crea una nueva instancia de ApiDocumentation
    /// </summary>
    /// <param name="projectId">ID del proyecto</param>
    /// <param name="apiName">Nombre de la API</param>
    /// <param name="version">Versión de la API</param>
    /// <param name="baseUrl">URL base</param>
    /// <param name="description">Descripción</param>
    /// <param name="language">Idioma</param>
    /// <param name="createdBy">Usuario que crea la documentación</param>
    public ApiDocumentation(
        Guid projectId,
        string apiName,
        string version,
        string baseUrl,
        string description,
        Language language,
        Guid createdBy)
    {
        ProjectId = projectId;
        ApiName = apiName ?? throw new ArgumentNullException(nameof(apiName));
        Version = version ?? throw new ArgumentNullException(nameof(version));
        BaseUrl = baseUrl ?? throw new ArgumentNullException(nameof(baseUrl));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        Language = language;
        CreatedBy = createdBy;
        UpdatedBy = createdBy;
    }

    /// <summary>
    /// Actualiza la especificación OpenAPI
    /// </summary>
    /// <param name="openApiSpec">Especificación OpenAPI</param>
    /// <param name="updatedBy">Usuario que realiza la actualización</param>
    public void UpdateOpenApiSpec(string openApiSpec, Guid updatedBy)
    {
        OpenApiSpec = openApiSpec ?? throw new ArgumentNullException(nameof(openApiSpec));
        LastGeneratedAt = DateTime.UtcNow;
        UpdateTimestamp(updatedBy);
    }

    /// <summary>
    /// Actualiza la colección de Postman
    /// </summary>
    /// <param name="postmanCollection">Colección de Postman</param>
    /// <param name="updatedBy">Usuario que realiza la actualización</param>
    public void UpdatePostmanCollection(string postmanCollection, Guid updatedBy)
    {
        PostmanCollection = postmanCollection;
        UpdateTimestamp(updatedBy);
    }

    /// <summary>
    /// Actualiza el SDK de TypeScript
    /// </summary>
    /// <param name="typeScriptSDK">SDK de TypeScript</param>
    /// <param name="updatedBy">Usuario que realiza la actualización</param>
    public void UpdateTypeScriptSDK(string typeScriptSDK, Guid updatedBy)
    {
        TypeScriptSDK = typeScriptSDK;
        UpdateTimestamp(updatedBy);
    }

    /// <summary>
    /// Actualiza el SDK de C#
    /// </summary>
    /// <param name="cSharpSDK">SDK de C#</param>
    /// <param name="updatedBy">Usuario que realiza la actualización</param>
    public void UpdateCSharpSDK(string cSharpSDK, Guid updatedBy)
    {
        CSharpSDK = cSharpSDK;
        UpdateTimestamp(updatedBy);
    }

    /// <summary>
    /// Actualiza las guías de uso
    /// </summary>
    /// <param name="usageGuides">Guías de uso</param>
    /// <param name="updatedBy">Usuario que realiza la actualización</param>
    public void UpdateUsageGuides(string usageGuides, Guid updatedBy)
    {
        UsageGuides = usageGuides;
        UpdateTimestamp(updatedBy);
    }

    /// <summary>
    /// Actualiza los embeddings para búsqueda semántica
    /// </summary>
    /// <param name="embeddings">Embeddings</param>
    /// <param name="updatedBy">Usuario que realiza la actualización</param>
    public void UpdateEmbeddings(float[] embeddings, Guid updatedBy)
    {
        Embeddings = embeddings;
        UpdateTimestamp(updatedBy);
    }

    /// <summary>
    /// Actualiza los metadatos
    /// </summary>
    /// <param name="metadata">Metadatos en formato JSON</param>
    /// <param name="updatedBy">Usuario que realiza la actualización</param>
    public void UpdateMetadata(string metadata, Guid updatedBy)
    {
        Metadata = metadata;
        UpdateTimestamp(updatedBy);
    }

    /// <summary>
    /// Actualiza la información básica de la API
    /// </summary>
    /// <param name="apiName">Nombre de la API</param>
    /// <param name="version">Versión</param>
    /// <param name="baseUrl">URL base</param>
    /// <param name="description">Descripción</param>
    /// <param name="updatedBy">Usuario que realiza la actualización</param>
    public void UpdateBasicInfo(string apiName, string version, string baseUrl, string description, Guid updatedBy)
    {
        ApiName = apiName ?? throw new ArgumentNullException(nameof(apiName));
        Version = version ?? throw new ArgumentNullException(nameof(version));
        BaseUrl = baseUrl ?? throw new ArgumentNullException(nameof(baseUrl));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        UpdateTimestamp(updatedBy);
    }
}

