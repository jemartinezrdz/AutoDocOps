namespace AutoDocOps.Domain.ValueObjects;

/// <summary>
/// Configuración para la generación de documentación
/// </summary>
public record DocumentationConfig
{
    /// <summary>
    /// Indica si se debe generar OpenAPI 3.1
    /// </summary>
    public bool GenerateOpenApi { get; init; } = true;

    /// <summary>
    /// Indica si se debe generar Swagger UI
    /// </summary>
    public bool GenerateSwaggerUI { get; init; } = true;

    /// <summary>
    /// Indica si se debe generar colecciones de Postman
    /// </summary>
    public bool GeneratePostmanCollection { get; init; } = true;

    /// <summary>
    /// Indica si se debe generar SDK de TypeScript
    /// </summary>
    public bool GenerateTypeScriptSDK { get; init; } = true;

    /// <summary>
    /// Indica si se debe generar SDK de C#
    /// </summary>
    public bool GenerateCSharpSDK { get; init; } = true;

    /// <summary>
    /// Indica si se debe generar diagramas ER
    /// </summary>
    public bool GenerateERDiagrams { get; init; } = true;

    /// <summary>
    /// Indica si se debe generar diccionario de datos
    /// </summary>
    public bool GenerateDataDictionary { get; init; } = true;

    /// <summary>
    /// Indica si se debe generar guías de uso
    /// </summary>
    public bool GenerateUsageGuides { get; init; } = true;

    /// <summary>
    /// Indica si se debe habilitar chat semántico
    /// </summary>
    public bool EnableSemanticChat { get; init; } = true;

    /// <summary>
    /// Formato de salida para los diagramas
    /// </summary>
    public string DiagramFormat { get; init; } = "PNG";

    /// <summary>
    /// Tema visual para la documentación
    /// </summary>
    public string Theme { get; init; } = "Default";

    /// <summary>
    /// Configuraciones personalizadas en formato JSON
    /// </summary>
    public string? CustomSettings { get; init; }

    /// <summary>
    /// Indica si se debe incluir ejemplos de código
    /// </summary>
    public bool IncludeCodeExamples { get; init; } = true;

    /// <summary>
    /// Indica si se debe incluir información de versionado
    /// </summary>
    public bool IncludeVersioning { get; init; } = true;

    /// <summary>
    /// Constructor para crear una configuración de documentación
    /// </summary>
    /// <param name="generateOpenApi">Generar OpenAPI</param>
    /// <param name="generateSwaggerUI">Generar Swagger UI</param>
    /// <param name="generatePostmanCollection">Generar colección Postman</param>
    /// <param name="generateTypeScriptSDK">Generar SDK TypeScript</param>
    /// <param name="generateCSharpSDK">Generar SDK C#</param>
    /// <param name="generateERDiagrams">Generar diagramas ER</param>
    /// <param name="generateDataDictionary">Generar diccionario de datos</param>
    /// <param name="generateUsageGuides">Generar guías de uso</param>
    /// <param name="enableSemanticChat">Habilitar chat semántico</param>
    /// <param name="diagramFormat">Formato de diagramas</param>
    /// <param name="theme">Tema visual</param>
    /// <param name="customSettings">Configuraciones personalizadas</param>
    /// <param name="includeCodeExamples">Incluir ejemplos de código</param>
    /// <param name="includeVersioning">Incluir versionado</param>
    public DocumentationConfig(
        bool generateOpenApi = true,
        bool generateSwaggerUI = true,
        bool generatePostmanCollection = true,
        bool generateTypeScriptSDK = true,
        bool generateCSharpSDK = true,
        bool generateERDiagrams = true,
        bool generateDataDictionary = true,
        bool generateUsageGuides = true,
        bool enableSemanticChat = true,
        string diagramFormat = "PNG",
        string theme = "Default",
        string? customSettings = null,
        bool includeCodeExamples = true,
        bool includeVersioning = true)
    {
        if (string.IsNullOrWhiteSpace(diagramFormat))
            throw new ArgumentException("El formato de diagrama no puede estar vacío", nameof(diagramFormat));

        if (string.IsNullOrWhiteSpace(theme))
            throw new ArgumentException("El tema no puede estar vacío", nameof(theme));

        GenerateOpenApi = generateOpenApi;
        GenerateSwaggerUI = generateSwaggerUI;
        GeneratePostmanCollection = generatePostmanCollection;
        GenerateTypeScriptSDK = generateTypeScriptSDK;
        GenerateCSharpSDK = generateCSharpSDK;
        GenerateERDiagrams = generateERDiagrams;
        GenerateDataDictionary = generateDataDictionary;
        GenerateUsageGuides = generateUsageGuides;
        EnableSemanticChat = enableSemanticChat;
        DiagramFormat = diagramFormat;
        Theme = theme;
        CustomSettings = customSettings;
        IncludeCodeExamples = includeCodeExamples;
        IncludeVersioning = includeVersioning;
    }

    /// <summary>
    /// Crea una configuración completa con todas las opciones habilitadas
    /// </summary>
    /// <returns>Configuración completa</returns>
    public static DocumentationConfig Full()
    {
        return new DocumentationConfig();
    }

    /// <summary>
    /// Crea una configuración básica solo con OpenAPI y Swagger
    /// </summary>
    /// <returns>Configuración básica</returns>
    public static DocumentationConfig Basic()
    {
        return new DocumentationConfig(
            generateOpenApi: true,
            generateSwaggerUI: true,
            generatePostmanCollection: false,
            generateTypeScriptSDK: false,
            generateCSharpSDK: false,
            generateERDiagrams: false,
            generateDataDictionary: false,
            generateUsageGuides: false,
            enableSemanticChat: false
        );
    }

    /// <summary>
    /// Crea una configuración para APIs únicamente
    /// </summary>
    /// <returns>Configuración para APIs</returns>
    public static DocumentationConfig ApiOnly()
    {
        return new DocumentationConfig(
            generateOpenApi: true,
            generateSwaggerUI: true,
            generatePostmanCollection: true,
            generateTypeScriptSDK: true,
            generateCSharpSDK: true,
            generateERDiagrams: false,
            generateDataDictionary: false,
            generateUsageGuides: true,
            enableSemanticChat: true
        );
    }

    /// <summary>
    /// Crea una configuración para bases de datos únicamente
    /// </summary>
    /// <returns>Configuración para bases de datos</returns>
    public static DocumentationConfig DatabaseOnly()
    {
        return new DocumentationConfig(
            generateOpenApi: false,
            generateSwaggerUI: false,
            generatePostmanCollection: false,
            generateTypeScriptSDK: false,
            generateCSharpSDK: false,
            generateERDiagrams: true,
            generateDataDictionary: true,
            generateUsageGuides: true,
            enableSemanticChat: true
        );
    }
}

