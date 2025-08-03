using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Serilog;
using AutoDocOps.Api.Extensions;
using AutoDocOps.Api.Middleware;
using AutoDocOps.Application;
using AutoDocOps.Infrastructure;

namespace AutoDocOps.Api;

/// <summary>
/// Punto de entrada principal de la aplicación AutoDocOps API
/// </summary>
public class Program
{
    /// <summary>
    /// Método principal de la aplicación
    /// </summary>
    /// <param name="args">Argumentos de línea de comandos</param>
    public static async Task Main(string[] args)
    {
        // Configurar Serilog
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();

        try
        {
            Log.Information("Iniciando AutoDocOps API");

            var builder = WebApplication.CreateBuilder(args);

            // Configurar Serilog
            builder.Host.UseSerilog();

            // Configurar servicios
            ConfigureServices(builder.Services, builder.Configuration);

            var app = builder.Build();

            // Configurar pipeline
            await ConfigurePipeline(app);

            Log.Information("AutoDocOps API iniciada correctamente");
            await app.RunAsync();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Error fatal al iniciar AutoDocOps API");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    /// <summary>
    /// Configura los servicios de la aplicación
    /// </summary>
    /// <param name="services">Colección de servicios</param>
    /// <param name="configuration">Configuración de la aplicación</param>
    private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        // Configurar JSON con Source Generators
        services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
            options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        });

        // Configurar CORS
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            });
        });

        // Configurar autenticación JWT
        var jwtKey = configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key no configurada");
        var key = Encoding.ASCII.GetBytes(jwtKey);

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = configuration["Jwt:Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

        services.AddAuthorization();

        // Configurar OpenAPI/Swagger
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new() 
            { 
                Title = "AutoDocOps API", 
                Version = "v1",
                Description = "API para generación automática de documentación viva para APIs .NET y bases de datos SQL Server"
            });

            // Configurar autenticación JWT en Swagger
            c.AddSecurityDefinition("Bearer", new()
            {
                Description = "JWT Authorization header usando el esquema Bearer. Ejemplo: \"Authorization: Bearer {token}\"",
                Name = "Authorization",
                In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            c.AddSecurityRequirement(new()
            {
                {
                    new()
                    {
                        Reference = new()
                        {
                            Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });

            // Incluir comentarios XML
            var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                c.IncludeXmlComments(xmlPath);
            }
        });

        // Registrar capas de la aplicación
        services.AddApplication();
        services.AddInfrastructure(configuration);

        // Configurar health checks
        services.AddHealthChecks();
    }

    /// <summary>
    /// Configura el pipeline de la aplicación
    /// </summary>
    /// <param name="app">Aplicación web</param>
    private static async Task ConfigurePipeline(WebApplication app)
    {
        // Middleware de manejo de errores
        app.UseMiddleware<ErrorHandlingMiddleware>();

        // Configurar Swagger en desarrollo
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "AutoDocOps API v1");
                c.RoutePrefix = string.Empty; // Swagger en la raíz
            });
        }

        // Configurar HTTPS y seguridad
        app.UseHttpsRedirection();
        app.UseSecurityHeaders();

        // CORS
        app.UseCors("AllowAll");

        // Autenticación y autorización
        app.UseAuthentication();
        app.UseAuthorization();

        // Health checks
        app.MapHealthChecks("/health");

        // Configurar endpoints
        app.ConfigureEndpoints();

        // Aplicar migraciones en desarrollo
        if (app.Environment.IsDevelopment())
        {
            await app.ApplyMigrationsAsync();
        }
    }
}

/// <summary>
/// Contexto de serialización JSON para AOT
/// </summary>
[JsonSerializable(typeof(AutoDocOps.Api.DTOs.ApiInfoResponse))]
[JsonSerializable(typeof(AutoDocOps.Api.Middleware.ErrorResponse))]
[JsonSerializable(typeof(AutoDocOps.Api.Endpoints.AnalyzeDotNetRequest))]
[JsonSerializable(typeof(AutoDocOps.Api.Endpoints.AnalyzeDotNetResponse))]
[JsonSerializable(typeof(AutoDocOps.Api.Endpoints.AnalyzeSqlServerRequest))]
[JsonSerializable(typeof(AutoDocOps.Api.Endpoints.AnalyzeSqlServerResponse))]
[JsonSerializable(typeof(AutoDocOps.Api.Endpoints.GenerateGuidesRequest))]
[JsonSerializable(typeof(AutoDocOps.Api.Endpoints.GenerateGuidesResponse))]
[JsonSerializable(typeof(AutoDocOps.Api.Endpoints.GeneratePostmanRequest))]
[JsonSerializable(typeof(AutoDocOps.Api.Endpoints.GeneratePostmanResponse))]
[JsonSerializable(typeof(AutoDocOps.Api.Endpoints.GenerateTypeScriptSDKRequest))]
[JsonSerializable(typeof(AutoDocOps.Api.Endpoints.GenerateCSharpSDKRequest))]
[JsonSerializable(typeof(AutoDocOps.Api.Endpoints.GenerateSDKResponse))]
[JsonSerializable(typeof(AutoDocOps.Api.Endpoints.SemanticChatRequest))]
[JsonSerializable(typeof(AutoDocOps.Api.Endpoints.SemanticChatResponse))]
[JsonSerializable(typeof(AutoDocOps.Api.Endpoints.GenerateERDiagramRequest))]
[JsonSerializable(typeof(AutoDocOps.Api.Endpoints.GenerateERDiagramResponse))]
[JsonSerializable(typeof(AutoDocOps.Api.Endpoints.GenerateDataDictionaryRequest))]
[JsonSerializable(typeof(AutoDocOps.Api.Endpoints.GenerateDataDictionaryResponse))]
[JsonSerializable(typeof(AutoDocOps.Infrastructure.Services.CodeMetadata))]
[JsonSerializable(typeof(AutoDocOps.Infrastructure.Services.DatabaseSchemaResult))]
[JsonSerializable(typeof(AutoDocOps.Infrastructure.Services.ControllerInfo))]
[JsonSerializable(typeof(AutoDocOps.Infrastructure.Services.ModelInfo))]
[JsonSerializable(typeof(AutoDocOps.Infrastructure.Services.ServiceInfo))]
[JsonSerializable(typeof(AutoDocOps.Infrastructure.Services.TableInfo))]
[JsonSerializable(typeof(AutoDocOps.Infrastructure.Services.ColumnInfo))]
[JsonSerializable(typeof(AutoDocOps.Infrastructure.Services.IndexInfo))]
[JsonSerializable(typeof(AutoDocOps.Infrastructure.Services.ActionInfo))]
[JsonSerializable(typeof(AutoDocOps.Infrastructure.Services.ParameterInfo))]
[JsonSerializable(typeof(AutoDocOps.Infrastructure.Services.ViewInfo))]
[JsonSerializable(typeof(AutoDocOps.Infrastructure.Services.StoredProcedureInfo))]
[JsonSerializable(typeof(AutoDocOps.Infrastructure.Services.FunctionInfo))]
[JsonSerializable(typeof(AutoDocOps.Infrastructure.Services.ForeignKeyInfo))]
[JsonSerializable(typeof(AutoDocOps.Infrastructure.Services.DatabaseMetadata))]
[JsonSerializable(typeof(List<AutoDocOps.Infrastructure.Services.ControllerInfo>))]
[JsonSerializable(typeof(List<AutoDocOps.Infrastructure.Services.ModelInfo>))]
[JsonSerializable(typeof(List<AutoDocOps.Infrastructure.Services.ServiceInfo>))]
[JsonSerializable(typeof(List<AutoDocOps.Infrastructure.Services.TableInfo>))]
[JsonSerializable(typeof(List<AutoDocOps.Infrastructure.Services.ColumnInfo>))]
[JsonSerializable(typeof(List<AutoDocOps.Infrastructure.Services.IndexInfo>))]
[JsonSerializable(typeof(List<AutoDocOps.Infrastructure.Services.ActionInfo>))]
[JsonSerializable(typeof(List<AutoDocOps.Infrastructure.Services.ParameterInfo>))]
[JsonSerializable(typeof(List<AutoDocOps.Infrastructure.Services.ViewInfo>))]
[JsonSerializable(typeof(List<AutoDocOps.Infrastructure.Services.StoredProcedureInfo>))]
[JsonSerializable(typeof(List<AutoDocOps.Infrastructure.Services.FunctionInfo>))]
[JsonSerializable(typeof(List<AutoDocOps.Infrastructure.Services.ForeignKeyInfo>))]
[JsonSerializable(typeof(List<string>))]
[JsonSerializable(typeof(string))]
[JsonSerializable(typeof(int))]
[JsonSerializable(typeof(bool))]
[JsonSerializable(typeof(DateTime))]
[JsonSerializable(typeof(Guid))]
[JsonSerializable(typeof(object))]
public partial class AppJsonSerializerContext : JsonSerializerContext
{
}

