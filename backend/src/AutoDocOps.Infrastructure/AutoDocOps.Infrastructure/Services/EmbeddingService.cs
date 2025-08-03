using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using System.Security.Cryptography;

namespace AutoDocOps.Infrastructure.Services;

/// <summary>
/// Servicio para generar embeddings vectoriales
/// </summary>
public interface IEmbeddingService
{
    /// <summary>
    /// Genera embeddings para un texto
    /// </summary>
    /// <param name="text">Texto a procesar</param>
    /// <param name="useCache">Si usar caché para el resultado</param>
    /// <returns>Vector de embeddings</returns>
    Task<float[]> GenerateEmbeddingAsync(string text, bool useCache = true);

    /// <summary>
    /// Genera embeddings para múltiples textos
    /// </summary>
    /// <param name="texts">Textos a procesar</param>
    /// <param name="useCache">Si usar caché para los resultados</param>
    /// <returns>Lista de vectores de embeddings</returns>
    Task<List<float[]>> GenerateEmbeddingsAsync(IEnumerable<string> texts, bool useCache = true);

    /// <summary>
    /// Calcula la similitud coseno entre dos vectores
    /// </summary>
    /// <param name="vector1">Primer vector</param>
    /// <param name="vector2">Segundo vector</param>
    /// <returns>Similitud coseno (0-1)</returns>
    float CalculateCosineSimilarity(float[] vector1, float[] vector2);

    /// <summary>
    /// Prepara texto para generar embeddings
    /// </summary>
    /// <param name="text">Texto original</param>
    /// <param name="maxTokens">Máximo número de tokens</param>
    /// <returns>Texto preparado</returns>
    string PrepareTextForEmbedding(string text, int maxTokens = 8000);

    /// <summary>
    /// Genera un hash del texto para usar como clave de caché
    /// </summary>
    /// <param name="text">Texto a hashear</param>
    /// <returns>Hash del texto</returns>
    string GenerateTextHash(string text);
}

/// <summary>
/// Implementación del servicio de embeddings
/// </summary>
public class EmbeddingService : IEmbeddingService
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<EmbeddingService> _logger;
    private readonly IConfiguration _configuration;
    private readonly string _embeddingModel;
    private readonly TimeSpan _cacheExpiration;

    /// <summary>
    /// Constructor del servicio de embeddings
    /// </summary>
    /// <param name="cache">Caché en memoria</param>
    /// <param name="logger">Logger</param>
    /// <param name="configuration">Configuración</param>
    public EmbeddingService(
        IMemoryCache cache,
        ILogger<EmbeddingService> logger,
        IConfiguration configuration)
    {
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

        _embeddingModel = configuration["OpenAI:EmbeddingModel"] ?? "text-embedding-3-small";
        
        var cacheExpirationConfig = configuration["Cache:EmbeddingsCacheExpiration"] ?? "01:00:00";
        _cacheExpiration = TimeSpan.Parse(cacheExpirationConfig);
    }

    /// <inheritdoc />
    public async Task<float[]> GenerateEmbeddingAsync(string text, bool useCache = true)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            throw new ArgumentException("El texto no puede estar vacío", nameof(text));
        }

        var preparedText = PrepareTextForEmbedding(text);
        var cacheKey = $"embedding_{GenerateTextHash(preparedText)}";

        // Intentar obtener del caché
        if (useCache && _cache.TryGetValue(cacheKey, out float[]? cachedEmbedding))
        {
            _logger.LogDebug("Embedding obtenido del caché para texto de {Length} caracteres", preparedText.Length);
            return cachedEmbedding!;
        }

        try
        {
            _logger.LogInformation("Generando embedding para texto de {Length} caracteres", preparedText.Length);

            // TODO: Implementar generación de embeddings con OpenAI
            await Task.Delay(1);
            
            // Mock embedding para compilación
            var embedding = new float[1536];
            var random = new Random();
            for (int i = 0; i < embedding.Length; i++)
            {
                embedding[i] = (float)(random.NextDouble() * 2 - 1); // Valores entre -1 y 1
            }

            // Guardar en caché
            if (useCache)
            {
                _cache.Set(cacheKey, embedding, _cacheExpiration);
                _logger.LogDebug("Embedding guardado en caché");
            }

            _logger.LogInformation("Embedding generado exitosamente con {Dimensions} dimensiones", embedding.Length);
            return embedding;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generando embedding para texto de {Length} caracteres", preparedText.Length);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<List<float[]>> GenerateEmbeddingsAsync(IEnumerable<string> texts, bool useCache = true)
    {
        var textList = texts.ToList();
        if (!textList.Any())
        {
            return new List<float[]>();
        }

        _logger.LogInformation("Generando embeddings para {Count} textos", textList.Count);

        var embeddings = new List<float[]>();

        // Generar embeddings para cada texto
        foreach (var text in textList)
        {
            var embedding = await GenerateEmbeddingAsync(text, useCache);
            embeddings.Add(embedding);
        }

        _logger.LogInformation("Embeddings completados para {Count} textos", textList.Count);
        return embeddings;
    }

    /// <inheritdoc />
    public float CalculateCosineSimilarity(float[] vector1, float[] vector2)
    {
        if (vector1.Length != vector2.Length)
        {
            throw new ArgumentException("Los vectores deben tener la misma dimensión");
        }

        double dotProduct = 0;
        double magnitude1 = 0;
        double magnitude2 = 0;

        for (int i = 0; i < vector1.Length; i++)
        {
            dotProduct += vector1[i] * vector2[i];
            magnitude1 += vector1[i] * vector1[i];
            magnitude2 += vector2[i] * vector2[i];
        }

        magnitude1 = Math.Sqrt(magnitude1);
        magnitude2 = Math.Sqrt(magnitude2);

        if (magnitude1 == 0 || magnitude2 == 0)
        {
            return 0;
        }

        return (float)(dotProduct / (magnitude1 * magnitude2));
    }

    /// <inheritdoc />
    public string PrepareTextForEmbedding(string text, int maxTokens = 8000)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return string.Empty;
        }

        // Limpiar el texto
        var cleanedText = text
            .Replace("\r\n", " ")
            .Replace("\n", " ")
            .Replace("\r", " ")
            .Replace("\t", " ");

        // Remover espacios múltiples
        while (cleanedText.Contains("  "))
        {
            cleanedText = cleanedText.Replace("  ", " ");
        }

        cleanedText = cleanedText.Trim();

        // Aproximación simple de tokens (1 token ≈ 4 caracteres para texto en español/inglés)
        var maxChars = maxTokens * 4;
        
        if (cleanedText.Length <= maxChars)
        {
            return cleanedText;
        }

        // Truncar manteniendo palabras completas
        var truncated = cleanedText.Substring(0, maxChars);
        var lastSpaceIndex = truncated.LastIndexOf(' ');
        
        if (lastSpaceIndex > maxChars * 0.8) // Si el último espacio está cerca del final
        {
            truncated = truncated.Substring(0, lastSpaceIndex);
        }

        _logger.LogDebug("Texto truncado de {OriginalLength} a {TruncatedLength} caracteres", 
            cleanedText.Length, truncated.Length);

        return truncated;
    }

    /// <inheritdoc />
    public string GenerateTextHash(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return string.Empty;
        }

        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(text);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
}

/// <summary>
/// Extensiones para configurar el servicio de embeddings
/// </summary>
public static class EmbeddingServiceExtensions
{
    /// <summary>
    /// Configura los servicios de embeddings
    /// </summary>
    /// <param name="services">Colección de servicios</param>
    /// <param name="configuration">Configuración</param>
    /// <returns>Colección de servicios configurada</returns>
    public static IServiceCollection AddEmbeddingServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var openAiApiKey = configuration["OpenAI:ApiKey"];
        
        if (string.IsNullOrEmpty(openAiApiKey))
        {
            throw new InvalidOperationException(
                "OpenAI API Key debe estar configurada en appsettings.json");
        }

        // Configurar caché en memoria
        services.AddMemoryCache();

        // Registrar servicio de embeddings
        services.AddScoped<IEmbeddingService, EmbeddingService>();

        return services;
    }
}

