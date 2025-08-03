namespace AutoDocOps.Domain.Enums;

/// <summary>
/// Tipos de proyecto soportados por AutoDocOps
/// </summary>
public enum ProjectType
{
    /// <summary>
    /// Proyecto de API .NET
    /// </summary>
    DotNetApi = 1,

    /// <summary>
    /// Base de datos SQL Server
    /// </summary>
    SqlServerDatabase = 2,

    /// <summary>
    /// Proyecto híbrido (API + Base de datos)
    /// </summary>
    Hybrid = 3
}

