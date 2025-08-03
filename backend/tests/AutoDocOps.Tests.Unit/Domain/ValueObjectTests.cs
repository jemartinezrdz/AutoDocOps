using AutoDocOps.Domain.ValueObjects;

namespace AutoDocOps.Tests.Unit.Domain;

/// <summary>
/// Tests unitarios para Value Objects
/// </summary>
public class ValueObjectTests
{
    public class ConnectionConfigTests
    {
        [Fact]
        public void ConnectionConfig_ShouldCreateWithValidData()
        {
            // Arrange
            var connectionString = "Server=localhost;Database=TestDB;Integrated Security=true;";
            var repositoryUrl = "https://github.com/user/repo";
            var branch = "main";

            // Act
            var config = new ConnectionConfig
            {
                ConnectionString = connectionString,
                RepositoryUrl = repositoryUrl,
                Branch = branch
            };

            // Assert
            config.ConnectionString.Should().Be(connectionString);
            config.RepositoryUrl.Should().Be(repositoryUrl);
            config.Branch.Should().Be(branch);
        }

        [Theory]
        [InlineData("Server=localhost;Database=TestDB;Integrated Security=true;")]
        [InlineData("Data Source=server;Initial Catalog=db;User ID=user;Password=pass;")]
        [InlineData("Host=localhost;Database=postgres;Username=user;Password=pass;")]
        public void ConnectionConfig_ShouldAcceptValidConnectionStrings(string connectionString)
        {
            // Arrange & Act
            var config = new ConnectionConfig
            {
                ConnectionString = connectionString
            };

            // Assert
            config.ConnectionString.Should().Be(connectionString);
        }

        [Theory]
        [InlineData("https://github.com/user/repo")]
        [InlineData("https://gitlab.com/user/repo")]
        [InlineData("https://bitbucket.org/user/repo")]
        [InlineData("git@github.com:user/repo.git")]
        public void ConnectionConfig_ShouldAcceptValidRepositoryUrls(string repositoryUrl)
        {
            // Arrange & Act
            var config = new ConnectionConfig
            {
                RepositoryUrl = repositoryUrl
            };

            // Assert
            config.RepositoryUrl.Should().Be(repositoryUrl);
        }

        [Theory]
        [InlineData("main")]
        [InlineData("master")]
        [InlineData("develop")]
        [InlineData("feature/new-feature")]
        [InlineData("release/v1.0.0")]
        public void ConnectionConfig_ShouldAcceptValidBranches(string branch)
        {
            // Arrange & Act
            var config = new ConnectionConfig
            {
                Branch = branch
            };

            // Assert
            config.Branch.Should().Be(branch);
        }
    }

    public class DocumentationConfigTests
    {
        [Fact]
        public void DocumentationConfig_ShouldCreateWithDefaultValues()
        {
            // Arrange & Act
            var config = new DocumentationConfig();

            // Assert
            config.IncludeSwagger.Should().BeTrue();
            config.IncludePostman.Should().BeTrue();
            config.IncludeSDKs.Should().BeTrue();
            config.GenerateGuides.Should().BeTrue();
        }

        [Fact]
        public void DocumentationConfig_ShouldAllowCustomConfiguration()
        {
            // Arrange & Act
            var config = new DocumentationConfig
            {
                IncludeSwagger = false,
                IncludePostman = true,
                IncludeSDKs = false,
                GenerateGuides = true
            };

            // Assert
            config.IncludeSwagger.Should().BeFalse();
            config.IncludePostman.Should().BeTrue();
            config.IncludeSDKs.Should().BeFalse();
            config.GenerateGuides.Should().BeTrue();
        }

        [Theory]
        [InlineData(true, true, true, true)]
        [InlineData(false, false, false, false)]
        [InlineData(true, false, true, false)]
        [InlineData(false, true, false, true)]
        public void DocumentationConfig_ShouldAcceptAllCombinations(
            bool includeSwagger, 
            bool includePostman, 
            bool includeSDKs, 
            bool generateGuides)
        {
            // Arrange & Act
            var config = new DocumentationConfig
            {
                IncludeSwagger = includeSwagger,
                IncludePostman = includePostman,
                IncludeSDKs = includeSDKs,
                GenerateGuides = generateGuides
            };

            // Assert
            config.IncludeSwagger.Should().Be(includeSwagger);
            config.IncludePostman.Should().Be(includePostman);
            config.IncludeSDKs.Should().Be(includeSDKs);
            config.GenerateGuides.Should().Be(generateGuides);
        }

        [Fact]
        public void DocumentationConfig_ShouldIndicateWhenFullDocumentationEnabled()
        {
            // Arrange
            var config = new DocumentationConfig
            {
                IncludeSwagger = true,
                IncludePostman = true,
                IncludeSDKs = true,
                GenerateGuides = true
            };

            // Act
            var isFullDocumentation = config.IncludeSwagger && 
                                    config.IncludePostman && 
                                    config.IncludeSDKs && 
                                    config.GenerateGuides;

            // Assert
            isFullDocumentation.Should().BeTrue();
        }

        [Fact]
        public void DocumentationConfig_ShouldIndicateWhenMinimalDocumentationEnabled()
        {
            // Arrange
            var config = new DocumentationConfig
            {
                IncludeSwagger = true,
                IncludePostman = false,
                IncludeSDKs = false,
                GenerateGuides = false
            };

            // Act
            var isMinimalDocumentation = config.IncludeSwagger && 
                                       !config.IncludePostman && 
                                       !config.IncludeSDKs && 
                                       !config.GenerateGuides;

            // Assert
            isMinimalDocumentation.Should().BeTrue();
        }
    }
}

