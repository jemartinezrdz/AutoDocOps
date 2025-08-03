namespace AutoDocOps.Domain.ValueObjects;

/// <summary>
/// Configuración de conexión para repositorios y bases de datos
/// </summary>
public record ConnectionConfig
{
    /// <summary>
    /// Cadena de conexión o URL
    /// </summary>
    public string ConnectionString { get; init; } = string.Empty;

    /// <summary>
    /// Tipo de autenticación
    /// </summary>
    public string AuthenticationType { get; init; } = string.Empty;

    /// <summary>
    /// Usuario para la conexión (si aplica)
    /// </summary>
    public string? Username { get; init; }

    /// <summary>
    /// Token de acceso o contraseña encriptada
    /// </summary>
    public string? AccessToken { get; init; }

    /// <summary>
    /// Configuraciones adicionales en formato JSON
    /// </summary>
    public string? AdditionalSettings { get; init; }

    /// <summary>
    /// Indica si la conexión está habilitada
    /// </summary>
    public bool IsEnabled { get; init; } = true;

    /// <summary>
    /// Timeout de conexión en segundos
    /// </summary>
    public int TimeoutSeconds { get; init; } = 30;

    /// <summary>
    /// Constructor para crear una configuración de conexión
    /// </summary>
    /// <param name="connectionString">Cadena de conexión</param>
    /// <param name="authenticationType">Tipo de autenticación</param>
    /// <param name="username">Usuario</param>
    /// <param name="accessToken">Token de acceso</param>
    /// <param name="additionalSettings">Configuraciones adicionales</param>
    /// <param name="isEnabled">Si está habilitada</param>
    /// <param name="timeoutSeconds">Timeout en segundos</param>
    public ConnectionConfig(
        string connectionString,
        string authenticationType,
        string? username = null,
        string? accessToken = null,
        string? additionalSettings = null,
        bool isEnabled = true,
        int timeoutSeconds = 30)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentException("La cadena de conexión no puede estar vacía", nameof(connectionString));

        if (string.IsNullOrWhiteSpace(authenticationType))
            throw new ArgumentException("El tipo de autenticación no puede estar vacío", nameof(authenticationType));

        if (timeoutSeconds <= 0)
            throw new ArgumentException("El timeout debe ser mayor a cero", nameof(timeoutSeconds));

        ConnectionString = connectionString;
        AuthenticationType = authenticationType;
        Username = username;
        AccessToken = accessToken;
        AdditionalSettings = additionalSettings;
        IsEnabled = isEnabled;
        TimeoutSeconds = timeoutSeconds;
    }

    /// <summary>
    /// Crea una configuración para repositorio Git
    /// </summary>
    /// <param name="repositoryUrl">URL del repositorio</param>
    /// <param name="accessToken">Token de acceso</param>
    /// <param name="username">Usuario (opcional)</param>
    /// <returns>Configuración de conexión para Git</returns>
    public static ConnectionConfig ForGitRepository(string repositoryUrl, string accessToken, string? username = null)
    {
        return new ConnectionConfig(
            connectionString: repositoryUrl,
            authenticationType: "Bearer",
            username: username,
            accessToken: accessToken
        );
    }

    /// <summary>
    /// Crea una configuración para base de datos SQL Server
    /// </summary>
    /// <param name="server">Servidor</param>
    /// <param name="database">Base de datos</param>
    /// <param name="username">Usuario</param>
    /// <param name="password">Contraseña</param>
    /// <param name="integratedSecurity">Usar seguridad integrada</param>
    /// <returns>Configuración de conexión para SQL Server</returns>
    public static ConnectionConfig ForSqlServer(
        string server,
        string database,
        string? username = null,
        string? password = null,
        bool integratedSecurity = false)
    {
        var connectionString = integratedSecurity
            ? $"Server={server};Database={database};Integrated Security=true;TrustServerCertificate=true;"
            : $"Server={server};Database={database};User Id={username};Password={password};TrustServerCertificate=true;";

        return new ConnectionConfig(
            connectionString: connectionString,
            authenticationType: integratedSecurity ? "Windows" : "SqlServer",
            username: username,
            accessToken: password
        );
    }

    /// <summary>
    /// Crea una configuración para Supabase
    /// </summary>
    /// <param name="url">URL de Supabase</param>
    /// <param name="apiKey">API Key</param>
    /// <returns>Configuración de conexión para Supabase</returns>
    public static ConnectionConfig ForSupabase(string url, string apiKey)
    {
        return new ConnectionConfig(
            connectionString: url,
            authenticationType: "ApiKey",
            accessToken: apiKey
        );
    }
}

