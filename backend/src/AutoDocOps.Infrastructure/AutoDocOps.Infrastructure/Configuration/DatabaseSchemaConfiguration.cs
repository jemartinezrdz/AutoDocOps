using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AutoDocOps.Domain.Entities;
using AutoDocOps.Domain.Enums;

namespace AutoDocOps.Infrastructure.Configuration;

/// <summary>
/// Configuración de Entity Framework para la entidad DatabaseSchema
/// </summary>
public class DatabaseSchemaConfiguration : IEntityTypeConfiguration<DatabaseSchema>
{
    /// <summary>
    /// Configura la entidad DatabaseSchema
    /// </summary>
    /// <param name="builder">Constructor de la entidad</param>
    public void Configure(EntityTypeBuilder<DatabaseSchema> builder)
    {
        // Configuración de tabla
        builder.ToTable("DatabaseSchemas");

        // Clave primaria
        builder.HasKey(d => d.Id);

        // Propiedades básicas
        builder.Property(d => d.ProjectId)
            .IsRequired();

        builder.Property(d => d.DatabaseName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(d => d.SchemaName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(d => d.Version)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(d => d.Description)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(d => d.Language)
            .IsRequired()
            .HasConversion<int>();

        // Propiedades de contenido (pueden ser grandes)
        builder.Property(d => d.SchemaDefinition)
            .IsRequired()
            .HasColumnType("text");

        builder.Property(d => d.ERDiagram)
            .HasColumnType("text");

        builder.Property(d => d.DataDictionary)
            .HasColumnType("text");

        builder.Property(d => d.SampleQueries)
            .HasColumnType("text");

        builder.Property(d => d.StoredProceduresDoc)
            .HasColumnType("text");

        builder.Property(d => d.FunctionsDoc)
            .HasColumnType("text");

        builder.Property(d => d.TriggersDoc)
            .HasColumnType("text");

        builder.Property(d => d.UsageGuides)
            .HasColumnType("text");

        builder.Property(d => d.Metadata)
            .HasColumnType("text");

        // Configuración de embeddings para pgvector
        builder.Property(d => d.Embeddings)
            .HasColumnType("vector(1536)"); // OpenAI embeddings son de 1536 dimensiones

        // Propiedades de estadísticas
        builder.Property(d => d.TableCount)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(d => d.ViewCount)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(d => d.StoredProcedureCount)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(d => d.FunctionCount)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(d => d.LastGeneratedAt);

        // Propiedades de BaseEntity
        builder.Property(d => d.CreatedAt)
            .IsRequired();

        builder.Property(d => d.UpdatedAt)
            .IsRequired();

        builder.Property(d => d.CreatedBy)
            .IsRequired();

        builder.Property(d => d.UpdatedBy)
            .IsRequired();

        builder.Property(d => d.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        // Relación con Project
        builder.HasOne(d => d.Project)
            .WithMany(p => p.DatabaseSchemas)
            .HasForeignKey(d => d.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        // Índices adicionales para búsqueda semántica
        builder.HasIndex(d => d.Embeddings)
            .HasMethod("ivfflat")
            .HasDatabaseName("IX_DatabaseSchemas_Embeddings_IVFFlat");

        // Índice compuesto para búsqueda por base de datos y esquema
        builder.HasIndex(d => new { d.DatabaseName, d.SchemaName })
            .HasDatabaseName("IX_DatabaseSchemas_DatabaseName_SchemaName");
    }
}

