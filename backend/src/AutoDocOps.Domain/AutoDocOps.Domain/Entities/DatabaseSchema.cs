using AutoDocOps.Domain.Enums;

namespace AutoDocOps.Domain.Entities;

/// <summary>
/// Representa el esquema de una base de datos
/// </summary>
public class DatabaseSchema : BaseEntity
{
    /// <summary>
    /// Identificador del proyecto al que pertenece
    /// </summary>
    public Guid ProjectId { get; private set; }

    /// <summary>
    /// Proyecto al que pertenece este esquema
    /// </summary>
    public Project Project { get; private set; } = null!;

    /// <summary>
    /// Nombre de la base de datos
    /// </summary>
    public string DatabaseName { get; private set; } = string.Empty;

    /// <summary>
    /// Nombre del esquema
    /// </summary>
    public string SchemaName { get; private set; } = string.Empty;

    /// <summary>
    /// Versión del esquema
    /// </summary>
    public string Version { get; private set; } = string.Empty;

    /// <summary>
    /// Descripción del esquema
    /// </summary>
    public string Description { get; private set; } = string.Empty;

    /// <summary>
    /// Definición del esquema en formato JSON
    /// </summary>
    public string SchemaDefinition { get; private set; } = string.Empty;

    /// <summary>
    /// Diagrama ER generado
    /// </summary>
    public string? ERDiagram { get; private set; }

    /// <summary>
    /// Diccionario de datos en formato JSON
    /// </summary>
    public string? DataDictionary { get; private set; }

    /// <summary>
    /// Scripts SQL de ejemplo
    /// </summary>
    public string? SampleQueries { get; private set; }

    /// <summary>
    /// Documentación de procedimientos almacenados
    /// </summary>
    public string? StoredProceduresDoc { get; private set; }

    /// <summary>
    /// Documentación de funciones
    /// </summary>
    public string? FunctionsDoc { get; private set; }

    /// <summary>
    /// Documentación de triggers
    /// </summary>
    public string? TriggersDoc { get; private set; }

    /// <summary>
    /// Guías de uso de la base de datos
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
    /// Número total de tablas
    /// </summary>
    public int TableCount { get; private set; }

    /// <summary>
    /// Número total de vistas
    /// </summary>
    public int ViewCount { get; private set; }

    /// <summary>
    /// Número total de procedimientos almacenados
    /// </summary>
    public int StoredProcedureCount { get; private set; }

    /// <summary>
    /// Número total de funciones
    /// </summary>
    public int FunctionCount { get; private set; }

    /// <summary>
    /// Constructor privado para Entity Framework
    /// </summary>
    private DatabaseSchema() { }

    /// <summary>
    /// Crea una nueva instancia de DatabaseSchema
    /// </summary>
    /// <param name="projectId">ID del proyecto</param>
    /// <param name="databaseName">Nombre de la base de datos</param>
    /// <param name="schemaName">Nombre del esquema</param>
    /// <param name="version">Versión del esquema</param>
    /// <param name="description">Descripción</param>
    /// <param name="language">Idioma</param>
    /// <param name="createdBy">Usuario que crea el esquema</param>
    public DatabaseSchema(
        Guid projectId,
        string databaseName,
        string schemaName,
        string version,
        string description,
        Language language,
        Guid createdBy)
    {
        ProjectId = projectId;
        DatabaseName = ValidateRequiredString(databaseName, nameof(databaseName));
        SchemaName = ValidateRequiredString(schemaName, nameof(schemaName));
        Version = ValidateSemanticVersion(version, nameof(version));
        Description = ValidateRequiredString(description, nameof(description));
        Language = language;
        CreatedBy = createdBy;
        UpdatedBy = createdBy;
    }

    private static string ValidateRequiredString(string value, string paramName)
    {
        if (value == null)
            throw new ArgumentNullException(paramName);
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException($"'{paramName}' cannot be empty or whitespace.", paramName);
        return value;
    }

    private static string ValidateSemanticVersion(string version, string paramName)
    {
        if (version == null)
            throw new ArgumentNullException(paramName);
        if (string.IsNullOrWhiteSpace(version))
            throw new ArgumentException($"'{paramName}' cannot be empty or whitespace.", paramName);
        var regex = new System.Text.RegularExpressions.Regex(@"^\d+\.\d+\.\d+(-[\w\d]+)?$");
        if (!regex.IsMatch(version))
            throw new ArgumentException($"'{paramName}' must be a valid semantic version (e.g., 1.0.0, 2.1.3-beta).", paramName);
        return version;
    }

    /// <summary>
    /// Actualiza la definición del esquema
    /// </summary>
    /// <param name="schemaDefinition">Definición del esquema</param>
    /// <param name="tableCount">Número de tablas</param>
    /// <param name="viewCount">Número de vistas</param>
    /// <param name="storedProcedureCount">Número de procedimientos</param>
    /// <param name="functionCount">Número de funciones</param>
    /// <param name="updatedBy">Usuario que realiza la actualización</param>
    public void UpdateSchemaDefinition(
        string schemaDefinition,
        int tableCount,
        int viewCount,
        int storedProcedureCount,
        int functionCount,
        Guid updatedBy)
    {
        SchemaDefinition = schemaDefinition ?? throw new ArgumentNullException(nameof(schemaDefinition));
        TableCount = tableCount;
        ViewCount = viewCount;
        StoredProcedureCount = storedProcedureCount;
        FunctionCount = functionCount;
        LastGeneratedAt = DateTime.UtcNow;
        UpdateTimestamp(updatedBy);
    }

    /// <summary>
    /// Actualiza el diagrama ER
    /// </summary>
    /// <param name="erDiagram">Diagrama ER</param>
    /// <param name="updatedBy">Usuario que realiza la actualización</param>
    public void UpdateERDiagram(string erDiagram, Guid updatedBy)
    {
        ERDiagram = erDiagram;
        UpdateTimestamp(updatedBy);
    }

    /// <summary>
    /// Actualiza el diccionario de datos
    /// </summary>
    /// <param name="dataDictionary">Diccionario de datos</param>
    /// <param name="updatedBy">Usuario que realiza la actualización</param>
    public void UpdateDataDictionary(string dataDictionary, Guid updatedBy)
    {
        DataDictionary = dataDictionary;
        UpdateTimestamp(updatedBy);
    }

    /// <summary>
    /// Actualiza las consultas de ejemplo
    /// </summary>
    /// <param name="sampleQueries">Consultas de ejemplo</param>
    /// <param name="updatedBy">Usuario que realiza la actualización</param>
    public void UpdateSampleQueries(string sampleQueries, Guid updatedBy)
    {
        SampleQueries = sampleQueries;
        UpdateTimestamp(updatedBy);
    }

    /// <summary>
    /// Actualiza la documentación de procedimientos almacenados
    /// </summary>
    /// <param name="storedProceduresDoc">Documentación de procedimientos</param>
    /// <param name="updatedBy">Usuario que realiza la actualización</param>
    public void UpdateStoredProceduresDoc(string storedProceduresDoc, Guid updatedBy)
    {
        StoredProceduresDoc = storedProceduresDoc;
        UpdateTimestamp(updatedBy);
    }

    /// <summary>
    /// Actualiza la documentación de funciones
    /// </summary>
    /// <param name="functionsDoc">Documentación de funciones</param>
    /// <param name="updatedBy">Usuario que realiza la actualización</param>
    public void UpdateFunctionsDoc(string functionsDoc, Guid updatedBy)
    {
        FunctionsDoc = functionsDoc;
        UpdateTimestamp(updatedBy);
    }

    /// <summary>
    /// Actualiza la documentación de triggers
    /// </summary>
    /// <param name="triggersDoc">Documentación de triggers</param>
    /// <param name="updatedBy">Usuario que realiza la actualización</param>
    public void UpdateTriggersDoc(string triggersDoc, Guid updatedBy)
    {
        TriggersDoc = triggersDoc;
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
    /// Actualiza la información básica del esquema
    /// </summary>
    /// <param name="databaseName">Nombre de la base de datos</param>
    /// <param name="schemaName">Nombre del esquema</param>
    /// <param name="version">Versión</param>
    /// <param name="description">Descripción</param>
    /// <param name="updatedBy">Usuario que realiza la actualización</param>
    public void UpdateBasicInfo(string databaseName, string schemaName, string version, string description, Guid updatedBy)
    {
        DatabaseName = ValidateRequiredString(databaseName, nameof(databaseName));
        SchemaName = ValidateRequiredString(schemaName, nameof(schemaName));
        Version = ValidateSemanticVersion(version, nameof(version));
        Description = ValidateRequiredString(description, nameof(description));
        UpdateTimestamp(updatedBy);
    }
}

