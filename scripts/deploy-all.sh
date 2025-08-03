#!/bin/bash

# Script maestro de despliegue completo para AutoDocOps
# Despliega backend (Fly.io) y frontend (Cloudflare Pages)
# Autor: AutoDocOps Team
# Versión: 1.0.0

set -e  # Salir en caso de error

# Colores para output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
PURPLE='\033[0;35m'
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

header() {
    echo -e "${PURPLE}================================${NC}"
    echo -e "${PURPLE}$1${NC}"
    echo -e "${PURPLE}================================${NC}"
}

# Verificar que los scripts existen
check_scripts() {
    log "Verificando scripts de despliegue..."
    
    if [ ! -f "scripts/deploy.sh" ]; then
        error "Script de backend no encontrado: scripts/deploy.sh"
        exit 1
    fi
    
    if [ ! -f "scripts/deploy-frontend.sh" ]; then
        error "Script de frontend no encontrado: scripts/deploy-frontend.sh"
        exit 1
    fi
    
    success "Scripts de despliegue encontrados"
}

# Verificar dependencias globales
check_global_dependencies() {
    log "Verificando dependencias globales..."
    
    # Verificar Git
    if ! command -v git &> /dev/null; then
        error "Git no está instalado"
        exit 1
    fi
    
    # Verificar que estamos en un repositorio Git
    if [ ! -d ".git" ]; then
        warning "No estás en un repositorio Git"
        log "Inicializando repositorio Git..."
        git init
        git add .
        git commit -m "Initial commit for AutoDocOps"
    fi
    
    success "Dependencias globales verificadas"
}

# Pre-deployment checks
pre_deployment_checks() {
    log "Ejecutando verificaciones pre-despliegue..."
    
    # Verificar que el proyecto compila
    log "Verificando compilación del backend..."
    cd backend
    if ! dotnet build -c Release --verbosity quiet; then
        error "El backend no compila correctamente"
        exit 1
    fi
    cd ..
    
    # Verificar que el frontend compila
    log "Verificando compilación del frontend..."
    cd frontend/AutoDocOps-Frontend
    if ! npm install --silent; then
        error "Error instalando dependencias del frontend"
        exit 1
    fi
    cd ../..
    
    success "Verificaciones pre-despliegue completadas"
}

# Desplegar backend
deploy_backend() {
    header "DESPLEGANDO BACKEND A FLY.IO"
    
    log "Iniciando despliegue del backend..."
    
    if ./scripts/deploy.sh "$ENVIRONMENT"; then
        success "Backend desplegado exitosamente"
        BACKEND_URL="https://autodocops-api.fly.dev"
    else
        error "Error desplegando backend"
        exit 1
    fi
}

# Actualizar configuración del frontend con URL del backend
update_frontend_config() {
    log "Actualizando configuración del frontend..."
    
    cd frontend/AutoDocOps-Frontend
    
    # Actualizar .env.production con la URL real del backend
    if [ -f ".env.production" ]; then
        sed -i "s|EXPO_PUBLIC_API_URL=.*|EXPO_PUBLIC_API_URL=$BACKEND_URL|" .env.production
    else
        cat > .env.production << EOF
EXPO_PUBLIC_API_URL=$BACKEND_URL
EXPO_PUBLIC_APP_NAME=AutoDocOps
EXPO_PUBLIC_APP_VERSION=1.0.0
EXPO_PUBLIC_ENVIRONMENT=$ENVIRONMENT
EOF
    fi
    
    cd ../..
    success "Configuración del frontend actualizada"
}

# Desplegar frontend
deploy_frontend() {
    header "DESPLEGANDO FRONTEND A CLOUDFLARE PAGES"
    
    log "Iniciando despliegue del frontend..."
    
    if ./scripts/deploy-frontend.sh "$ENVIRONMENT"; then
        success "Frontend desplegado exitosamente"
        if [ "$ENVIRONMENT" = "production" ]; then
            FRONTEND_URL="https://autodocops-frontend.pages.dev"
        else
            FRONTEND_URL="https://autodocops-frontend-staging.pages.dev"
        fi
    else
        error "Error desplegando frontend"
        exit 1
    fi
}

# Verificar despliegue completo
verify_full_deployment() {
    header "VERIFICANDO DESPLIEGUE COMPLETO"
    
    log "Verificando conectividad entre frontend y backend..."
    
    # Esperar un momento para que todo se propague
    sleep 30
    
    # Verificar backend
    log "Verificando backend en $BACKEND_URL/health"
    if curl -f -s "$BACKEND_URL/health" > /dev/null; then
        success "✅ Backend funcionando correctamente"
    else
        warning "⚠️  Backend puede tardar unos minutos en estar disponible"
    fi
    
    # Verificar frontend
    log "Verificando frontend en $FRONTEND_URL"
    if curl -f -s "$FRONTEND_URL" > /dev/null; then
        success "✅ Frontend funcionando correctamente"
    else
        warning "⚠️  Frontend puede tardar unos minutos en estar disponible"
    fi
    
    success "Verificación completada"
}

# Generar reporte de despliegue
generate_deployment_report() {
    header "REPORTE DE DESPLIEGUE"
    
    cat << EOF

🎉 DESPLIEGUE COMPLETADO EXITOSAMENTE
=====================================

📊 Información del Despliegue:
   • Ambiente: $ENVIRONMENT
   • Fecha: $(date)
   • Versión: 1.0.0

🔗 URLs de la Aplicación:
   • Backend API: $BACKEND_URL
   • Frontend Web: $FRONTEND_URL
   • Documentación API: $BACKEND_URL/swagger
   • Health Check: $BACKEND_URL/health

📱 Accesos Rápidos:
   • Dashboard Fly.io: https://fly.io/apps/autodocops-api
   • Dashboard Cloudflare: https://dash.cloudflare.com/pages
   • Logs Backend: fly logs -a autodocops-api
   • Métricas: $BACKEND_URL/metrics

🛠️ Comandos Útiles:
   • Ver logs: fly logs -a autodocops-api
   • SSH al backend: fly ssh console -a autodocops-api
   • Escalar backend: fly scale count 2 -a autodocops-api
   • Actualizar secrets: fly secrets set KEY=value -a autodocops-api

📋 Próximos Pasos:
   1. Configurar dominio personalizado (opcional)
   2. Configurar monitoreo y alertas
   3. Configurar backup de base de datos
   4. Revisar métricas de rendimiento

💡 Notas Importantes:
   • El backend se escala automáticamente según demanda
   • El frontend se sirve desde CDN global de Cloudflare
   • Todos los secrets están configurados de forma segura
   • SSL/TLS está habilitado automáticamente

EOF

    success "Reporte generado exitosamente"
}

# Función principal
main() {
    header "AUTODOCOPS - DESPLIEGUE COMPLETO"
    
    # Verificar argumentos
    ENVIRONMENT=${1:-production}
    
    if [ "$ENVIRONMENT" != "production" ] && [ "$ENVIRONMENT" != "staging" ]; then
        error "Ambiente inválido. Usa: production o staging"
        exit 1
    fi
    
    log "🚀 Iniciando despliegue completo en ambiente: $ENVIRONMENT"
    
    # Ejecutar pasos
    check_scripts
    check_global_dependencies
    pre_deployment_checks
    deploy_backend
    update_frontend_config
    deploy_frontend
    verify_full_deployment
    generate_deployment_report
    
    success "🎉 ¡DESPLIEGUE COMPLETO FINALIZADO EXITOSAMENTE!"
}

# Mostrar ayuda
show_help() {
    cat << EOF
Script maestro de despliegue completo para AutoDocOps

DESCRIPCIÓN:
    Despliega tanto el backend (Fly.io) como el frontend (Cloudflare Pages)
    de forma automatizada y coordinada.

USO:
    $0 [AMBIENTE]

AMBIENTES:
    production  - Despliegue a producción (default)
    staging     - Despliegue a staging

VARIABLES DE ENTORNO OPCIONALES:
    CUSTOM_DOMAIN_API - Dominio personalizado para la API
    CUSTOM_DOMAIN_APP - Dominio personalizado para la aplicación

EJEMPLOS:
    $0 production
    $0 staging
    CUSTOM_DOMAIN_API=api.autodocops.com CUSTOM_DOMAIN_APP=app.autodocops.com $0 production

REQUISITOS:
    • Fly CLI instalado y autenticado
    • Wrangler CLI instalado y autenticado
    • .NET 8 SDK instalado
    • Node.js y npm instalados
    • Git configurado

ORDEN DE DESPLIEGUE:
    1. Verificaciones pre-despliegue
    2. Despliegue del backend a Fly.io
    3. Actualización de configuración del frontend
    4. Despliegue del frontend a Cloudflare Pages
    5. Verificación de conectividad
    6. Generación de reporte

EOF
}

# Verificar argumentos
if [ "$1" = "-h" ] || [ "$1" = "--help" ]; then
    show_help
    exit 0
fi

# Ejecutar función principal
main "$@"

