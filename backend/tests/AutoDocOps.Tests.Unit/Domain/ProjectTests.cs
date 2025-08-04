using AutoDocOps.Domain.Entities;
using AutoDocOps.Domain.Enums;
using AutoDocOps.Domain.ValueObjects;
using FluentAssertions;
using System;
using System.Threading;
using Xunit;

namespace AutoDocOps.Tests.Unit.Domain;

/// <summary>
/// Tests unitarios para la entidad Project
/// </summary>
public class ProjectTests
{
    [Fact]
    public void Project_ShouldCreateWithValidData()
    {
        // Arrange
        var name = "Test Project";
        var description = "Test Description";
        var createdBy = Guid.NewGuid();
        var projectType = ProjectType.DotNetApi;
        var language = Language.Spanish;
        var connectionConfig = new ConnectionConfig
        {
            ConnectionString = "test connection",
            RepositoryUrl = "https://github.com/test/repo",
            Branch = "main"
        };
        var docConfig = new DocumentationConfig
        {
            IncludeSwagger = true,
            IncludePostman = true,
            IncludeSDKs = true,
            GenerateGuides = true
        };

        // Act
        var project = new Project(
            name,
            description,
            projectType,
            connectionConfig,
            language,
            docConfig,
            createdBy);

        // Assert
        project.Name.Should().Be(name);
        project.Description.Should().Be(description);
        project.Type.Should().Be(projectType);
        project.PreferredLanguage.Should().Be(language);
        project.Status.Should().Be(ProjectStatus.Created);
        project.CreatedBy.Should().Be(createdBy);
        project.UpdatedBy.Should().Be(createdBy);
        project.Id.Should().NotBeEmpty();
        project.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        project.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Project_ShouldThrowArgumentNullException_WhenNameIsNull()
    {
        // Arrange
        var description = "Valid Description";
        var createdBy = Guid.NewGuid();
        var connectionConfig = new ConnectionConfig { ConnectionString = "test connection" };
        var docConfig = new DocumentationConfig();

        // Act
        Action act = () => new Project(null, description, ProjectType.DotNetApi, connectionConfig, Language.Spanish, docConfig, createdBy);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Project_ShouldThrowArgumentException_WhenNameIsEmptyOrWhitespace(string invalidName)
    {
        // Arrange
        var description = "Valid Description";
        var createdBy = Guid.NewGuid();
        var connectionConfig = new ConnectionConfig { ConnectionString = "test connection" };
        var docConfig = new DocumentationConfig();

        // Act
        Action act = () => new Project(invalidName, description, ProjectType.DotNetApi, connectionConfig, Language.Spanish, docConfig, createdBy);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Project_ShouldUpdateBasicInfo()
    {
        // Arrange
        var project = CreateValidProject();
        var newName = "Updated Name";
        var newDescription = "Updated Description";
        var updatedBy = Guid.NewGuid();
        var originalUpdatedAt = project.UpdatedAt;

        // Act
        Thread.Sleep(10); // Pequeña pausa para asegurar diferencia en timestamp
        project.UpdateBasicInfo(newName, newDescription, updatedBy);

        // Assert
        project.Name.Should().Be(newName);
        project.Description.Should().Be(newDescription);
        project.UpdatedBy.Should().Be(updatedBy);
        project.UpdatedAt.Should().BeAfter(originalUpdatedAt);
    }

    [Fact]
    public void Project_ShouldChangeStatus()
    {
        // Arrange
        var project = CreateValidProject();
        var newStatus = ProjectStatus.Configured;
        var updatedBy = Guid.NewGuid();

        // Act
        project.ChangeStatus(newStatus, updatedBy);

        // Assert
        project.Status.Should().Be(newStatus);
        project.UpdatedBy.Should().Be(updatedBy);
    }

    [Fact]
    public void Project_ShouldMarkAsAnalyzed()
    {
        // Arrange
        var project = CreateValidProject();
        var analyzedBy = Guid.NewGuid();

        // Act
        project.MarkAsAnalyzed(analyzedBy);

        // Assert
        project.Status.Should().Be(ProjectStatus.Analyzed);
        project.LastAnalyzedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        project.UpdatedBy.Should().Be(analyzedBy);
    }

    [Fact]
    public void Project_ShouldUpdateVersion()
    {
        // Arrange
        var project = CreateValidProject();
        var newVersion = "2.0.0";
        var updatedBy = Guid.NewGuid();

        // Act
        project.UpdateVersion(newVersion, updatedBy);

        // Assert
        project.Version.Should().Be(newVersion);
        project.UpdatedBy.Should().Be(updatedBy);
    }

    [Fact]
    public void Project_ShouldUpdateConnectionConfig()
    {
        // Arrange
        var project = CreateValidProject();
        var newConnectionConfig = new ConnectionConfig
        {
            ConnectionString = "new connection string",
            RepositoryUrl = "https://github.com/new/repo",
            Branch = "develop"
        };
        var updatedBy = Guid.NewGuid();

        // Act
        project.UpdateConnectionConfig(newConnectionConfig, updatedBy);

        // Assert
        project.ConnectionConfig.Should().Be(newConnectionConfig);
        project.UpdatedBy.Should().Be(updatedBy);
    }

    [Fact]
    public void Project_ShouldUpdateRepositoryConfig()
    {
        // Arrange
        var project = CreateValidProject();
        var newRepositoryUrl = "https://github.com/updated/repo";
        var newBranch = "feature/new-feature";
        var updatedBy = Guid.NewGuid();

        // Act
        project.UpdateRepositoryConfig(newRepositoryUrl, newBranch, updatedBy);

        // Assert
        project.RepositoryUrl.Should().Be(newRepositoryUrl);
        project.Branch.Should().Be(newBranch);
        project.UpdatedBy.Should().Be(updatedBy);
    }

    [Theory]
    [InlineData(ProjectStatus.Created)]
    [InlineData(ProjectStatus.Configured)]
    [InlineData(ProjectStatus.Analyzing)]
    [InlineData(ProjectStatus.Analyzed)]
    [InlineData(ProjectStatus.DocumentationGenerated)]
    [InlineData(ProjectStatus.Error)]
    [InlineData(ProjectStatus.Paused)]
    public void Project_ShouldAcceptValidStatuses(ProjectStatus status)
    {
        // Arrange
        var project = CreateValidProject();
        var updatedBy = Guid.NewGuid();

        // Act
        project.ChangeStatus(status, updatedBy);

        // Assert
        project.Status.Should().Be(status);
    }

    [Theory]
    [InlineData(ProjectType.DotNetApi)]
    [InlineData(ProjectType.SqlServerDatabase)]
    public void Project_ShouldAcceptValidProjectTypes(ProjectType projectType)
    {
        // Arrange
        var connectionConfig = new ConnectionConfig { ConnectionString = "test" };
        var docConfig = new DocumentationConfig();

        // Act
        var project = new Project(
            "Test Project",
            "Test Description",
            projectType,
            connectionConfig,
            Language.English,
            docConfig,
            Guid.NewGuid());

        // Assert
        project.Type.Should().Be(projectType);
    }

    [Theory]
    [InlineData(Language.Spanish)]
    [InlineData(Language.English)]
    public void Project_ShouldAcceptValidLanguages(Language language)
    {
        // Arrange
        var connectionConfig = new ConnectionConfig { ConnectionString = "test" };
        var docConfig = new DocumentationConfig();

        // Act
        var project = new Project(
            "Test Project",
            "Test Description",
            ProjectType.DotNetApi,
            connectionConfig,
            language,
            docConfig,
            Guid.NewGuid());

        // Assert
        project.PreferredLanguage.Should().Be(language);
    }

    private static Project CreateValidProject()
    {
        var connectionConfig = new ConnectionConfig
        {
            ConnectionString = "test connection",
            RepositoryUrl = "https://github.com/test/repo",
            Branch = "main"
        };
        var docConfig = new DocumentationConfig
        {
            IncludeSwagger = true,
            IncludePostman = true,
            IncludeSDKs = true,
            GenerateGuides = true
        };

        return new Project(
            "Test Project",
            "Test Description",
            ProjectType.DotNetApi,
            connectionConfig,
            Language.Spanish,
            docConfig,
            Guid.NewGuid());
    }
}