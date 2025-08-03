namespace AutoDocOps.Api.Extensions;

/// <summary>
/// Extensiones para la configuración de servicios
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Configura los servicios de la capa de aplicación
    /// </summary>
    /// <param name="services">Colección de servicios</param>
    /// <returns>Colección de servicios configurada</returns>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Configurar MediatR
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(
            typeof(AutoDocOps.Application.AssemblyReference).Assembly));

        return services;
    }
}

