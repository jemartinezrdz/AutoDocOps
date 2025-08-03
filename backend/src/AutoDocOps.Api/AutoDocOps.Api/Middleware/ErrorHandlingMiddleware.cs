using System.Net;
using System.Text.Json;

namespace AutoDocOps.Api.Middleware;

/// <summary>
/// Middleware para el manejo centralizado de errores
/// </summary>
public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    /// <summary>
    /// Constructor del middleware
    /// </summary>
    /// <param name="next">Siguiente middleware en el pipeline</param>
    /// <param name="logger">Logger para registrar errores</param>
    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// Invoca el middleware
    /// </summary>
    /// <param name="context">Contexto HTTP</param>
    /// <returns>Tarea asíncrona</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error no manejado en la solicitud {RequestPath}", context.Request.Path);
            await HandleExceptionAsync(context, ex);
        }
    }

    /// <summary>
    /// Maneja las excepciones y genera una respuesta apropiada
    /// </summary>
    /// <param name="context">Contexto HTTP</param>
    /// <param name="exception">Excepción ocurrida</param>
    /// <returns>Tarea asíncrona</returns>
    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var response = context.Response;
        response.ContentType = "application/json";

        var errorResponse = new ErrorResponse();

        switch (exception)
        {
            case ArgumentNullException:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse.Message = "Solicitud inválida - parámetro nulo";
                errorResponse.Details = exception.Message;
                break;

            case ArgumentException:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse.Message = "Solicitud inválida";
                errorResponse.Details = exception.Message;
                break;

            case UnauthorizedAccessException:
                response.StatusCode = (int)HttpStatusCode.Unauthorized;
                errorResponse.Message = "No autorizado";
                break;

            case KeyNotFoundException:
                response.StatusCode = (int)HttpStatusCode.NotFound;
                errorResponse.Message = "Recurso no encontrado";
                break;

            case InvalidOperationException:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse.Message = "Operación inválida";
                errorResponse.Details = exception.Message;
                break;

            default:
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                errorResponse.Message = "Error interno del servidor";
                break;
        }

        errorResponse.StatusCode = response.StatusCode;
        errorResponse.Timestamp = DateTime.UtcNow;
        errorResponse.Path = context.Request.Path;

        var jsonResponse = JsonSerializer.Serialize(errorResponse, AppJsonSerializerContext.Default.ErrorResponse);

        await response.WriteAsync(jsonResponse);
    }
}

/// <summary>
/// Respuesta de error estándar
/// </summary>
public class ErrorResponse
{
    /// <summary>
    /// Código de estado HTTP
    /// </summary>
    public int StatusCode { get; set; }

    /// <summary>
    /// Mensaje de error principal
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Detalles adicionales del error
    /// </summary>
    public string? Details { get; set; }

    /// <summary>
    /// Timestamp del error
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Ruta donde ocurrió el error
    /// </summary>
    public string? Path { get; set; }

    /// <summary>
    /// ID de correlación para tracking
    /// </summary>
    public string CorrelationId { get; set; } = Guid.NewGuid().ToString();
}

