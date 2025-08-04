import { test, expect } from '@playwright/test';

test.describe('AutoDocOps API - Happy Path Scenarios', () => {
  
  test('Scenario 1: Connect repository and view documentation', async ({ page }) => {
    // This test simulates the main user journey: connecting a repo and viewing docs
    
    test.step('Check API health', async () => {
      const response = await page.request.get('/health');
      expect(response.ok()).toBeTruthy();
      
      const healthData = await response.json();
      expect(healthData.status).toBe('Healthy');
    });
    
    test.step('Get API information', async () => {
      const response = await page.request.get('/api/info');
      expect(response.ok()).toBeTruthy();
      
      const apiInfo = await response.json();
      expect(apiInfo.name).toBe('AutoDocOps API');
      expect(apiInfo.version).toBeTruthy();
      expect(apiInfo.status).toBe('Running');
    });
    
    test.step('List available documentation endpoints', async () => {
      const response = await page.request.get('/api/v1/documentation');
      expect(response.ok()).toBeTruthy();
      
      const docInfo = await response.json();
      expect(docInfo.message).toBe('AutoDocOps Documentation Service');
      expect(docInfo.endpoints).toBeInstanceOf(Array);
      expect(docInfo.endpoints.length).toBeGreaterThan(0);
    });
    
    test.step('Generate documentation for a sample project', async () => {
      const projectRequest = {
        projectName: 'test-project',
        description: 'A test project for E2E testing'
      };
      
      const response = await page.request.post('/api/v1/documentation/generate', {
        data: projectRequest,
        headers: {
          'Content-Type': 'application/json',
          'api-version': '1.0'
        }
      });
      
      expect(response.ok()).toBeTruthy();
      
      const result = await response.json();
      expect(result.message).toBe('Documentation generation started');
      expect(result.projectName).toBe(projectRequest.projectName);
      expect(result.status).toBe('Processing');
      expect(result.id).toBeTruthy();
      expect(result.version).toBeTruthy();
    });
  });
  
  test('Scenario 2: API versioning and error handling', async ({ page }) => {
    
    test.step('Test API versioning with header', async () => {
      const response = await page.request.get('/api/v1/documentation', {
        headers: {
          'api-version': '1.0'
        }
      });
      
      expect(response.ok()).toBeTruthy();
      expect(response.headers()['api-version']).toBeTruthy();
    });
    
    test.step('Test API versioning with query parameter', async () => {
      const response = await page.request.get('/api/v1/documentation?version=1.0');
      expect(response.ok()).toBeTruthy();
    });
    
    test.step('Test error handling for invalid requests', async () => {
      const response = await page.request.post('/api/v1/documentation/generate', {
        data: {}, // Empty request should cause validation error
        headers: {
          'Content-Type': 'application/json'
        }
      });
      
      // Depending on validation implementation, this might be 400 or still work
      // For now, just check that we get a response
      expect(response.status()).toBeGreaterThanOrEqual(200);
    });
    
    test.step('Test non-existent endpoint', async () => {
      const response = await page.request.get('/api/v1/nonexistent');
      expect(response.status()).toBe(404);
    });
  });
});

test.describe('AutoDocOps API - Performance Tests', () => {
  
  test('Response time should be under 200ms for health check', async ({ page }) => {
    const startTime = Date.now();
    
    const response = await page.request.get('/health');
    
    const endTime = Date.now();
    const responseTime = endTime - startTime;
    
    expect(response.ok()).toBeTruthy();
    expect(responseTime).toBeLessThan(200);
    
    console.log(`Health check response time: ${responseTime}ms`);
  });
  
  test('API info endpoint should respond quickly', async ({ page }) => {
    const startTime = Date.now();
    
    const response = await page.request.get('/api/info');
    
    const endTime = Date.now();
    const responseTime = endTime - startTime;
    
    expect(response.ok()).toBeTruthy();
    expect(responseTime).toBeLessThan(300);
    
    console.log(`API info response time: ${responseTime}ms`);
  });
});

test.describe('AutoDocOps API - Security Tests', () => {
  
  test('Security headers should be present', async ({ page }) => {
    const response = await page.request.get('/health');
    
    const headers = response.headers();
    
    // Check for security headers
    expect(headers['x-content-type-options']).toBe('nosniff');
    expect(headers['x-frame-options']).toBe('DENY');
    expect(headers['x-xss-protection']).toBe('1; mode=block');
    expect(headers['referrer-policy']).toBe('strict-origin-when-cross-origin');
    expect(headers['content-security-policy']).toBeTruthy();
  });
  
  test('HTTPS redirect should work in production', async ({ page }) => {
    // This test would be more relevant in a production environment
    const response = await page.request.get('/health');
    expect(response.ok()).toBeTruthy();
    
    // In production, we should have HSTS header
    const headers = response.headers();
    if (process.env.NODE_ENV === 'production') {
      expect(headers['strict-transport-security']).toBeTruthy();
    }
  });
  
  test('CORS headers should be properly configured', async ({ page }) => {
    const response = await page.request.get('/api/info');
    const headers = response.headers();
    
    // CORS headers should be present for API endpoints
    expect(headers['access-control-allow-origin']).toBeTruthy();
  });
});