namespace AutoDocOps.Api.DTOs;

/// <summary>
/// Respuesta con información básica de la API
/// </summary>
public class ApiInfoResponse
{
    /// <summary>
    /// Nombre del servicio
    /// </summary>
    public string Service { get; set; } = "AutoDocOps API";

    /// <summary>
    /// Versión del servicio
    /// </summary>
    public string Version { get; set; } = "1.0.0";

    /// <summary>
    /// Estado del servicio
    /// </summary>
    public string Status { get; set; } = "Running";

    /// <summary>
    /// Timestamp de la respuesta
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

