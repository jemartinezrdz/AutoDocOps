using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using System.Text.RegularExpressions;

namespace AutoDocOps.Infrastructure.Services;

/// <summary>
/// Servicio para análisis de código y extracción de metadatos
/// </summary>
public interface ICodeAnalysisService
{
    /// <summary>
    /// Analiza un repositorio .NET desde una URL
    /// </summary>
    /// <param name="repositoryUrl">URL del repositorio</param>
    /// <param name="branch">Rama a analizar</param>
    /// <returns>Código fuente extraído y metadatos</returns>
    Task<CodeAnalysisResult> AnalyzeRepositoryAsync(string repositoryUrl, string branch = "main");

    /// <summary>
    /// Analiza código .NET local
    /// </summary>
    /// <param name="sourceCode">Código fuente</param>
    /// <returns>Metadatos extraídos</returns>
    Task<CodeMetadata> AnalyzeDotNetCodeAsync(string sourceCode);

    /// <summary>
    /// Extrae esquema de base de datos SQL Server
    /// </summary>
    /// <param name="connectionString">Cadena de conexión</param>
    /// <returns>Definición del esquema</returns>
    Task<DatabaseSchemaResult> ExtractSqlServerSchemaAsync(string connectionString);

    /// <summary>
    /// Valida una cadena de conexión SQL Server
    /// </summary>
    /// <param name="connectionString">Cadena de conexión</param>
    /// <returns>True si es válida</returns>
    Task<bool> ValidateConnectionStringAsync(string connectionString);

    /// <summary>
    /// Extrae metadatos de un proyecto .NET
    /// </summary>
    /// <param name="projectContent">Contenido del archivo .csproj</param>
    /// <returns>Metadatos del proyecto</returns>
    ProjectMetadata ExtractProjectMetadata(string projectContent);
}

/// <summary>
/// Resultado del análisis de código
/// </summary>
public class CodeAnalysisResult
{
    /// <summary>
    /// Código fuente analizado
    /// </summary>
    public string SourceCode { get; set; } = string.Empty;
    
    /// <summary>
    /// Metadatos extraídos del código
    /// </summary>
    public CodeMetadata Metadata { get; set; } = new();
    
    /// <summary>
    /// Lista de controladores encontrados
    /// </summary>
    public List<string> Controllers { get; set; } = new();
    
    /// <summary>
    /// Lista de modelos encontrados
    /// </summary>
    public List<string> Models { get; set; } = new();
    
    /// <summary>
    /// Lista de servicios encontrados
    /// </summary>
    public List<string> Services { get; set; } = new();
    
    /// <summary>
    /// Información del proyecto
    /// </summary>
    public ProjectMetadata ProjectInfo { get; set; } = new();
}

/// <summary>
/// Metadatos de código .NET
/// </summary>
public class CodeMetadata
{
    /// <summary>
    /// Lista de controladores con información detallada
    /// </summary>
    public List<ControllerInfo> Controllers { get; set; } = new();
    
    /// <summary>
    /// Lista de modelos con información detallada
    /// </summary>
    public List<ModelInfo> Models { get; set; } = new();
    
    /// <summary>
    /// Lista de servicios con información detallada
    /// </summary>
    public List<ServiceInfo> Services { get; set; } = new();
    
    /// <summary>
    /// Lista de dependencias del proyecto
    /// </summary>
    public List<string> Dependencies { get; set; } = new();
    
    /// <summary>
    /// Framework utilizado (.NET Core, .NET Framework, etc.)
    /// </summary>
    public string Framework { get; set; } = string.Empty;
    
    /// <summary>
    /// Versión del framework
    /// </summary>
    public string Version { get; set; } = string.Empty;
}

/// <summary>
/// Información de controlador
/// </summary>
public class ControllerInfo
{
    public string Name { get; set; } = string.Empty;
    public string Route { get; set; } = string.Empty;
    public List<ActionInfo> Actions { get; set; } = new();
    public List<string> Attributes { get; set; } = new();
}

/// <summary>
/// Información de acción
/// </summary>
public class ActionInfo
{
    public string Name { get; set; } = string.Empty;
    public string HttpMethod { get; set; } = string.Empty;
    public string Route { get; set; } = string.Empty;
    public List<ParameterInfo> Parameters { get; set; } = new();
    public string ReturnType { get; set; } = string.Empty;
    public List<string> Attributes { get; set; } = new();
    public string Summary { get; set; } = string.Empty;
}

/// <summary>
/// Información de parámetro
/// </summary>
public class ParameterInfo
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public bool IsRequired { get; set; }
    public string Source { get; set; } = string.Empty; // Query, Body, Route, Header
    public string Description { get; set; } = string.Empty;
}

/// <summary>
/// Información de modelo
/// </summary>
public class ModelInfo
{
    public string Name { get; set; } = string.Empty;
    public List<PropertyInfo> Properties { get; set; } = new();
    public List<string> Attributes { get; set; } = new();
    public string Summary { get; set; } = string.Empty;
}

/// <summary>
/// Información de propiedad
/// </summary>
public class PropertyInfo
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public bool IsRequired { get; set; }
    public bool IsNullable { get; set; }
    public List<string> Attributes { get; set; } = new();
    public string Description { get; set; } = string.Empty;
}

/// <summary>
/// Información de servicio
/// </summary>
public class ServiceInfo
{
    public string Name { get; set; } = string.Empty;
    public string Interface { get; set; } = string.Empty;
    public List<MethodInfo> Methods { get; set; } = new();
}

/// <summary>
/// Información de método
/// </summary>
public class MethodInfo
{
    public string Name { get; set; } = string.Empty;
    public string ReturnType { get; set; } = string.Empty;
    public List<ParameterInfo> Parameters { get; set; } = new();
    public string Summary { get; set; } = string.Empty;
}

/// <summary>
/// Metadatos de proyecto
/// </summary>
public class ProjectMetadata
{
    public string Name { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string Framework { get; set; } = string.Empty;
    public List<string> PackageReferences { get; set; } = new();
    public string Description { get; set; } = string.Empty;
    public string Authors { get; set; } = string.Empty;
}

/// <summary>
/// Resultado del análisis de esquema de base de datos
/// </summary>
public class DatabaseSchemaResult
{
    public string SchemaDefinition { get; set; } = string.Empty;
    public List<TableInfo> Tables { get; set; } = new();
    public List<ViewInfo> Views { get; set; } = new();
    public List<StoredProcedureInfo> StoredProcedures { get; set; } = new();
    public List<FunctionInfo> Functions { get; set; } = new();
    public DatabaseMetadata Metadata { get; set; } = new();
}

/// <summary>
/// Información de tabla
/// </summary>
public class TableInfo
{
    public string Name { get; set; } = string.Empty;
    public string Schema { get; set; } = string.Empty;
    public List<ColumnInfo> Columns { get; set; } = new();
    public List<IndexInfo> Indexes { get; set; } = new();
    public List<ForeignKeyInfo> ForeignKeys { get; set; } = new();
    public string Description { get; set; } = string.Empty;
}

/// <summary>
/// Información de columna
/// </summary>
public class ColumnInfo
{
    public string Name { get; set; } = string.Empty;
    public string DataType { get; set; } = string.Empty;
    public bool IsNullable { get; set; }
    public bool IsPrimaryKey { get; set; }
    public bool IsIdentity { get; set; }
    public string DefaultValue { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

/// <summary>
/// Información de índice
/// </summary>
public class IndexInfo
{
    public string Name { get; set; } = string.Empty;
    public List<string> Columns { get; set; } = new();
    public bool IsUnique { get; set; }
    public bool IsClustered { get; set; }
}

/// <summary>
/// Información de clave foránea
/// </summary>
public class ForeignKeyInfo
{
    public string Name { get; set; } = string.Empty;
    public string Column { get; set; } = string.Empty;
    public string ReferencedTable { get; set; } = string.Empty;
    public string ReferencedColumn { get; set; } = string.Empty;
}

/// <summary>
/// Información de vista
/// </summary>
public class ViewInfo
{
    public string Name { get; set; } = string.Empty;
    public string Schema { get; set; } = string.Empty;
    public string Definition { get; set; } = string.Empty;
    public List<ColumnInfo> Columns { get; set; } = new();
}

/// <summary>
/// Información de stored procedure
/// </summary>
public class StoredProcedureInfo
{
    public string Name { get; set; } = string.Empty;
    public string Schema { get; set; } = string.Empty;
    public List<ParameterInfo> Parameters { get; set; } = new();
    public string Definition { get; set; } = string.Empty;
}

/// <summary>
/// Información de función
/// </summary>
public class FunctionInfo
{
    public string Name { get; set; } = string.Empty;
    public string Schema { get; set; } = string.Empty;
    public string ReturnType { get; set; } = string.Empty;
    public List<ParameterInfo> Parameters { get; set; } = new();
    public string Definition { get; set; } = string.Empty;
}

/// <summary>
/// Metadatos de base de datos
/// </summary>
public class DatabaseMetadata
{
    public string DatabaseName { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public int TableCount { get; set; }
    public int ViewCount { get; set; }
    public int StoredProcedureCount { get; set; }
    public int FunctionCount { get; set; }
    public DateTime LastAnalyzed { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Implementación del servicio de análisis de código
/// </summary>
public class CodeAnalysisService : ICodeAnalysisService
{
    private readonly ILogger<CodeAnalysisService> _logger;
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Constructor del servicio de análisis de código
    /// </summary>
    /// <param name="logger">Logger</param>
    /// <param name="configuration">Configuración</param>
    /// <param name="httpClient">Cliente HTTP</param>
    public CodeAnalysisService(
        ILogger<CodeAnalysisService> logger,
        IConfiguration configuration,
        HttpClient httpClient)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    /// <inheritdoc />
    public async Task<CodeAnalysisResult> AnalyzeRepositoryAsync(string repositoryUrl, string branch = "main")
    {
        try
        {
            _logger.LogInformation("Analizando repositorio: {RepositoryUrl}", repositoryUrl);

            // TODO: Implementar clonado y análisis de repositorio Git
            await Task.Delay(1);

            // Mock para compilación
            var result = new CodeAnalysisResult
            {
                SourceCode = "// Código fuente extraído del repositorio",
                Metadata = new CodeMetadata
                {
                    Framework = ".NET 8",
                    Version = "1.0.0"
                },
                ProjectInfo = new ProjectMetadata
                {
                    Name = "API Project",
                    Framework = ".NET 8",
                    Version = "1.0.0"
                }
            };

            _logger.LogInformation("Análisis de repositorio completado");
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error analizando repositorio: {RepositoryUrl}", repositoryUrl);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<CodeMetadata> AnalyzeDotNetCodeAsync(string sourceCode)
    {
        try
        {
            _logger.LogInformation("Analizando código .NET");

            await Task.Delay(1);

            var metadata = new CodeMetadata();

            // Extraer controladores
            metadata.Controllers = ExtractControllers(sourceCode);

            // Extraer modelos
            metadata.Models = ExtractModels(sourceCode);

            // Extraer servicios
            metadata.Services = ExtractServices(sourceCode);

            // Extraer dependencias
            metadata.Dependencies = ExtractDependencies(sourceCode);

            _logger.LogInformation("Análisis de código completado. Controladores: {Controllers}, Modelos: {Models}", 
                metadata.Controllers.Count, metadata.Models.Count);

            return metadata;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error analizando código .NET");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<DatabaseSchemaResult> ExtractSqlServerSchemaAsync(string connectionString)
    {
        try
        {
            _logger.LogInformation("Extrayendo esquema de SQL Server");

            // TODO: Implementar extracción real de esquema SQL Server
            await Task.Delay(1);

            var result = new DatabaseSchemaResult
            {
                SchemaDefinition = "-- Esquema extraído de SQL Server",
                Metadata = new DatabaseMetadata
                {
                    DatabaseName = "SampleDB",
                    Version = "SQL Server 2022"
                }
            };

            _logger.LogInformation("Extracción de esquema completada");
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extrayendo esquema SQL Server");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<bool> ValidateConnectionStringAsync(string connectionString)
    {
        try
        {
            _logger.LogInformation("Validando cadena de conexión");

            // TODO: Implementar validación real de conexión
            await Task.Delay(1);

            // Validación básica de formato
            return !string.IsNullOrEmpty(connectionString) && 
                   connectionString.Contains("Server=") || connectionString.Contains("Data Source=");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validando cadena de conexión");
            return false;
        }
    }

    /// <inheritdoc />
    public ProjectMetadata ExtractProjectMetadata(string projectContent)
    {
        try
        {
            var metadata = new ProjectMetadata();

            // Extraer nombre del proyecto
            var nameMatch = Regex.Match(projectContent, @"<AssemblyName>(.*?)</AssemblyName>");
            if (nameMatch.Success)
                metadata.Name = nameMatch.Groups[1].Value;

            // Extraer versión
            var versionMatch = Regex.Match(projectContent, @"<Version>(.*?)</Version>");
            if (versionMatch.Success)
                metadata.Version = versionMatch.Groups[1].Value;

            // Extraer framework
            var frameworkMatch = Regex.Match(projectContent, @"<TargetFramework>(.*?)</TargetFramework>");
            if (frameworkMatch.Success)
                metadata.Framework = frameworkMatch.Groups[1].Value;

            // Extraer referencias de paquetes
            var packageMatches = Regex.Matches(projectContent, @"<PackageReference Include=""(.*?)""");
            foreach (Match match in packageMatches)
            {
                metadata.PackageReferences.Add(match.Groups[1].Value);
            }

            return metadata;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extrayendo metadatos del proyecto");
            return new ProjectMetadata();
        }
    }

    #region Métodos privados de extracción

    private List<ControllerInfo> ExtractControllers(string sourceCode)
    {
        var controllers = new List<ControllerInfo>();

        // TODO: Implementar extracción real de controladores usando Roslyn
        // Por ahora, extracción básica con regex

        var controllerMatches = Regex.Matches(sourceCode, 
            @"public class (\w+Controller)\s*:\s*ControllerBase", 
            RegexOptions.Multiline);

        foreach (Match match in controllerMatches)
        {
            var controller = new ControllerInfo
            {
                Name = match.Groups[1].Value
            };

            // Extraer ruta del controlador
            var routeMatch = Regex.Match(sourceCode, 
                $@"\[Route\(""([^""]*)""\)\].*?public class {controller.Name}",
                RegexOptions.Singleline);
            
            if (routeMatch.Success)
                controller.Route = routeMatch.Groups[1].Value;

            controllers.Add(controller);
        }

        return controllers;
    }

    private List<ModelInfo> ExtractModels(string sourceCode)
    {
        var models = new List<ModelInfo>();

        // TODO: Implementar extracción real de modelos usando Roslyn
        var modelMatches = Regex.Matches(sourceCode, 
            @"public class (\w+)(?!\s*:\s*ControllerBase)", 
            RegexOptions.Multiline);

        foreach (Match match in modelMatches)
        {
            var model = new ModelInfo
            {
                Name = match.Groups[1].Value
            };

            models.Add(model);
        }

        return models;
    }

    private List<ServiceInfo> ExtractServices(string sourceCode)
    {
        var services = new List<ServiceInfo>();

        // TODO: Implementar extracción real de servicios
        var serviceMatches = Regex.Matches(sourceCode, 
            @"public class (\w+Service)", 
            RegexOptions.Multiline);

        foreach (Match match in serviceMatches)
        {
            var service = new ServiceInfo
            {
                Name = match.Groups[1].Value
            };

            services.Add(service);
        }

        return services;
    }

    private List<string> ExtractDependencies(string sourceCode)
    {
        var dependencies = new List<string>();

        // Extraer using statements
        var usingMatches = Regex.Matches(sourceCode, @"using ([^;]+);");
        foreach (Match match in usingMatches)
        {
            dependencies.Add(match.Groups[1].Value);
        }

        return dependencies.Distinct().ToList();
    }

    #endregion
}

/// <summary>
/// Extensiones para configurar el servicio de análisis de código
/// </summary>
public static class CodeAnalysisServiceExtensions
{
    /// <summary>
    /// Configura los servicios de análisis de código
    /// </summary>
    /// <param name="services">Colección de servicios</param>
    /// <param name="configuration">Configuración</param>
    /// <returns>Colección de servicios configurada</returns>
    public static IServiceCollection AddCodeAnalysisServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Configurar HttpClient para análisis de código
        services.AddHttpClient<ICodeAnalysisService, CodeAnalysisService>();

        // Registrar servicio de análisis de código
        services.AddScoped<ICodeAnalysisService, CodeAnalysisService>();

        return services;
    }
}

