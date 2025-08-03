// global-setup.js
const { chromium } = require('@playwright/test');

async function globalSetup(config) {
  console.log('🚀 Iniciando setup global para tests E2E');
  
  const baseURL = process.env.BASE_URL || 'https://autodocops-staging.fly.dev';
  console.log(`📡 URL base: ${baseURL}`);
  
  // Verificar que el API esté disponible antes de ejecutar tests
  const browser = await chromium.launch();
  const page = await browser.newPage();
  
  try {
    console.log('🔍 Verificando disponibilidad del API...');
    
    // Intentar conectar al health endpoint
    const response = await page.goto(`${baseURL}/health`, {
      waitUntil: 'networkidle',
      timeout: 30000
    });
    
    if (!response || !response.ok()) {
      throw new Error(`API no disponible. Status: ${response?.status()}`);
    }
    
    console.log('✅ API disponible y funcionando');
    
    // Verificar endpoint de info
    await page.goto(`${baseURL}/api/info`);
    const apiInfo = await page.textContent('body');
    const info = JSON.parse(apiInfo);
    
    if (info.service !== 'AutoDocOps API') {
      throw new Error('API no retorna información esperada');
    }
    
    console.log(`✅ API Info verificado: ${info.service} v${info.version}`);
    
  } catch (error) {
    console.error('❌ Error en setup global:', error.message);
    throw error;
  } finally {
    await browser.close();
  }
  
  console.log('🎯 Setup global completado exitosamente');
}

module.exports = globalSetup;

