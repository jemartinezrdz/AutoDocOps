// @ts-check
const { test, expect } = require('@playwright/test');

test.describe('AutoDocOps API E2E Tests', () => {
  const baseURL = process.env.BASE_URL || 'https://autodocops-staging.fly.dev';

  test('should return API info', async ({ request }) => {
    const response = await request.get(`${baseURL}/api/info`);
    
    expect(response.status()).toBe(200);
    
    const data = await response.json();
    expect(data.service).toBe('AutoDocOps API');
    expect(data.version).toBe('1.0.0');
    expect(data.status).toBe('Running');
    expect(data.timestamp).toBeDefined();
  });

  test('should return health status', async ({ request }) => {
    const response = await request.get(`${baseURL}/health`);
    
    expect(response.status()).toBe(200);
  });

  test('should analyze .NET code', async ({ request }) => {
    const testCode = `
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok("Hello World");
    }
}`;

    const response = await request.post(`${baseURL}/api/documentation/analyze-dotnet`, {
      data: {
        sourceCode: testCode,
        language: 'es'
      }
    });
    
    expect(response.status()).toBe(200);
    
    const data = await response.json();
    expect(data.success).toBe(true);
    expect(data.openApiSpecification).toBeDefined();
    expect(data.metadata).toBeDefined();
    expect(data.message).toContain('exitosamente');
  });

  test('should handle semantic chat', async ({ request }) => {
    const response = await request.post(`${baseURL}/api/documentation/semantic-chat`, {
      data: {
        question: '¿Cómo puedo usar esta API?',
        context: 'Esta es una API para generar documentación automática de APIs .NET y bases de datos SQL Server.',
        language: 'es'
      }
    });
    
    expect(response.status()).toBe(200);
    
    const data = await response.json();
    expect(data.success).toBe(true);
    expect(data.answer).toBeDefined();
    expect(data.context).toBeDefined();
    expect(data.message).toContain('exitosamente');
  });

  test('should generate usage guides', async ({ request }) => {
    const mockOpenApiSpec = JSON.stringify({
      openapi: '3.1.0',
      info: {
        title: 'Test API',
        version: '1.0.0'
      },
      paths: {
        '/test': {
          get: {
            summary: 'Test endpoint',
            responses: {
              '200': {
                description: 'Success'
              }
            }
          }
        }
      }
    });

    const response = await request.post(`${baseURL}/api/documentation/generate-guides`, {
      data: {
        openApiSpecification: mockOpenApiSpec,
        language: 'es'
      }
    });
    
    expect(response.status()).toBe(200);
    
    const data = await response.json();
    expect(data.success).toBe(true);
    expect(data.usageGuides).toBeDefined();
    expect(data.message).toContain('exitosamente');
  });

  test('should generate Postman collection', async ({ request }) => {
    const mockOpenApiSpec = JSON.stringify({
      openapi: '3.1.0',
      info: {
        title: 'Test API',
        version: '1.0.0'
      },
      paths: {
        '/test': {
          get: {
            summary: 'Test endpoint',
            responses: {
              '200': {
                description: 'Success'
              }
            }
          }
        }
      }
    });

    const response = await request.post(`${baseURL}/api/documentation/generate-postman`, {
      data: {
        openApiSpecification: mockOpenApiSpec,
        baseUrl: 'https://api.example.com'
      }
    });
    
    expect(response.status()).toBe(200);
    
    const data = await response.json();
    expect(data.success).toBe(true);
    expect(data.postmanCollection).toBeDefined();
    expect(data.message).toContain('exitosamente');
  });

  test('should generate TypeScript SDK', async ({ request }) => {
    const mockOpenApiSpec = JSON.stringify({
      openapi: '3.1.0',
      info: {
        title: 'Test API',
        version: '1.0.0'
      },
      paths: {
        '/test': {
          get: {
            summary: 'Test endpoint',
            responses: {
              '200': {
                description: 'Success'
              }
            }
          }
        }
      }
    });

    const response = await request.post(`${baseURL}/api/documentation/generate-typescript-sdk`, {
      data: {
        openApiSpecification: mockOpenApiSpec,
        packageName: 'test-api-client'
      }
    });
    
    expect(response.status()).toBe(200);
    
    const data = await response.json();
    expect(data.success).toBe(true);
    expect(data.sdkCode).toBeDefined();
    expect(data.message).toContain('exitosamente');
  });

  test('should generate C# SDK', async ({ request }) => {
    const mockOpenApiSpec = JSON.stringify({
      openapi: '3.1.0',
      info: {
        title: 'Test API',
        version: '1.0.0'
      },
      paths: {
        '/test': {
          get: {
            summary: 'Test endpoint',
            responses: {
              '200': {
                description: 'Success'
              }
            }
          }
        }
      }
    });

    const response = await request.post(`${baseURL}/api/documentation/generate-csharp-sdk`, {
      data: {
        openApiSpecification: mockOpenApiSpec,
        namespace: 'TestApi.Client'
      }
    });
    
    expect(response.status()).toBe(200);
    
    const data = await response.json();
    expect(data.success).toBe(true);
    expect(data.sdkCode).toBeDefined();
    expect(data.message).toContain('exitosamente');
  });

  test('should generate ER diagram', async ({ request }) => {
    const testSchema = `
CREATE TABLE Users (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Email NVARCHAR(255) UNIQUE NOT NULL
);

CREATE TABLE Projects (
    Id INT PRIMARY KEY IDENTITY(1,1),
    UserId INT FOREIGN KEY REFERENCES Users(Id),
    Name NVARCHAR(200) NOT NULL
);`;

    const response = await request.post(`${baseURL}/api/documentation/generate-er-diagram`, {
      data: {
        schemaDefinition: testSchema,
        language: 'es'
      }
    });
    
    expect(response.status()).toBe(200);
    
    const data = await response.json();
    expect(data.success).toBe(true);
    expect(data.mermaidCode).toBeDefined();
    expect(data.message).toContain('exitosamente');
  });

  test('should generate data dictionary', async ({ request }) => {
    const testSchema = `
CREATE TABLE Users (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Email NVARCHAR(255) UNIQUE NOT NULL
);`;

    const response = await request.post(`${baseURL}/api/documentation/generate-data-dictionary`, {
      data: {
        schemaDefinition: testSchema,
        language: 'es'
      }
    });
    
    expect(response.status()).toBe(200);
    
    const data = await response.json();
    expect(data.success).toBe(true);
    expect(data.dataDictionary).toBeDefined();
    expect(data.message).toContain('exitosamente');
  });

  test('should handle invalid requests gracefully', async ({ request }) => {
    const response = await request.post(`${baseURL}/api/documentation/analyze-dotnet`, {
      data: {
        sourceCode: '',
        language: 'es'
      }
    });
    
    // Should handle empty code gracefully
    expect([400, 500]).toContain(response.status());
  });

  test('should have proper CORS headers', async ({ request }) => {
    const response = await request.options(`${baseURL}/api/info`);
    
    expect(response.status()).toBe(200);
    
    const headers = response.headers();
    expect(headers['access-control-allow-origin']).toBeDefined();
    expect(headers['access-control-allow-methods']).toBeDefined();
    expect(headers['access-control-allow-headers']).toBeDefined();
  });
});

