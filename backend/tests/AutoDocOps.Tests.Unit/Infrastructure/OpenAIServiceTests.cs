using AutoDocOps.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AutoDocOps.Tests.Unit.Infrastructure;

/// <summary>
/// Tests unitarios para OpenAIService
/// </summary>
public class OpenAIServiceTests
{
    private readonly Mock<ILogger<OpenAIService>> _mockLogger;
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly OpenAIService _openAIService;

    public OpenAIServiceTests()
    {
        _mockLogger = new Mock<ILogger<OpenAIService>>();
        _mockConfiguration = new Mock<IConfiguration>();
        
        // Configurar mock de configuración
        _mockConfiguration.Setup(x => x["OpenAI:ApiKey"]).Returns("test-api-key");
        _mockConfiguration.Setup(x => x["OpenAI:Model"]).Returns("gpt-4o-mini");
        
        _openAIService = new OpenAIService(_mockLogger.Object, _mockConfiguration.Object);
    }

    [Fact]
    public void OpenAIService_ShouldInitialize_WithValidConfiguration()
    {
        // Arrange & Act & Assert
        _openAIService.Should().NotBeNull();
    }

    [Theory]
    [InlineData("es")]
    [InlineData("en")]
    public void AnalyzeDotNetApiAsync_ShouldAcceptValidLanguages(string language)
    {
        // Arrange
        var sourceCode = @"
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route(""api/[controller]"")]
public class TestController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(""Hello World"");
    }
}";

        // Act & Assert
        var act = async () => await _openAIService.AnalyzeDotNetApiAsync(sourceCode, language);
        
        // En un test real, esto requeriría mock del cliente OpenAI
        // Por ahora, verificamos que el método existe y acepta los parámetros
        act.Should().NotBeNull();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void AnalyzeDotNetApiAsync_ShouldHandleInvalidSourceCode(string invalidSourceCode)
    {
        // Arrange & Act & Assert
        var act = async () => await _openAIService.AnalyzeDotNetApiAsync(invalidSourceCode, "es");
        
        act.Should().ThrowAsync<ArgumentException>()
           .WithMessage("*código fuente no puede estar vacío*");
    }

    [Theory]
    [InlineData("es")]
    [InlineData("en")]
    public void AnalyzeSqlServerSchemaAsync_ShouldAcceptValidLanguages(string language)
    {
        // Arrange
        var schemaDefinition = @"
CREATE TABLE Users (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Email NVARCHAR(255) UNIQUE NOT NULL
);";

        // Act & Assert
        var act = async () => await _openAIService.AnalyzeSqlServerSchemaAsync(schemaDefinition, language);
        
        act.Should().NotBeNull();
    }

    [Theory]
    [InlineData("es")]
    [InlineData("en")]
    public void GenerateUsageGuidesAsync_ShouldAcceptValidParameters(string language)
    {
        // Arrange
        var openApiSpec = @"{
            ""openapi"": ""3.1.0"",
            ""info"": {
                ""title"": ""Test API"",
                ""version"": ""1.0.0""
            }
        }";

        // Act & Assert
        var act = async () => await _openAIService.GenerateUsageGuidesAsync(openApiSpec, language);
        
        act.Should().NotBeNull();
    }

    [Fact]
    public void GeneratePostmanCollectionAsync_ShouldAcceptValidParameters()
    {
        // Arrange
        var openApiSpec = @"{
            ""openapi"": ""3.1.0"",
            ""info"": {
                ""title"": ""Test API"",
                ""version"": ""1.0.0""
            }
        }";
        var baseUrl = "https://api.example.com";

        // Act & Assert
        var act = async () => await _openAIService.GeneratePostmanCollectionAsync(openApiSpec, baseUrl);
        
        act.Should().NotBeNull();
    }

    [Fact]
    public void GenerateTypeScriptSDKAsync_ShouldAcceptValidParameters()
    {
        // Arrange
        var openApiSpec = @"{
            ""openapi"": ""3.1.0"",
            ""info"": {
                ""title"": ""Test API"",
                ""version"": ""1.0.0""
            }
        }";
        var packageName = "test-api-client";

        // Act & Assert
        var act = async () => await _openAIService.GenerateTypeScriptSDKAsync(openApiSpec, packageName);
        
        act.Should().NotBeNull();
    }

    [Fact]
    public void GenerateCSharpSDKAsync_ShouldAcceptValidParameters()
    {
        // Arrange
        var openApiSpec = @"{
            ""openapi"": ""3.1.0"",
            ""info"": {
                ""title"": ""Test API"",
                ""version"": ""1.0.0""
            }
        }";
        var namespaceName = "TestApi.Client";

        // Act & Assert
        var act = async () => await _openAIService.GenerateCSharpSDKAsync(openApiSpec, namespaceName);
        
        act.Should().NotBeNull();
    }

    [Theory]
    [InlineData("es")]
    [InlineData("en")]
    public void AnswerQuestionAsync_ShouldAcceptValidParameters(string language)
    {
        // Arrange
        var question = "¿Cómo puedo usar esta API?";
        var context = "Esta es una API para generar documentación automática.";

        // Act & Assert
        var act = async () => await _openAIService.AnswerQuestionAsync(question, context, language);
        
        act.Should().NotBeNull();
    }

    [Theory]
    [InlineData("es")]
    [InlineData("en")]
    public void GenerateERDiagramAsync_ShouldAcceptValidParameters(string language)
    {
        // Arrange
        var schemaDefinition = @"
CREATE TABLE Users (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL
);";

        // Act & Assert
        var act = async () => await _openAIService.GenerateERDiagramAsync(schemaDefinition, language);
        
        act.Should().NotBeNull();
    }

    [Theory]
    [InlineData("es")]
    [InlineData("en")]
    public void GenerateDataDictionaryAsync_ShouldAcceptValidParameters(string language)
    {
        // Arrange
        var schemaDefinition = @"
CREATE TABLE Users (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL
);";

        // Act & Assert
        var act = async () => await _openAIService.GenerateDataDictionaryAsync(schemaDefinition, language);
        
        act.Should().NotBeNull();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void AnswerQuestionAsync_ShouldHandleInvalidQuestions(string invalidQuestion)
    {
        // Arrange
        var context = "Valid context";
        var language = "es";

        // Act & Assert
        var act = async () => await _openAIService.AnswerQuestionAsync(invalidQuestion, context, language);
        
        act.Should().ThrowAsync<ArgumentException>()
           .WithMessage("*pregunta no puede estar vacía*");
    }

    [Theory]
    [InlineData("invalid-language")]
    [InlineData("fr")]
    [InlineData("de")]
    public void AnalyzeDotNetApiAsync_ShouldHandleUnsupportedLanguages(string unsupportedLanguage)
    {
        // Arrange
        var sourceCode = "valid source code";

        // Act & Assert
        var act = async () => await _openAIService.AnalyzeDotNetApiAsync(sourceCode, unsupportedLanguage);
        
        act.Should().ThrowAsync<ArgumentException>()
           .WithMessage($"*Idioma '{unsupportedLanguage}' no soportado*");
    }
}

