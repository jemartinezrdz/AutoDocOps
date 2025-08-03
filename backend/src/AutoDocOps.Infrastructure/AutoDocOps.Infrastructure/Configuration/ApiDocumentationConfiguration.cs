using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AutoDocOps.Domain.Entities;
using AutoDocOps.Domain.Enums;

namespace AutoDocOps.Infrastructure.Configuration;

/// <summary>
/// Configuración de Entity Framework para la entidad ApiDocumentation
/// </summary>
public class ApiDocumentationConfiguration : IEntityTypeConfiguration<ApiDocumentation>
{
    /// <summary>
    /// Configura la entidad ApiDocumentation
    /// </summary>
    /// <param name="builder">Constructor de la entidad</param>
    public void Configure(EntityTypeBuilder<ApiDocumentation> builder)
    {
        // Configuración de tabla
        builder.ToTable("ApiDocumentations");

        // Clave primaria
        builder.HasKey(a => a.Id);

        // Propiedades básicas
        builder.Property(a => a.ProjectId)
            .IsRequired();

        builder.Property(a => a.ApiName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(a => a.Version)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(a => a.BaseUrl)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(a => a.Description)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(a => a.Language)
            .IsRequired()
            .HasConversion<int>();

        // Propiedades de contenido (pueden ser grandes)
        builder.Property(a => a.OpenApiSpec)
            .IsRequired()
            .HasColumnType("text");

        builder.Property(a => a.PostmanCollection)
            .HasColumnType("text");

        builder.Property(a => a.TypeScriptSDK)
            .HasColumnType("text");

        builder.Property(a => a.CSharpSDK)
            .HasColumnType("text");

        builder.Property(a => a.UsageGuides)
            .HasColumnType("text");

        builder.Property(a => a.Metadata)
            .HasColumnType("text");

        // Configuración de embeddings para pgvector
        builder.Property(a => a.Embeddings)
            .HasColumnType("vector(1536)"); // OpenAI embeddings son de 1536 dimensiones

        builder.Property(a => a.LastGeneratedAt);

        // Propiedades de BaseEntity
        builder.Property(a => a.CreatedAt)
            .IsRequired();

        builder.Property(a => a.UpdatedAt)
            .IsRequired();

        builder.Property(a => a.CreatedBy)
            .IsRequired();

        builder.Property(a => a.UpdatedBy)
            .IsRequired();

        builder.Property(a => a.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        // Relación con Project
        builder.HasOne(a => a.Project)
            .WithMany(p => p.ApiDocumentations)
            .HasForeignKey(a => a.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        // Índices adicionales para búsqueda semántica
        builder.HasIndex(a => a.Embeddings)
            .HasMethod("ivfflat")
            .HasDatabaseName("IX_ApiDocumentations_Embeddings_IVFFlat");
    }
}

