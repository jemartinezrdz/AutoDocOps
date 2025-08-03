#!/bin/bash

# Script de despliegue automatizado para AutoDocOps
# Autor: AutoDocOps Team
# Versión: 1.0.0

set -e  # Salir en caso de error

# Colores para output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Función para logging
log() {
    echo -e "${BLUE}[$(date +'%Y-%m-%d %H:%M:%S')]${NC} $1"
}

success() {
    echo -e "${GREEN}[SUCCESS]${NC} $1"
}

warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# Verificar dependencias
check_dependencies() {
    log "Verificando dependencias..."
    
    if ! command -v fly &> /dev/null; then
        error "Fly CLI no está instalado. Instálalo desde https://fly.io/docs/getting-started/installing-flyctl/"
        exit 1
    fi
    
    if ! command -v dotnet &> /dev/null; then
        error ".NET SDK no está instalado."
        exit 1
    fi
    
    if ! command -v docker &> /dev/null; then
        warning "Docker no está disponible. El build se realizará en Fly.io."
    fi
    
    success "Todas las dependencias están disponibles"
}

# Verificar autenticación con Fly.io
check_fly_auth() {
    log "Verificando autenticación con Fly.io..."
    
    if ! fly auth whoami &> /dev/null; then
        warning "No estás autenticado con Fly.io"
        log "Ejecuta: fly auth login"
        exit 1
    fi
    
    success "Autenticado con Fly.io"
}

# Build y test del proyecto
build_and_test() {
    log "Compilando y ejecutando tests..."
    
    cd backend
    
    # Restaurar dependencias
    log "Restaurando dependencias..."
    dotnet restore
    
    # Compilar proyecto
    log "Compilando proyecto..."
    dotnet build -c Release --no-restore
    
    # Ejecutar tests
    log "Ejecutando tests..."
    if [ -d "tests" ]; then
        dotnet test --no-build -c Release --verbosity minimal
        success "Todos los tests pasaron"
    else
        warning "No se encontraron tests"
    fi
    
    cd ..
    success "Build y tests completados"
}

# Configurar secrets en Fly.io
setup_secrets() {
    log "Configurando secrets en Fly.io..."
    
    # Verificar si la app existe
    if ! fly apps list | grep -q "autodocops-api"; then
        log "Creando aplicación en Fly.io..."
        fly apps create autodocops-api --org personal
    fi
    
    # Configurar secrets (valores de ejemplo - deben ser reemplazados)
    warning "Configurando secrets de ejemplo. DEBES actualizarlos con valores reales:"
    
    echo "DATABASE_URL=postgresql://user:pass@host:5432/autodocops" | fly secrets import
    echo "SUPABASE_URL=https://your-project.supabase.co" | fly secrets import
    echo "SUPABASE_KEY=your-supabase-anon-key" | fly secrets import
    echo "OPENAI_API_KEY=your-openai-api-key" | fly secrets import
    echo "JWT_KEY=your-super-secret-jwt-key-at-least-32-chars" | fly secrets import
    
    success "Secrets configurados (recuerda actualizarlos)"
}

# Desplegar a Fly.io
deploy_to_fly() {
    log "Desplegando a Fly.io..."
    
    # Verificar que fly.toml existe
    if [ ! -f "fly.toml" ]; then
        error "No se encontró fly.toml"
        exit 1
    fi
    
    # Deploy
    fly deploy --ha=false --wait-timeout=300
    
    success "Despliegue completado"
}

# Verificar despliegue
verify_deployment() {
    log "Verificando despliegue..."
    
    # Obtener URL de la aplicación
    APP_URL=$(fly info | grep "Hostname" | awk '{print $2}')
    
    if [ -z "$APP_URL" ]; then
        error "No se pudo obtener la URL de la aplicación"
        exit 1
    fi
    
    # Verificar health check
    log "Verificando health check en https://$APP_URL/health"
    
    # Esperar un momento para que la app se inicie
    sleep 30
    
    if curl -f -s "https://$APP_URL/health" > /dev/null; then
        success "Health check exitoso"
        success "Aplicación desplegada en: https://$APP_URL"
    else
        error "Health check falló"
        log "Verificando logs..."
        fly logs
        exit 1
    fi
}

# Configurar dominio personalizado (opcional)
setup_custom_domain() {
    if [ ! -z "$CUSTOM_DOMAIN" ]; then
        log "Configurando dominio personalizado: $CUSTOM_DOMAIN"
        fly certs create "$CUSTOM_DOMAIN"
        success "Certificado SSL creado para $CUSTOM_DOMAIN"
    fi
}

# Función principal
main() {
    log "Iniciando despliegue de AutoDocOps..."
    
    # Verificar argumentos
    ENVIRONMENT=${1:-production}
    
    if [ "$ENVIRONMENT" != "production" ] && [ "$ENVIRONMENT" != "staging" ]; then
        error "Ambiente inválido. Usa: production o staging"
        exit 1
    fi
    
    log "Desplegando en ambiente: $ENVIRONMENT"
    
    # Ejecutar pasos
    check_dependencies
    check_fly_auth
    build_and_test
    
    if [ "$ENVIRONMENT" = "production" ]; then
        setup_secrets
    fi
    
    deploy_to_fly
    verify_deployment
    setup_custom_domain
    
    success "🎉 Despliegue completado exitosamente!"
    success "📱 API disponible en: https://autodocops-api.fly.dev"
    success "📊 Dashboard: https://fly.io/apps/autodocops-api"
    success "📋 Logs: fly logs -a autodocops-api"
    success "🔧 SSH: fly ssh console -a autodocops-api"
}

# Mostrar ayuda
show_help() {
    echo "Script de despliegue para AutoDocOps"
    echo ""
    echo "Uso: $0 [AMBIENTE]"
    echo ""
    echo "Ambientes:"
    echo "  production  - Despliegue a producción (default)"
    echo "  staging     - Despliegue a staging"
    echo ""
    echo "Variables de entorno opcionales:"
    echo "  CUSTOM_DOMAIN - Dominio personalizado para la aplicación"
    echo ""
    echo "Ejemplos:"
    echo "  $0 production"
    echo "  CUSTOM_DOMAIN=api.autodocops.com $0 production"
    echo "  $0 staging"
}

# Verificar argumentos
if [ "$1" = "-h" ] || [ "$1" = "--help" ]; then
    show_help
    exit 0
fi

# Ejecutar función principal
main "$@"

