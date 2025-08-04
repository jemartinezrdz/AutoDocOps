import http from 'k6/http';
import { check, sleep } from 'k6';
import { Rate, Trend } from 'k6/metrics';

// Custom metrics
const errorRate = new Rate('errors');
const responseTime = new Trend('response_time');

// Test configuration
export const options = {
  stages: [
    { duration: '1m', target: 10 }, // Ramp up to 10 users over 1 minute
    { duration: '3m', target: 50 }, // Stay at 50 users for 3 minutes
    { duration: '2m', target: 100 }, // Ramp up to 100 users over 2 minutes
    { duration: '5m', target: 100 }, // Stay at 100 users for 5 minutes (main test)
    { duration: '2m', target: 0 }, // Ramp down to 0 users over 2 minutes
  ],
  thresholds: {
    http_req_duration: ['p(95)<300'], // 95% of requests must complete below 300ms
    http_req_failed: ['rate<0.05'], // Error rate must be below 5%
    errors: ['rate<0.05'],
    response_time: ['p(95)<300'],
  },
  ext: {
    loadimpact: {
      projectID: parseInt(__ENV.K6_PROJECT_ID || '0'),
      name: 'AutoDocOps Load Test - 100 Concurrent Repositories',
    },
  },
};

// Base URL from environment or default
const BASE_URL = __ENV.BASE_URL || 'https://autodocops-staging.fly.dev';

// Test data - simulating 100 different repositories
const repositories = [];
for (let i = 1; i <= 100; i++) {
  repositories.push({
    projectName: `test-repo-${i}`,
    description: `Load test repository ${i} for AutoDocOps performance testing`,
  });
}

export default function () {
  const repo = repositories[Math.floor(Math.random() * repositories.length)];
  
  // Test 1: Health Check
  let response = http.get(`${BASE_URL}/health`);
  check(response, {
    'health check status is 200': (r) => r.status === 200,
    'health check response time < 100ms': (r) => r.timings.duration < 100,
  }) || errorRate.add(1);
  
  responseTime.add(response.timings.duration);
  sleep(0.5);
  
  // Test 2: API Info
  response = http.get(`${BASE_URL}/api/info`);
  check(response, {
    'api info status is 200': (r) => r.status === 200,
    'api info response time < 200ms': (r) => r.timings.duration < 200,
    'api info contains correct service name': (r) => {
      try {
        const body = JSON.parse(r.body);
        return body.name === 'AutoDocOps API';
      } catch {
        return false;
      }
    },
  }) || errorRate.add(1);
  
  responseTime.add(response.timings.duration);
  sleep(0.5);
  
  // Test 3: Documentation List
  response = http.get(`${BASE_URL}/api/v1/documentation`, {
    headers: {
      'api-version': '1.0',
    },
  });
  check(response, {
    'documentation list status is 200': (r) => r.status === 200,
    'documentation list response time < 300ms': (r) => r.timings.duration < 300,
  }) || errorRate.add(1);
  
  responseTime.add(response.timings.duration);
  sleep(0.5);
  
  // Test 4: Documentation Generation (Main Load Test)
  const payload = JSON.stringify(repo);
  response = http.post(`${BASE_URL}/api/v1/documentation/generate`, payload, {
    headers: {
      'Content-Type': 'application/json',
      'api-version': '1.0',
    },
  });
  
  check(response, {
    'doc generation status is 200': (r) => r.status === 200,
    'doc generation response time < 500ms': (r) => r.timings.duration < 500,
    'doc generation returns valid response': (r) => {
      try {
        const body = JSON.parse(r.body);
        return body.message === 'Documentation generation started' && 
               body.projectName === repo.projectName &&
               body.id;
      } catch {
        return false;
      }
    },
  }) || errorRate.add(1);
  
  responseTime.add(response.timings.duration);
  
  // Random sleep between 1-3 seconds to simulate real user behavior
  sleep(Math.random() * 2 + 1);
}

export function handleSummary(data) {
  return {
    'test-results/load-test-summary.json': JSON.stringify(data, null, 2),
    'test-results/load-test-summary.html': generateHTMLSummary(data),
  };
}

function generateHTMLSummary(data) {
  const date = new Date().toISOString();
  const duration = data.state.isStandardOut ? 'N/A' : `${Math.round(data.state.testRunDurationMs / 1000)}s`;
  
  return `
<!DOCTYPE html>
<html>
<head>
    <title>AutoDocOps Load Test Results</title>
    <style>
        body { font-family: Arial, sans-serif; margin: 20px; }
        .header { background: #f0f8ff; padding: 15px; border-radius: 5px; }
        .metric { margin: 10px 0; padding: 10px; border-left: 4px solid #007bff; }
        .passed { border-color: #28a745; background: #f8fff9; }
        .failed { border-color: #dc3545; background: #fff8f8; }
        .warning { border-color: #ffc107; background: #fffef8; }
        table { border-collapse: collapse; width: 100%; margin: 20px 0; }
        th, td { border: 1px solid #ddd; padding: 8px; text-align: left; }
        th { background-color: #f2f2f2; }
    </style>
</head>
<body>
    <div class="header">
        <h1>AutoDocOps Load Test Results</h1>
        <p><strong>Date:</strong> ${date}</p>
        <p><strong>Duration:</strong> ${duration}</p>
        <p><strong>Virtual Users:</strong> Peak of 100 concurrent users</p>
    </div>
    
    <h2>Key Metrics</h2>
    <div class="metric ${data.metrics.http_req_duration?.values?.['p(95)'] < 300 ? 'passed' : 'failed'}">
        <strong>95th Percentile Response Time:</strong> ${Math.round(data.metrics.http_req_duration?.values?.['p(95)'] || 0)}ms
        (Target: &lt; 300ms)
    </div>
    
    <div class="metric ${data.metrics.http_req_failed?.values?.rate < 0.05 ? 'passed' : 'failed'}">
        <strong>Error Rate:</strong> ${((data.metrics.http_req_failed?.values?.rate || 0) * 100).toFixed(2)}%
        (Target: &lt; 5%)
    </div>
    
    <div class="metric ${data.metrics.http_reqs?.values?.count >= 1000 ? 'passed' : 'warning'}">
        <strong>Total Requests:</strong> ${data.metrics.http_reqs?.values?.count || 0}
    </div>
    
    <div class="metric">
        <strong>Average Response Time:</strong> ${Math.round(data.metrics.http_req_duration?.values?.avg || 0)}ms
    </div>
    
    <h2>Detailed Metrics</h2>
    <table>
        <tr>
            <th>Metric</th>
            <th>Average</th>
            <th>Min</th>
            <th>Max</th>
            <th>90th %ile</th>
            <th>95th %ile</th>
        </tr>
        <tr>
            <td>HTTP Request Duration</td>
            <td>${Math.round(data.metrics.http_req_duration?.values?.avg || 0)}ms</td>
            <td>${Math.round(data.metrics.http_req_duration?.values?.min || 0)}ms</td>
            <td>${Math.round(data.metrics.http_req_duration?.values?.max || 0)}ms</td>
            <td>${Math.round(data.metrics.http_req_duration?.values?.['p(90)'] || 0)}ms</td>
            <td>${Math.round(data.metrics.http_req_duration?.values?.['p(95)'] || 0)}ms</td>
        </tr>
    </table>
    
    <h2>Test Scenarios</h2>
    <ul>
        <li>Health Check Endpoint</li>
        <li>API Information Endpoint</li>
        <li>Documentation List Endpoint</li>
        <li>Documentation Generation Endpoint (Main Load)</li>
    </ul>
    
    <p><em>Generated by k6 performance testing for AutoDocOps</em></p>
</body>
</html>`;
}