// global-teardown.js
async function globalTeardown(config) {
  console.log('🧹 Iniciando teardown global para tests E2E');
  
  // Limpiar recursos si es necesario
  // Por ejemplo, limpiar datos de test, cerrar conexiones, etc.
  
  try {
    // Aquí se pueden agregar tareas de limpieza
    // como eliminar datos de test, resetear estado, etc.
    
    console.log('🗑️ Limpieza de recursos completada');
    
    // Generar reporte de resumen si es necesario
    const testResults = process.env.PLAYWRIGHT_TEST_RESULTS;
    if (testResults) {
      console.log('📊 Resultados de tests disponibles en:', testResults);
    }
    
  } catch (error) {
    console.error('❌ Error en teardown global:', error.message);
    // No lanzar error para no fallar el pipeline
  }
  
  console.log('✅ Teardown global completado');
}

module.exports = globalTeardown;

