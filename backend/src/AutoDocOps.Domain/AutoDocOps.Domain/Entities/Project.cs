using AutoDocOps.Domain.Enums;
using AutoDocOps.Domain.ValueObjects;

namespace AutoDocOps.Domain.Entities;

/// <summary>
/// Representa un proyecto de documentación en AutoDocOps
/// </summary>
public class Project : BaseEntity
{
    /// <summary>
    /// Nombre del proyecto
    /// </summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// Descripción del proyecto
    /// </summary>
    public string Description { get; private set; } = string.Empty;

    /// <summary>
    /// Tipo de proyecto (API .NET, Base de datos SQL Server, etc.)
    /// </summary>
    public ProjectType Type { get; private set; }

    /// <summary>
    /// Estado actual del proyecto
    /// </summary>
    public ProjectStatus Status { get; private set; }

    /// <summary>
    /// Configuración de conexión al repositorio o base de datos
    /// </summary>
    public ConnectionConfig ConnectionConfig { get; private set; } = null!;

    /// <summary>
    /// URL del repositorio Git (si aplica)
    /// </summary>
    public string? RepositoryUrl { get; private set; }

    /// <summary>
    /// Rama del repositorio a analizar
    /// </summary>
    public string? Branch { get; private set; }

    /// <summary>
    /// Idioma preferido para la documentación
    /// </summary>
    public Language PreferredLanguage { get; private set; }

    /// <summary>
    /// Configuración de generación de documentación
    /// </summary>
    public DocumentationConfig DocumentationConfig { get; private set; } = null!;

    /// <summary>
    /// Fecha del último análisis realizado
    /// </summary>
    public DateTime? LastAnalyzedAt { get; private set; }

    /// <summary>
    /// Versión actual de la documentación
    /// </summary>
    public string Version { get; private set; } = "1.0.0";

    /// <summary>
    /// Colección de documentaciones de API asociadas al proyecto
    /// </summary>
    public ICollection<ApiDocumentation> ApiDocumentations { get; private set; } = new List<ApiDocumentation>();

    /// <summary>
    /// Colección de esquemas de base de datos asociados al proyecto
    /// </summary>
    public ICollection<DatabaseSchema> DatabaseSchemas { get; private set; } = new List<DatabaseSchema>();

    /// <summary>
    /// Constructor privado para Entity Framework
    /// </summary>
    private Project() { }

    /// <summary>
    /// Crea una nueva instancia de Project
    /// </summary>
    /// <param name="name">Nombre del proyecto</param>
    /// <param name="description">Descripción del proyecto</param>
    /// <param name="type">Tipo de proyecto</param>
    /// <param name="connectionConfig">Configuración de conexión</param>
    /// <param name="preferredLanguage">Idioma preferido</param>
    /// <param name="documentationConfig">Configuración de documentación</param>
    /// <param name="createdBy">Usuario que crea el proyecto</param>
    public Project(
        string name,
        string description,
        ProjectType type,
        ConnectionConfig connectionConfig,
        Language preferredLanguage,
        DocumentationConfig documentationConfig,
        Guid createdBy)
    {
        if (name is null)
            throw new ArgumentNullException(nameof(name), "El nombre del proyecto no puede ser nulo");
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("El nombre del proyecto no puede estar vacío", nameof(name));

        if (description is null)
            throw new ArgumentNullException(nameof(description), "La descripción del proyecto no puede ser nula");
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("La descripción del proyecto no puede estar vacía", nameof(description));

        Name = name;
        Description = description;
        Type = type;
        ConnectionConfig = connectionConfig ?? throw new ArgumentNullException(nameof(connectionConfig));
        PreferredLanguage = preferredLanguage;
        DocumentationConfig = documentationConfig ?? throw new ArgumentNullException(nameof(documentationConfig));
        Status = ProjectStatus.Created;
        CreatedBy = createdBy;
        UpdatedBy = createdBy;
    }

    /// <summary>
    /// Actualiza la información básica del proyecto
    /// </summary>
    /// <param name="name">Nuevo nombre</param>
    /// <param name="description">Nueva descripción</param>
    /// <param name="updatedBy">Usuario que realiza la actualización</param>
    public void UpdateBasicInfo(string name, string description, Guid updatedBy)
    {
        if (name is null)
            throw new ArgumentNullException(nameof(name), "El nombre del proyecto no puede ser nulo");
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("El nombre del proyecto no puede estar vacío", nameof(name));

        if (description is null)
            throw new ArgumentNullException(nameof(description), "La descripción del proyecto no puede ser nula");
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("La descripción del proyecto no puede estar vacía", nameof(description));

        Name = name;
        Description = description;
        UpdateTimestamp(updatedBy);
    }

    /// <summary>
    /// Actualiza la configuración de conexión
    /// </summary>
    /// <param name="connectionConfig">Nueva configuración de conexión</param>
    /// <param name="updatedBy">Usuario que realiza la actualización</param>
    public void UpdateConnectionConfig(ConnectionConfig connectionConfig, Guid updatedBy)
    {
        ConnectionConfig = connectionConfig ?? throw new ArgumentNullException(nameof(connectionConfig));
        UpdateTimestamp(updatedBy);
    }

    /// <summary>
    /// Actualiza la configuración del repositorio
    /// </summary>
    /// <param name="repositoryUrl">URL del repositorio</param>
    /// <param name="branch">Rama a analizar</param>
    /// <param name="updatedBy">Usuario que realiza la actualización</param>
    public void UpdateRepositoryConfig(string? repositoryUrl, string? branch, Guid updatedBy)
    {
        RepositoryUrl = repositoryUrl;
        Branch = branch;
        UpdateTimestamp(updatedBy);
    }

    /// <summary>
    /// Cambia el estado del proyecto
    /// </summary>
    /// <param name="status">Nuevo estado</param>
    /// <param name="updatedBy">Usuario que realiza el cambio</param>
    public void ChangeStatus(ProjectStatus status, Guid updatedBy)
    {
        Status = status;
        UpdateTimestamp(updatedBy);
    }

    /// <summary>
    /// Marca el proyecto como analizado
    /// </summary>
    /// <param name="analyzedBy">Usuario que realizó el análisis</param>
    public void MarkAsAnalyzed(Guid analyzedBy)
    {
        LastAnalyzedAt = DateTime.UtcNow;
        Status = ProjectStatus.Analyzed;
        UpdateTimestamp(analyzedBy);
    }

    /// <summary>
    /// Actualiza la versión del proyecto
    /// </summary>
    /// <param name="version">Nueva versión</param>
    /// <param name="updatedBy">Usuario que actualiza la versión</param>
    public void UpdateVersion(string version, Guid updatedBy)
    {
        if (version is null)
            throw new ArgumentNullException(nameof(version), "La versión no puede ser nula");
        if (string.IsNullOrWhiteSpace(version))
            throw new ArgumentException("La versión no puede estar vacía", nameof(version));
        // Validar formato semántico x.y.z
        var semverRegex = "^\\d+\\.\\d+\\.\\d+$";
        if (!System.Text.RegularExpressions.Regex.IsMatch(version, semverRegex))
            throw new ArgumentException("La versión debe tener formato x.y.z", nameof(version));

        Version = version;
        UpdateTimestamp(updatedBy);
    }
}

