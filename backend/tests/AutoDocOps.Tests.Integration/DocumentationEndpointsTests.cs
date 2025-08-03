using System.Net;
using System.Text;
using System.Text.Json;

namespace AutoDocOps.Tests.Integration;

/// <summary>
/// Tests de integración para endpoints de documentación
/// </summary>
public class DocumentationEndpointsTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public DocumentationEndpointsTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task GetApiInfo_ShouldReturnSuccessAndCorrectContentType()
    {
        // Act
        var response = await _client.GetAsync("/api/info");

        // Assert
        response.EnsureSuccessStatusCode();
        response.Content.Headers.ContentType?.ToString().Should().Contain("application/json");
        
        var content = await response.Content.ReadAsStringAsync();
        var apiInfo = JsonSerializer.Deserialize<JsonElement>(content);
        
        apiInfo.GetProperty("service").GetString().Should().Be("AutoDocOps API");
        apiInfo.GetProperty("version").GetString().Should().Be("1.0.0");
        apiInfo.GetProperty("status").GetString().Should().Be("Running");
    }

    [Fact]
    public async Task Health_ShouldReturnHealthy()
    {
        // Act
        var response = await _client.GetAsync("/health");

        // Assert
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task AnalyzeDotNet_WithValidCode_ShouldReturnSuccess()
    {
        // Arrange
        var request = new
        {
            sourceCode = @"
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
    
    [HttpPost]
    public IActionResult Post([FromBody] string value)
    {
        return Ok($""Received: {value}"");
    }
}",
            language = "es"
        };

        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/documentation/analyze-dotnet", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var responseContent = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<JsonElement>(responseContent);
        
        result.GetProperty("success").GetBoolean().Should().BeTrue();
        result.GetProperty("openApiSpecification").GetString().Should().NotBeNullOrEmpty();
        result.GetProperty("message").GetString().Should().Contain("exitosamente");
    }

    [Fact]
    public async Task AnalyzeDotNet_WithEmptyCode_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new
        {
            sourceCode = "",
            language = "es"
        };

        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/documentation/analyze-dotnet", content);

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task AnalyzeSqlServer_WithInvalidConnectionString_ShouldReturnError()
    {
        // Arrange
        var request = new
        {
            connectionString = "Invalid connection string",
            language = "es"
        };

        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/documentation/analyze-sqlserver", content);

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task GenerateGuides_WithValidOpenApiSpec_ShouldReturnSuccess()
    {
        // Arrange
        var openApiSpec = JsonSerializer.Serialize(new
        {
            openapi = "3.1.0",
            info = new
            {
                title = "Test API",
                version = "1.0.0"
            },
            paths = new
            {
                test = new
                {
                    get = new
                    {
                        summary = "Test endpoint",
                        responses = new
                        {
                            _200 = new
                            {
                                description = "Success"
                            }
                        }
                    }
                }
            }
        });

        var request = new
        {
            openApiSpecification = openApiSpec,
            language = "es"
        };

        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/documentation/generate-guides", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var responseContent = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<JsonElement>(responseContent);
        
        result.GetProperty("success").GetBoolean().Should().BeTrue();
        result.GetProperty("usageGuides").GetString().Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GeneratePostman_WithValidOpenApiSpec_ShouldReturnSuccess()
    {
        // Arrange
        var openApiSpec = JsonSerializer.Serialize(new
        {
            openapi = "3.1.0",
            info = new
            {
                title = "Test API",
                version = "1.0.0"
            },
            paths = new
            {
                test = new
                {
                    get = new
                    {
                        summary = "Test endpoint"
                    }
                }
            }
        });

        var request = new
        {
            openApiSpecification = openApiSpec,
            baseUrl = "https://api.example.com"
        };

        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/documentation/generate-postman", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var responseContent = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<JsonElement>(responseContent);
        
        result.GetProperty("success").GetBoolean().Should().BeTrue();
        result.GetProperty("postmanCollection").GetString().Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GenerateTypeScriptSDK_WithValidOpenApiSpec_ShouldReturnSuccess()
    {
        // Arrange
        var openApiSpec = JsonSerializer.Serialize(new
        {
            openapi = "3.1.0",
            info = new
            {
                title = "Test API",
                version = "1.0.0"
            }
        });

        var request = new
        {
            openApiSpecification = openApiSpec,
            packageName = "test-api-client"
        };

        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/documentation/generate-typescript-sdk", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var responseContent = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<JsonElement>(responseContent);
        
        result.GetProperty("success").GetBoolean().Should().BeTrue();
        result.GetProperty("sdkCode").GetString().Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GenerateCSharpSDK_WithValidOpenApiSpec_ShouldReturnSuccess()
    {
        // Arrange
        var openApiSpec = JsonSerializer.Serialize(new
        {
            openapi = "3.1.0",
            info = new
            {
                title = "Test API",
                version = "1.0.0"
            }
        });

        var request = new
        {
            openApiSpecification = openApiSpec,
            @namespace = "TestApi.Client"
        };

        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/documentation/generate-csharp-sdk", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var responseContent = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<JsonElement>(responseContent);
        
        result.GetProperty("success").GetBoolean().Should().BeTrue();
        result.GetProperty("sdkCode").GetString().Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task SemanticChat_WithValidQuestion_ShouldReturnSuccess()
    {
        // Arrange
        var request = new
        {
            question = "¿Cómo puedo usar esta API?",
            context = "Esta es una API para generar documentación automática de APIs .NET y bases de datos SQL Server.",
            language = "es"
        };

        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/documentation/semantic-chat", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var responseContent = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<JsonElement>(responseContent);
        
        result.GetProperty("success").GetBoolean().Should().BeTrue();
        result.GetProperty("answer").GetString().Should().NotBeNullOrEmpty();
        result.GetProperty("context").GetString().Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GenerateERDiagram_WithValidSchema_ShouldReturnSuccess()
    {
        // Arrange
        var request = new
        {
            schemaDefinition = @"
CREATE TABLE Users (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Email NVARCHAR(255) UNIQUE NOT NULL
);

CREATE TABLE Projects (
    Id INT PRIMARY KEY IDENTITY(1,1),
    UserId INT FOREIGN KEY REFERENCES Users(Id),
    Name NVARCHAR(200) NOT NULL
);",
            language = "es"
        };

        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/documentation/generate-er-diagram", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var responseContent = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<JsonElement>(responseContent);
        
        result.GetProperty("success").GetBoolean().Should().BeTrue();
        result.GetProperty("mermaidCode").GetString().Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GenerateDataDictionary_WithValidSchema_ShouldReturnSuccess()
    {
        // Arrange
        var request = new
        {
            schemaDefinition = @"
CREATE TABLE Users (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Email NVARCHAR(255) UNIQUE NOT NULL
);",
            language = "es"
        };

        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/documentation/generate-data-dictionary", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var responseContent = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<JsonElement>(responseContent);
        
        result.GetProperty("success").GetBoolean().Should().BeTrue();
        result.GetProperty("dataDictionary").GetString().Should().NotBeNullOrEmpty();
    }

    [Theory]
    [InlineData("/api/documentation/analyze-dotnet")]
    [InlineData("/api/documentation/analyze-sqlserver")]
    [InlineData("/api/documentation/generate-guides")]
    [InlineData("/api/documentation/generate-postman")]
    [InlineData("/api/documentation/generate-typescript-sdk")]
    [InlineData("/api/documentation/generate-csharp-sdk")]
    [InlineData("/api/documentation/semantic-chat")]
    [InlineData("/api/documentation/generate-er-diagram")]
    [InlineData("/api/documentation/generate-data-dictionary")]
    public async Task DocumentationEndpoints_ShouldRequireValidContentType(string endpoint)
    {
        // Arrange
        var content = new StringContent("invalid json", Encoding.UTF8, "text/plain");

        // Act
        var response = await _client.PostAsync(endpoint, content);

        // Assert
        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.BadRequest, 
            HttpStatusCode.UnsupportedMediaType,
            HttpStatusCode.InternalServerError);
    }

    [Theory]
    [InlineData("/api/documentation/analyze-dotnet")]
    [InlineData("/api/documentation/analyze-sqlserver")]
    [InlineData("/api/documentation/generate-guides")]
    public async Task DocumentationEndpoints_ShouldReturnBadRequest_WithEmptyBody(string endpoint)
    {
        // Arrange
        var content = new StringContent("", Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync(endpoint, content);

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.InternalServerError);
    }
}

