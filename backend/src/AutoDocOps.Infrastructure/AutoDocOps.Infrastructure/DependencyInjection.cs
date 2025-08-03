using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using AutoDocOps.Infrastructure.Data;
using AutoDocOps.Infrastructure.Services;
using Npgsql;

namespace AutoDocOps.Infrastructure;

/// <summary>
/// Configuración de inyección de dependencias para la capa de infraestructura
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Configura los servicios de la capa de infraestructura
    /// </summary>
    /// <param name="services">Colección de servicios</param>
    /// <param name="configuration">Configuración de la aplicación</param>
    /// <returns>Colección de servicios configurada</returns>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Configurar pool de conexiones Npgsql
        // Priorizar DATABASE_URL de variables de entorno sobre ConnectionStrings
        var connectionString = configuration["DATABASE_URL"] 
            ?? configuration.GetConnectionString("DefaultConnection") 
            ?? throw new InvalidOperationException("Connection string no configurada");

        // Configurar Npgsql con pool de conexiones optimizado
        // Temporalmente comentado para testing local
        /*
        var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
        
        // Configurar pool de conexiones
        dataSourceBuilder.ConnectionStringBuilder.Pooling = true;
        dataSourceBuilder.ConnectionStringBuilder.MinPoolSize = 5;
        dataSourceBuilder.ConnectionStringBuilder.MaxPoolSize = 100;
        dataSourceBuilder.ConnectionStringBuilder.ConnectionLifetime = 300; // 5 minutos
        dataSourceBuilder.ConnectionStringBuilder.ConnectionIdleLifetime = 60; // 1 minuto
        dataSourceBuilder.ConnectionStringBuilder.CommandTimeout = 30;
        
        // Habilitar extensión pgvector para embeddings
        dataSourceBuilder.EnableDynamicJson();
        
        var dataSource = dataSourceBuilder.Build();
        services.AddSingleton(dataSource);

        // Configurar Entity Framework con Npgsql
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(dataSource, npgsqlOptions =>
            {
                npgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(5),
                    errorCodesToAdd: null);
                
                // Configurar para usar pgvector
                npgsqlOptions.UseVector();
            });

            // Configuraciones adicionales para desarrollo
            options.EnableSensitiveDataLogging(false);
            options.EnableDetailedErrors(false);
            options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        });
        */

        // Configurar health checks para la base de datos
        // Temporalmente comentado para testing local
        // services.AddHealthChecks()
        //     .AddNpgSql(connectionString, name: "postgresql");
        
        // Health checks básicos para testing
        services.AddHealthChecks();

        // Configurar servicios de Supabase
        var supabaseUrl = configuration["Supabase:Url"];
        var supabaseKey = configuration["Supabase:Key"];
        if (!string.IsNullOrEmpty(supabaseUrl) && !string.IsNullOrEmpty(supabaseKey))
        {
            services.AddSupabaseServices(configuration);
        }

        // Configurar servicios de OpenAI y embeddings
        var openAiApiKey = configuration["OpenAI:ApiKey"];
        if (!string.IsNullOrEmpty(openAiApiKey))
        {
            services.AddEmbeddingServices(configuration);
            services.AddOpenAIServices(configuration);
            services.AddCodeAnalysisServices(configuration);
        }

        // Registrar repositorios y servicios
        // services.AddScoped<IProjectRepository, ProjectRepository>();
        // services.AddScoped<IApiDocumentationRepository, ApiDocumentationRepository>();
        // services.AddScoped<IDatabaseSchemaRepository, DatabaseSchemaRepository>();

        return services;
    }
}

