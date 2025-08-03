using Microsoft.EntityFrameworkCore;
using AutoDocOps.Domain.Entities;
using AutoDocOps.Infrastructure.Configuration;

namespace AutoDocOps.Infrastructure.Data;

/// <summary>
/// Contexto de base de datos principal de la aplicación
/// </summary>
public class ApplicationDbContext : DbContext
{
    /// <summary>
    /// Constructor del contexto de base de datos
    /// </summary>
    /// <param name="options">Opciones de configuración del contexto</param>
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// Conjunto de entidades Project
    /// </summary>
    public DbSet<Project> Projects => Set<Project>();

    /// <summary>
    /// Conjunto de entidades ApiDocumentation
    /// </summary>
    public DbSet<ApiDocumentation> ApiDocumentations => Set<ApiDocumentation>();

    /// <summary>
    /// Conjunto de entidades DatabaseSchema
    /// </summary>
    public DbSet<DatabaseSchema> DatabaseSchemas => Set<DatabaseSchema>();

    /// <summary>
    /// Configura el modelo de datos
    /// </summary>
    /// <param name="modelBuilder">Constructor del modelo</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Aplicar configuraciones de entidades
        modelBuilder.ApplyConfiguration(new ProjectConfiguration());
        modelBuilder.ApplyConfiguration(new ApiDocumentationConfiguration());
        modelBuilder.ApplyConfiguration(new DatabaseSchemaConfiguration());

        // Configurar esquema por defecto
        modelBuilder.HasDefaultSchema("autodocops");

        // Configurar índices para búsqueda
        ConfigureIndexes(modelBuilder);

        // Configurar filtros globales para soft delete
        ConfigureGlobalFilters(modelBuilder);
    }

    /// <summary>
    /// Configura índices para optimizar consultas
    /// </summary>
    /// <param name="modelBuilder">Constructor del modelo</param>
    private static void ConfigureIndexes(ModelBuilder modelBuilder)
    {
        // Índices para Project
        modelBuilder.Entity<Project>()
            .HasIndex(p => p.Name)
            .HasDatabaseName("IX_Projects_Name");

        modelBuilder.Entity<Project>()
            .HasIndex(p => p.Type)
            .HasDatabaseName("IX_Projects_Type");

        modelBuilder.Entity<Project>()
            .HasIndex(p => p.Status)
            .HasDatabaseName("IX_Projects_Status");

        modelBuilder.Entity<Project>()
            .HasIndex(p => new { p.CreatedBy, p.CreatedAt })
            .HasDatabaseName("IX_Projects_CreatedBy_CreatedAt");

        // Índices para ApiDocumentation
        modelBuilder.Entity<ApiDocumentation>()
            .HasIndex(a => a.ProjectId)
            .HasDatabaseName("IX_ApiDocumentations_ProjectId");

        modelBuilder.Entity<ApiDocumentation>()
            .HasIndex(a => a.ApiName)
            .HasDatabaseName("IX_ApiDocumentations_ApiName");

        modelBuilder.Entity<ApiDocumentation>()
            .HasIndex(a => a.Version)
            .HasDatabaseName("IX_ApiDocumentations_Version");

        // Índices para DatabaseSchema
        modelBuilder.Entity<DatabaseSchema>()
            .HasIndex(d => d.ProjectId)
            .HasDatabaseName("IX_DatabaseSchemas_ProjectId");

        modelBuilder.Entity<DatabaseSchema>()
            .HasIndex(d => d.DatabaseName)
            .HasDatabaseName("IX_DatabaseSchemas_DatabaseName");

        modelBuilder.Entity<DatabaseSchema>()
            .HasIndex(d => d.SchemaName)
            .HasDatabaseName("IX_DatabaseSchemas_SchemaName");
    }

    /// <summary>
    /// Configura filtros globales para soft delete
    /// </summary>
    /// <param name="modelBuilder">Constructor del modelo</param>
    private static void ConfigureGlobalFilters(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Project>()
            .HasQueryFilter(p => p.IsActive);

        modelBuilder.Entity<ApiDocumentation>()
            .HasQueryFilter(a => a.IsActive);

        modelBuilder.Entity<DatabaseSchema>()
            .HasQueryFilter(d => d.IsActive);
    }

    /// <summary>
    /// Guarda los cambios aplicando automáticamente timestamps
    /// </summary>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Número de entidades afectadas</returns>
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return await base.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Guarda los cambios aplicando automáticamente timestamps
    /// </summary>
    /// <returns>Número de entidades afectadas</returns>
    public override int SaveChanges()
    {
        UpdateTimestamps();
        return base.SaveChanges();
    }

    /// <summary>
    /// Actualiza automáticamente los timestamps de las entidades modificadas
    /// </summary>
    private void UpdateTimestamps()
    {
        var entries = ChangeTracker.Entries<BaseEntity>()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.GetType()
                    .GetProperty("CreatedAt")?
                    .SetValue(entry.Entity, DateTime.UtcNow);
            }

            if (entry.State == EntityState.Modified)
            {
                entry.Entity.GetType()
                    .GetProperty("UpdatedAt")?
                    .SetValue(entry.Entity, DateTime.UtcNow);
            }
        }
    }
}

