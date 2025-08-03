namespace AutoDocOps.Domain.Enums;

/// <summary>
/// Estados posibles de un proyecto
/// </summary>
public enum ProjectStatus
{
    /// <summary>
    /// Proyecto creado pero no configurado
    /// </summary>
    Created = 1,

    /// <summary>
    /// Proyecto configurado y listo para análisis
    /// </summary>
    Configured = 2,

    /// <summary>
    /// Análisis en progreso
    /// </summary>
    Analyzing = 3,

    /// <summary>
    /// Análisis completado
    /// </summary>
    Analyzed = 4,

    /// <summary>
    /// Documentación generada
    /// </summary>
    DocumentationGenerated = 5,

    /// <summary>
    /// Error en el proceso
    /// </summary>
    Error = 6,

    /// <summary>
    /// Proyecto pausado
    /// </summary>
    Paused = 7
}

