using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using System.Text.Json;

namespace AutoDocOps.Infrastructure.Services;

/// <summary>
/// Servicio para interactuar con OpenAI GPT-4o-mini
/// </summary>
public interface IOpenAIService
{
    /// <summary>
    /// Analiza código .NET y genera documentación OpenAPI
    /// </summary>
    /// <param name="sourceCode">Código fuente .NET</param>
    /// <param name="language">Idioma de la documentación</param>
    /// <returns>Especificación OpenAPI generada</returns>
    Task<string> AnalyzeDotNetApiAsync(string sourceCode, string language = "es");

    /// <summary>
    /// Analiza esquema de base de datos SQL Server
    /// </summary>
    /// <param name="schemaDefinition">Definición del esquema</param>
    /// <param name="language">Idioma de la documentación</param>
    /// <returns>Documentación del esquema generada</returns>
    Task<string> AnalyzeSqlServerSchemaAsync(string schemaDefinition, string language = "es");

    /// <summary>
    /// Genera documentación de uso para APIs
    /// </summary>
    /// <param name="openApiSpec">Especificación OpenAPI</param>
    /// <param name="language">Idioma de la documentación</param>
    /// <returns>Guías de uso generadas</returns>
    Task<string> GenerateUsageGuidesAsync(string openApiSpec, string language = "es");

    /// <summary>
    /// Genera colección de Postman desde OpenAPI
    /// </summary>
    /// <param name="openApiSpec">Especificación OpenAPI</param>
    /// <param name="baseUrl">URL base de la API</param>
    /// <returns>Colección de Postman en JSON</returns>
    Task<string> GeneratePostmanCollectionAsync(string openApiSpec, string baseUrl);

    /// <summary>
    /// Genera SDK de TypeScript desde OpenAPI
    /// </summary>
    /// <param name="openApiSpec">Especificación OpenAPI</param>
    /// <param name="packageName">Nombre del paquete</param>
    /// <returns>Código TypeScript del SDK</returns>
    Task<string> GenerateTypeScriptSDKAsync(string openApiSpec, string packageName);

    /// <summary>
    /// Genera SDK de C# desde OpenAPI
    /// </summary>
    /// <param name="openApiSpec">Especificación OpenAPI</param>
    /// <param name="namespace">Namespace del SDK</param>
    /// <returns>Código C# del SDK</returns>
    Task<string> GenerateCSharpSDKAsync(string openApiSpec, string @namespace);

    /// <summary>
    /// Responde preguntas sobre documentación usando chat semántico
    /// </summary>
    /// <param name="question">Pregunta del usuario</param>
    /// <param name="context">Contexto de documentación relevante</param>
    /// <param name="language">Idioma de la respuesta</param>
    /// <returns>Respuesta generada</returns>
    Task<string> AnswerQuestionAsync(string question, string context, string language = "es");

    /// <summary>
    /// Genera diagramas ER en formato Mermaid
    /// </summary>
    /// <param name="schemaDefinition">Definición del esquema</param>
    /// <param name="language">Idioma de las etiquetas</param>
    /// <returns>Código Mermaid del diagrama ER</returns>
    Task<string> GenerateERDiagramAsync(string schemaDefinition, string language = "es");

    /// <summary>
    /// Genera diccionario de datos
    /// </summary>
    /// <param name="schemaDefinition">Definición del esquema</param>
    /// <param name="language">Idioma de la documentación</param>
    /// <returns>Diccionario de datos en formato markdown</returns>
    Task<string> GenerateDataDictionaryAsync(string schemaDefinition, string language = "es");
}

/// <summary>
/// Implementación del servicio de OpenAI
/// </summary>
public class OpenAIService : IOpenAIService
{
    private readonly ILogger<OpenAIService> _logger;
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _model;
    private readonly int _maxTokens;
    private readonly float _temperature;

    /// <summary>
    /// Constructor del servicio de OpenAI
    /// </summary>
    /// <param name="logger">Logger</param>
    /// <param name="configuration">Configuración</param>
    /// <param name="httpClient">Cliente HTTP</param>
    public OpenAIService(
        ILogger<OpenAIService> logger,
        IConfiguration configuration,
        HttpClient httpClient)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

        _apiKey = configuration["OpenAI:ApiKey"] ?? throw new InvalidOperationException("OpenAI API Key no configurada");
        _model = configuration["OpenAI:Model"] ?? "gpt-4o-mini";
        _maxTokens = int.Parse(configuration["OpenAI:MaxTokens"] ?? "4000");
        _temperature = float.Parse(configuration["OpenAI:Temperature"] ?? "0.1");

        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
    }

    /// <inheritdoc />
    public async Task<string> AnalyzeDotNetApiAsync(string sourceCode, string language = "es")
    {
        // Validaciones de entrada
        if (string.IsNullOrWhiteSpace(sourceCode))
            throw new ArgumentException("El código fuente no puede estar vacío", nameof(sourceCode));
        
        if (!IsValidLanguage(language))
            throw new ArgumentException($"Idioma '{language}' no soportado. Use 'es' o 'en'", nameof(language));

        try
        {
            _logger.LogInformation("Analizando código .NET para generar OpenAPI");

            var prompt = language == "es" 
                ? GetDotNetAnalysisPromptSpanish()
                : GetDotNetAnalysisPromptEnglish();

            var fullPrompt = $"{prompt}\n\nCódigo fuente:\n```csharp\n{sourceCode}\n```";

            var response = await CallOpenAIAsync(fullPrompt);
            
            _logger.LogInformation("Análisis de .NET completado exitosamente");
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error analizando código .NET");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<string> AnalyzeSqlServerSchemaAsync(string schemaDefinition, string language = "es")
    {
        // Validaciones de entrada
        if (string.IsNullOrWhiteSpace(schemaDefinition))
            throw new ArgumentException("La definición del esquema no puede estar vacía", nameof(schemaDefinition));
        
        if (!IsValidLanguage(language))
            throw new ArgumentException($"Idioma '{language}' no soportado. Use 'es' o 'en'", nameof(language));

        try
        {
            _logger.LogInformation("Analizando esquema SQL Server");

            var prompt = language == "es" 
                ? GetSqlAnalysisPromptSpanish()
                : GetSqlAnalysisPromptEnglish();

            var fullPrompt = $"{prompt}\n\nEsquema de base de datos:\n```sql\n{schemaDefinition}\n```";

            var response = await CallOpenAIAsync(fullPrompt);
            
            _logger.LogInformation("Análisis de SQL Server completado exitosamente");
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error analizando esquema SQL Server");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<string> GenerateUsageGuidesAsync(string openApiSpec, string language = "es")
    {
        // Validaciones de entrada
        if (string.IsNullOrWhiteSpace(openApiSpec))
            throw new ArgumentException("La especificación OpenAPI no puede estar vacía", nameof(openApiSpec));
        
        if (!IsValidLanguage(language))
            throw new ArgumentException($"Idioma '{language}' no soportado. Use 'es' o 'en'", nameof(language));

        try
        {
            _logger.LogInformation("Generando guías de uso");

            var prompt = language == "es" 
                ? GetUsageGuidesPromptSpanish()
                : GetUsageGuidesPromptEnglish();

            var fullPrompt = $"{prompt}\n\nEspecificación OpenAPI:\n```json\n{openApiSpec}\n```";

            var response = await CallOpenAIAsync(fullPrompt);
            
            _logger.LogInformation("Guías de uso generadas exitosamente");
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generando guías de uso");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<string> GeneratePostmanCollectionAsync(string openApiSpec, string baseUrl)
    {
        // Validaciones de entrada
        if (string.IsNullOrWhiteSpace(openApiSpec))
            throw new ArgumentException("La especificación OpenAPI no puede estar vacía", nameof(openApiSpec));
        
        if (string.IsNullOrWhiteSpace(baseUrl))
            throw new ArgumentException("La URL base no puede estar vacía", nameof(baseUrl));

        try
        {
            _logger.LogInformation("Generando colección de Postman");

            var prompt = GetPostmanCollectionPrompt();
            var fullPrompt = $"{prompt}\n\nURL base: {baseUrl}\n\nEspecificación OpenAPI:\n```json\n{openApiSpec}\n```";

            var response = await CallOpenAIAsync(fullPrompt);
            
            _logger.LogInformation("Colección de Postman generada exitosamente");
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generando colección de Postman");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<string> GenerateTypeScriptSDKAsync(string openApiSpec, string packageName)
    {
        // Validaciones de entrada
        if (string.IsNullOrWhiteSpace(openApiSpec))
            throw new ArgumentException("La especificación OpenAPI no puede estar vacía", nameof(openApiSpec));
        
        if (string.IsNullOrWhiteSpace(packageName))
            throw new ArgumentException("El nombre del paquete no puede estar vacío", nameof(packageName));

        try
        {
            _logger.LogInformation("Generando SDK de TypeScript");

            var prompt = GetTypeScriptSDKPrompt();
            var fullPrompt = $"{prompt}\n\nNombre del paquete: {packageName}\n\nEspecificación OpenAPI:\n```json\n{openApiSpec}\n```";

            var response = await CallOpenAIAsync(fullPrompt);
            
            _logger.LogInformation("SDK de TypeScript generado exitosamente");
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generando SDK de TypeScript");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<string> GenerateCSharpSDKAsync(string openApiSpec, string @namespace)
    {
        // Validaciones de entrada
        if (string.IsNullOrWhiteSpace(openApiSpec))
            throw new ArgumentException("La especificación OpenAPI no puede estar vacía", nameof(openApiSpec));
        
        if (string.IsNullOrWhiteSpace(@namespace))
            throw new ArgumentException("El namespace no puede estar vacío", nameof(@namespace));

        try
        {
            _logger.LogInformation("Generando SDK de C#");

            var prompt = GetCSharpSDKPrompt();
            var fullPrompt = $"{prompt}\n\nNamespace: {@namespace}\n\nEspecificación OpenAPI:\n```json\n{openApiSpec}\n```";

            var response = await CallOpenAIAsync(fullPrompt);
            
            _logger.LogInformation("SDK de C# generado exitosamente");
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generando SDK de C#");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<string> AnswerQuestionAsync(string question, string context, string language = "es")
    {
        // Validaciones de entrada
        if (string.IsNullOrWhiteSpace(question))
            throw new ArgumentException("La pregunta no puede estar vacía", nameof(question));
        
        if (string.IsNullOrWhiteSpace(context))
            throw new ArgumentException("El contexto no puede estar vacío", nameof(context));
        
        if (!IsValidLanguage(language))
            throw new ArgumentException($"Idioma '{language}' no soportado. Use 'es' o 'en'", nameof(language));

        try
        {
            _logger.LogInformation("Respondiendo pregunta con chat semántico");

            var prompt = language == "es" 
                ? GetSemanticChatPromptSpanish()
                : GetSemanticChatPromptEnglish();

            var fullPrompt = $"{prompt}\n\nContexto:\n{context}\n\nPregunta: {question}";

            var response = await CallOpenAIAsync(fullPrompt);
            
            _logger.LogInformation("Respuesta generada exitosamente");
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error respondiendo pregunta");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<string> GenerateERDiagramAsync(string schemaDefinition, string language = "es")
    {
        // Validaciones de entrada
        if (string.IsNullOrWhiteSpace(schemaDefinition))
            throw new ArgumentException("La definición del esquema no puede estar vacía", nameof(schemaDefinition));
        
        if (!IsValidLanguage(language))
            throw new ArgumentException($"Idioma '{language}' no soportado. Use 'es' o 'en'", nameof(language));

        try
        {
            _logger.LogInformation("Generando diagrama ER");

            var prompt = language == "es" 
                ? GetERDiagramPromptSpanish()
                : GetERDiagramPromptEnglish();

            var fullPrompt = $"{prompt}\n\nEsquema de base de datos:\n```sql\n{schemaDefinition}\n```";

            var response = await CallOpenAIAsync(fullPrompt);
            
            _logger.LogInformation("Diagrama ER generado exitosamente");
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generando diagrama ER");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<string> GenerateDataDictionaryAsync(string schemaDefinition, string language = "es")
    {
        // Validaciones de entrada
        if (string.IsNullOrWhiteSpace(schemaDefinition))
            throw new ArgumentException("La definición del esquema no puede estar vacía", nameof(schemaDefinition));
        
        if (!IsValidLanguage(language))
            throw new ArgumentException($"Idioma '{language}' no soportado. Use 'es' o 'en'", nameof(language));

        try
        {
            _logger.LogInformation("Generando diccionario de datos");

            var prompt = language == "es" 
                ? GetDataDictionaryPromptSpanish()
                : GetDataDictionaryPromptEnglish();

            var fullPrompt = $"{prompt}\n\nEsquema de base de datos:\n```sql\n{schemaDefinition}\n```";

            var response = await CallOpenAIAsync(fullPrompt);
            
            _logger.LogInformation("Diccionario de datos generado exitosamente");
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generando diccionario de datos");
            throw;
        }
    }

    /// <summary>
    /// Llama a la API de OpenAI
    /// </summary>
    /// <param name="prompt">Prompt para enviar</param>
    /// <returns>Respuesta de OpenAI</returns>
    private async Task<string> CallOpenAIAsync(string prompt)
    {
        var requestBody = new
        {
            model = _model,
            messages = new[]
            {
                new { role = "user", content = prompt }
            },
            max_tokens = _maxTokens,
            temperature = _temperature
        };

        var json = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);
        response.EnsureSuccessStatusCode();

        var responseJson = await response.Content.ReadAsStringAsync();
        var responseObj = JsonSerializer.Deserialize<JsonElement>(responseJson);

        return responseObj
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString() ?? string.Empty;
    }

    #region Prompts en Español

    private string GetDotNetAnalysisPromptSpanish()
    {
        return @"Eres un experto en análisis de APIs .NET y generación de documentación OpenAPI 3.1.

Tu tarea es analizar el código fuente .NET proporcionado y generar una especificación OpenAPI 3.1 completa y precisa.

Instrucciones:
1. Analiza todos los controladores, endpoints, modelos y DTOs
2. Identifica parámetros, tipos de respuesta, códigos de estado HTTP
3. Extrae comentarios XML para descripciones
4. Genera una especificación OpenAPI 3.1 válida en JSON
5. Incluye ejemplos de request/response cuando sea posible
6. Documenta esquemas de autenticación si están presentes
7. Usa descripciones claras en español

Formato de salida: JSON válido de OpenAPI 3.1";
    }

    private string GetSqlAnalysisPromptSpanish()
    {
        return @"Eres un experto en análisis de bases de datos SQL Server y documentación de esquemas.

Tu tarea es analizar el esquema de base de datos proporcionado y generar documentación completa.

Instrucciones:
1. Analiza todas las tablas, columnas, tipos de datos, restricciones
2. Identifica relaciones entre tablas (FK, PK)
3. Documenta índices, triggers, stored procedures, funciones
4. Extrae comentarios y descripciones existentes
5. Genera documentación estructurada en formato markdown
6. Incluye estadísticas y metadatos relevantes
7. Usa descripciones claras en español

Formato de salida: Markdown estructurado con secciones claras";
    }

    private string GetUsageGuidesPromptSpanish()
    {
        return @"Eres un experto en documentación técnica y creación de guías de uso para APIs.

Tu tarea es generar guías de uso completas y fáciles de seguir basadas en la especificación OpenAPI.

Instrucciones:
1. Crea una guía de inicio rápido
2. Documenta autenticación y autorización
3. Proporciona ejemplos de código para cada endpoint
4. Incluye casos de uso comunes
5. Explica manejo de errores y códigos de respuesta
6. Agrega mejores prácticas y recomendaciones
7. Usa un lenguaje claro y accesible en español

Formato de salida: Markdown con ejemplos de código y explicaciones detalladas";
    }

    private string GetSemanticChatPromptSpanish()
    {
        return @"Eres un asistente experto en documentación técnica que ayuda a los desarrolladores a entender APIs y bases de datos.

Tu tarea es responder preguntas específicas usando el contexto de documentación proporcionado.

Instrucciones:
1. Analiza cuidadosamente el contexto proporcionado
2. Responde de manera precisa y útil
3. Incluye ejemplos de código cuando sea relevante
4. Proporciona enlaces o referencias a secciones específicas
5. Si no tienes información suficiente, indícalo claramente
6. Mantén un tono profesional pero accesible
7. Responde en español de manera clara y concisa

Formato de salida: Respuesta directa con ejemplos y referencias cuando sea apropiado";
    }

    private string GetERDiagramPromptSpanish()
    {
        return @"Eres un experto en diseño de bases de datos y generación de diagramas ER.

Tu tarea es generar un diagrama ER en formato Mermaid basado en el esquema de base de datos.

Instrucciones:
1. Analiza todas las tablas y sus relaciones
2. Identifica claves primarias y foráneas
3. Determina cardinalidades de las relaciones
4. Genera código Mermaid válido para el diagrama ER
5. Usa nombres descriptivos en español
6. Incluye tipos de datos principales
7. Organiza el diagrama de manera clara y legible

Formato de salida: Código Mermaid válido para diagrama ER";
    }

    private string GetDataDictionaryPromptSpanish()
    {
        return @"Eres un experto en documentación de bases de datos y creación de diccionarios de datos.

Tu tarea es generar un diccionario de datos completo basado en el esquema de base de datos.

Instrucciones:
1. Documenta cada tabla con su propósito
2. Lista todas las columnas con tipos, restricciones y descripciones
3. Identifica relaciones y dependencias
4. Incluye índices, triggers y stored procedures
5. Agrega ejemplos de datos cuando sea útil
6. Organiza la información de manera estructurada
7. Usa terminología clara en español

Formato de salida: Markdown estructurado con tablas y secciones organizadas";
    }

    #endregion

    #region Prompts en Inglés

    private string GetDotNetAnalysisPromptEnglish()
    {
        return @"You are an expert in .NET API analysis and OpenAPI 3.1 documentation generation.

Your task is to analyze the provided .NET source code and generate a complete and accurate OpenAPI 3.1 specification.

Instructions:
1. Analyze all controllers, endpoints, models, and DTOs
2. Identify parameters, response types, HTTP status codes
3. Extract XML comments for descriptions
4. Generate a valid OpenAPI 3.1 specification in JSON
5. Include request/response examples when possible
6. Document authentication schemes if present
7. Use clear descriptions in English

Output format: Valid OpenAPI 3.1 JSON";
    }

    private string GetSqlAnalysisPromptEnglish()
    {
        return @"You are an expert in SQL Server database analysis and schema documentation.

Your task is to analyze the provided database schema and generate comprehensive documentation.

Instructions:
1. Analyze all tables, columns, data types, constraints
2. Identify relationships between tables (FK, PK)
3. Document indexes, triggers, stored procedures, functions
4. Extract existing comments and descriptions
5. Generate structured documentation in markdown format
6. Include relevant statistics and metadata
7. Use clear descriptions in English

Output format: Structured markdown with clear sections";
    }

    private string GetUsageGuidesPromptEnglish()
    {
        return @"You are an expert in technical documentation and API usage guide creation.

Your task is to generate comprehensive and easy-to-follow usage guides based on the OpenAPI specification.

Instructions:
1. Create a quick start guide
2. Document authentication and authorization
3. Provide code examples for each endpoint
4. Include common use cases
5. Explain error handling and response codes
6. Add best practices and recommendations
7. Use clear and accessible language in English

Output format: Markdown with code examples and detailed explanations";
    }

    private string GetSemanticChatPromptEnglish()
    {
        return @"You are an expert technical documentation assistant that helps developers understand APIs and databases.

Your task is to answer specific questions using the provided documentation context.

Instructions:
1. Carefully analyze the provided context
2. Respond accurately and helpfully
3. Include code examples when relevant
4. Provide links or references to specific sections
5. If you don't have sufficient information, clearly indicate so
6. Maintain a professional but accessible tone
7. Respond in English clearly and concisely

Output format: Direct response with examples and references when appropriate";
    }

    private string GetERDiagramPromptEnglish()
    {
        return @"You are an expert in database design and ER diagram generation.

Your task is to generate an ER diagram in Mermaid format based on the database schema.

Instructions:
1. Analyze all tables and their relationships
2. Identify primary and foreign keys
3. Determine relationship cardinalities
4. Generate valid Mermaid code for the ER diagram
5. Use descriptive names in English
6. Include main data types
7. Organize the diagram clearly and legibly

Output format: Valid Mermaid code for ER diagram";
    }

    private string GetDataDictionaryPromptEnglish()
    {
        return @"You are an expert in database documentation and data dictionary creation.

Your task is to generate a comprehensive data dictionary based on the database schema.

Instructions:
1. Document each table with its purpose
2. List all columns with types, constraints, and descriptions
3. Identify relationships and dependencies
4. Include indexes, triggers, and stored procedures
5. Add data examples when useful
6. Organize information in a structured manner
7. Use clear terminology in English

Output format: Structured markdown with organized tables and sections";
    }

    #endregion

    #region Prompts Específicos

    private string GetPostmanCollectionPrompt()
    {
        return @"You are an expert in API testing and Postman collection generation.

Your task is to generate a complete Postman collection based on the OpenAPI specification.

Instructions:
1. Create a collection with all endpoints from the OpenAPI spec
2. Include proper request methods, headers, and parameters
3. Add example request bodies and query parameters
4. Set up authentication if specified
5. Include tests for common response codes
6. Organize requests in logical folders
7. Add collection-level variables for base URL

Output format: Valid Postman Collection v2.1 JSON";
    }

    private string GetTypeScriptSDKPrompt()
    {
        return @"You are an expert in TypeScript development and SDK generation.

Your task is to generate a complete TypeScript SDK based on the OpenAPI specification.

Instructions:
1. Generate TypeScript interfaces for all models
2. Create service classes for each endpoint group
3. Include proper error handling and response types
4. Add JSDoc comments for all public methods
5. Use modern TypeScript features (async/await, generics)
6. Include authentication handling
7. Generate a main index file with exports

Output format: Complete TypeScript SDK code with proper structure";
    }

    private string GetCSharpSDKPrompt()
    {
        return @"You are an expert in C# development and SDK generation.

Your task is to generate a complete C# SDK based on the OpenAPI specification.

Instructions:
1. Generate C# classes for all models with proper attributes
2. Create service classes for each endpoint group
3. Include proper error handling and response types
4. Add XML documentation for all public methods
5. Use modern C# features (async/await, nullable reference types)
6. Include authentication handling
7. Generate proper project structure

Output format: Complete C# SDK code with proper structure and documentation";
    }

    #endregion

    #region Private Helper Methods

    /// <summary>
    /// Valida si el idioma es soportado
    /// </summary>
    /// <param name="language">Idioma a validar</param>
    /// <returns>True si es válido, false en caso contrario</returns>
    private static bool IsValidLanguage(string language)
    {
        return language is "es" or "en";
    }

    #endregion
}

/// <summary>
/// Extensiones para configurar el servicio de OpenAI
/// </summary>
public static class OpenAIServiceExtensions
{
    /// <summary>
    /// Configura los servicios de OpenAI
    /// </summary>
    /// <param name="services">Colección de servicios</param>
    /// <param name="configuration">Configuración</param>
    /// <returns>Colección de servicios configurada</returns>
    public static IServiceCollection AddOpenAIServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var openAiApiKey = configuration["OpenAI:ApiKey"];
        
        if (string.IsNullOrEmpty(openAiApiKey))
        {
            throw new InvalidOperationException(
                "OpenAI API Key debe estar configurada en appsettings.json");
        }

        // Configurar HttpClient para OpenAI
        services.AddHttpClient<IOpenAIService, OpenAIService>();

        // Registrar servicio de OpenAI
        services.AddScoped<IOpenAIService, OpenAIService>();

        return services;
    }
}

