using AutoDocOps.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace AutoDocOps.Tests.Unit.Domain;

/// <summary>
/// Basic tests to verify domain models work correctly
/// </summary>
public class DomainModelTests
{
    [Fact]
    public void ConnectionConfig_ForGitRepository_ShouldCreateValidConfig()
    {
        // Arrange
        var repositoryUrl = "https://github.com/test/repo.git";
        var accessToken = "token123";
        var username = "testuser";

        // Act
        var config = ConnectionConfig.ForGitRepository(repositoryUrl, accessToken, username);

        // Assert
        config.ConnectionString.Should().Be(repositoryUrl);
        config.AuthenticationType.Should().Be("Bearer");
        config.Username.Should().Be(username);
        config.AccessToken.Should().Be(accessToken);
        config.IsEnabled.Should().BeTrue();
        config.TimeoutSeconds.Should().Be(30);
    }

    [Fact]
    public void ConnectionConfig_ForSqlServer_ShouldCreateValidConfig()
    {
        // Arrange
        var server = "localhost";
        var database = "TestDB";
        var username = "testuser";
        var password = "testpass";

        // Act
        var config = ConnectionConfig.ForSqlServer(server, database, username, password);

        // Assert
        config.ConnectionString.Should().Contain(server);
        config.ConnectionString.Should().Contain(database);
        config.ConnectionString.Should().Contain(username);
        config.AuthenticationType.Should().Be("SqlServer");
        config.Username.Should().Be(username);
        config.AccessToken.Should().Be(password);
    }

    [Fact]
    public void ConnectionConfig_ForSupabase_ShouldCreateValidConfig()
    {
        // Arrange
        var url = "https://test.supabase.co";
        var apiKey = "key123";

        // Act
        var config = ConnectionConfig.ForSupabase(url, apiKey);

        // Assert
        config.ConnectionString.Should().Be(url);
        config.AuthenticationType.Should().Be("ApiKey");
        config.AccessToken.Should().Be(apiKey);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void ConnectionConfig_Constructor_ShouldThrowForInvalidConnectionString(string invalidConnectionString)
    {
        // Act & Assert
        var act = () => new ConnectionConfig(invalidConnectionString, "Bearer");

        act.Should().Throw<ArgumentException>()
           .WithMessage("*cadena de conexión*");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void ConnectionConfig_Constructor_ShouldThrowForInvalidAuthenticationType(string invalidAuthType)
    {
        // Act & Assert
        var act = () => new ConnectionConfig("valid-connection", invalidAuthType);

        act.Should().Throw<ArgumentException>()
           .WithMessage("*tipo de autenticación*");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-10)]
    public void ConnectionConfig_Constructor_ShouldThrowForInvalidTimeout(int invalidTimeout)
    {
        // Act & Assert
        var act = () => new ConnectionConfig("valid-connection", "Bearer", timeoutSeconds: invalidTimeout);

        act.Should().Throw<ArgumentException>()
           .WithMessage("*timeout*");
    }
}