using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AutoDocOps.Domain.Entities;
using AutoDocOps.Domain.Enums;
using AutoDocOps.Domain.ValueObjects;
using System.Text.Json;

namespace AutoDocOps.Infrastructure.Configuration;

/// <summary>
/// Configuración de Entity Framework para la entidad Project
/// </summary>
public class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    /// <summary>
    /// Configura la entidad Project
    /// </summary>
    /// <param name="builder">Constructor de la entidad</param>
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        // Configuración de tabla
        builder.ToTable("Projects");

        // Clave primaria
        builder.HasKey(p => p.Id);

        // Propiedades básicas
        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Description)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(p => p.Type)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(p => p.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(p => p.PreferredLanguage)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(p => p.RepositoryUrl)
            .HasMaxLength(500);

        builder.Property(p => p.Branch)
            .HasMaxLength(100);

        builder.Property(p => p.Version)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(p => p.LastAnalyzedAt);

        // Configuración de Value Objects
        builder.OwnsOne(p => p.ConnectionConfig, cc =>
        {
            cc.Property(c => c.ConnectionString)
                .IsRequired()
                .HasMaxLength(1000)
                .HasColumnName("ConnectionString");

            cc.Property(c => c.AuthenticationType)
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnName("AuthenticationType");

            cc.Property(c => c.Username)
                .HasMaxLength(100)
                .HasColumnName("Username");

            cc.Property(c => c.AccessToken)
                .HasMaxLength(2000)
                .HasColumnName("AccessToken");

            cc.Property(c => c.AdditionalSettings)
                .HasMaxLength(4000)
                .HasColumnName("AdditionalSettings");

            cc.Property(c => c.IsEnabled)
                .IsRequired()
                .HasColumnName("ConnectionEnabled");

            cc.Property(c => c.TimeoutSeconds)
                .IsRequired()
                .HasColumnName("ConnectionTimeoutSeconds");
        });

        builder.OwnsOne(p => p.DocumentationConfig, dc =>
        {
            dc.Property(d => d.GenerateOpenApi)
                .IsRequired()
                .HasColumnName("GenerateOpenApi");

            dc.Property(d => d.GenerateSwaggerUI)
                .IsRequired()
                .HasColumnName("GenerateSwaggerUI");

            dc.Property(d => d.GeneratePostmanCollection)
                .IsRequired()
                .HasColumnName("GeneratePostmanCollection");

            dc.Property(d => d.GenerateTypeScriptSDK)
                .IsRequired()
                .HasColumnName("GenerateTypeScriptSDK");

            dc.Property(d => d.GenerateCSharpSDK)
                .IsRequired()
                .HasColumnName("GenerateCSharpSDK");

            dc.Property(d => d.GenerateERDiagrams)
                .IsRequired()
                .HasColumnName("GenerateERDiagrams");

            dc.Property(d => d.GenerateDataDictionary)
                .IsRequired()
                .HasColumnName("GenerateDataDictionary");

            dc.Property(d => d.GenerateUsageGuides)
                .IsRequired()
                .HasColumnName("GenerateUsageGuides");

            dc.Property(d => d.EnableSemanticChat)
                .IsRequired()
                .HasColumnName("EnableSemanticChat");

            dc.Property(d => d.DiagramFormat)
                .IsRequired()
                .HasMaxLength(10)
                .HasColumnName("DiagramFormat");

            dc.Property(d => d.Theme)
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnName("Theme");

            dc.Property(d => d.CustomSettings)
                .HasMaxLength(4000)
                .HasColumnName("CustomSettings");

            dc.Property(d => d.IncludeCodeExamples)
                .IsRequired()
                .HasColumnName("IncludeCodeExamples");

            dc.Property(d => d.IncludeVersioning)
                .IsRequired()
                .HasColumnName("IncludeVersioning");
        });

        // Propiedades de BaseEntity
        builder.Property(p => p.CreatedAt)
            .IsRequired();

        builder.Property(p => p.UpdatedAt)
            .IsRequired();

        builder.Property(p => p.CreatedBy)
            .IsRequired();

        builder.Property(p => p.UpdatedBy)
            .IsRequired();

        builder.Property(p => p.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        // Relaciones
        builder.HasMany(p => p.ApiDocumentations)
            .WithOne(a => a.Project)
            .HasForeignKey(a => a.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.DatabaseSchemas)
            .WithOne(d => d.Project)
            .HasForeignKey(d => d.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

