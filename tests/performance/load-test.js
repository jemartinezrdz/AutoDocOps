import http from 'k6/http';
import { check, sleep } from 'k6';
import { Rate } from 'k6/metrics';

// Métricas personalizadas
export const errorRate = new Rate('errors');

// Configuración de la prueba
export const options = {
  stages: [
    { duration: '2m', target: 10 }, // Ramp up
    { duration: '5m', target: 10 }, // Stay at 10 users
    { duration: '2m', target: 20 }, // Ramp up to 20 users
    { duration: '5m', target: 20 }, // Stay at 20 users
    { duration: '2m', target: 0 },  // Ramp down
  ],
  thresholds: {
    http_req_duration: ['p(95)<2000'], // 95% de requests < 2s
    http_req_failed: ['rate<0.1'],     // Error rate < 10%
    errors: ['rate<0.1'],              // Error rate personalizado < 10%
  },
};

// URL base del API
const BASE_URL = __ENV.BASE_URL || 'https://autodocops-staging.fly.dev';

// Datos de prueba
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
    
    [HttpPost]
    public IActionResult Post([FromBody] string value)
    {
        return Ok($"Received: {value}");
    }
}
`;

const testSqlSchema = `
CREATE TABLE Users (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Email NVARCHAR(255) UNIQUE NOT NULL,
    CreatedAt DATETIME2 DEFAULT GETDATE()
);

CREATE TABLE Projects (
    Id INT PRIMARY KEY IDENTITY(1,1),
    UserId INT FOREIGN KEY REFERENCES Users(Id),
    Name NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX),
    CreatedAt DATETIME2 DEFAULT GETDATE()
);
`;

export function setup() {
  console.log('Iniciando pruebas de carga para AutoDocOps API');
  console.log(`URL base: ${BASE_URL}`);
  
  // Verificar que el API esté disponible
  const healthCheck = http.get(`${BASE_URL}/health`);
  check(healthCheck, {
    'API está disponible': (r) => r.status === 200,
  });
  
  return { baseUrl: BASE_URL };
}

export default function (data) {
  // Test 1: Health Check
  testHealthCheck(data.baseUrl);
  
  // Test 2: API Info
  testApiInfo(data.baseUrl);
  
  // Test 3: Análisis de código .NET
  testDotNetAnalysis(data.baseUrl);
  
  // Test 4: Análisis de SQL Server
  testSqlServerAnalysis(data.baseUrl);
  
  // Test 5: Chat semántico
  testSemanticChat(data.baseUrl);
  
  sleep(1);
}

function testHealthCheck(baseUrl) {
  const response = http.get(`${baseUrl}/health`);
  
  const success = check(response, {
    'Health check status 200': (r) => r.status === 200,
    'Health check response time < 500ms': (r) => r.timings.duration < 500,
  });
  
  errorRate.add(!success);
}

function testApiInfo(baseUrl) {
  const response = http.get(`${baseUrl}/api/info`);
  
  const success = check(response, {
    'API info status 200': (r) => r.status === 200,
    'API info has service name': (r) => JSON.parse(r.body).service === 'AutoDocOps API',
    'API info response time < 1s': (r) => r.timings.duration < 1000,
  });
  
  errorRate.add(!success);
}

function testDotNetAnalysis(baseUrl) {
  const payload = JSON.stringify({
    sourceCode: testCode,
    language: 'es'
  });
  
  const params = {
    headers: {
      'Content-Type': 'application/json',
    },
  };
  
  const response = http.post(`${baseUrl}/api/documentation/analyze-dotnet`, payload, params);
  
  const success = check(response, {
    '.NET analysis status 200': (r) => r.status === 200,
    '.NET analysis has OpenAPI spec': (r) => {
      try {
        const body = JSON.parse(r.body);
        return body.success && body.openApiSpecification;
      } catch {
        return false;
      }
    },
    '.NET analysis response time < 10s': (r) => r.timings.duration < 10000,
  });
  
  errorRate.add(!success);
}

function testSqlServerAnalysis(baseUrl) {
  const payload = JSON.stringify({
    connectionString: 'Server=test;Database=test;Integrated Security=true;',
    language: 'es'
  });
  
  const params = {
    headers: {
      'Content-Type': 'application/json',
    },
  };
  
  const response = http.post(`${baseUrl}/api/documentation/analyze-sqlserver`, payload, params);
  
  // Esperamos que falle la conexión pero que el endpoint responda correctamente
  const success = check(response, {
    'SQL analysis endpoint responds': (r) => r.status === 400 || r.status === 500,
    'SQL analysis response time < 5s': (r) => r.timings.duration < 5000,
  });
  
  errorRate.add(!success);
}

function testSemanticChat(baseUrl) {
  const payload = JSON.stringify({
    question: '¿Cómo puedo usar esta API?',
    context: 'Esta es una API para generar documentación automática.',
    language: 'es'
  });
  
  const params = {
    headers: {
      'Content-Type': 'application/json',
    },
  };
  
  const response = http.post(`${baseUrl}/api/documentation/semantic-chat`, payload, params);
  
  const success = check(response, {
    'Semantic chat status 200': (r) => r.status === 200,
    'Semantic chat has answer': (r) => {
      try {
        const body = JSON.parse(r.body);
        return body.success && body.answer;
      } catch {
        return false;
      }
    },
    'Semantic chat response time < 15s': (r) => r.timings.duration < 15000,
  });
  
  errorRate.add(!success);
}

export function teardown(data) {
  console.log('Pruebas de carga completadas');
  console.log('Revisa los resultados para métricas detalladas');
}

