#!/bin/bash

# Script de despliegue del frontend AutoDocOps a Cloudflare Pages
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
    
    if ! command -v node &> /dev/null; then
        error "Node.js no está instalado."
        exit 1
    fi
    
    if ! command -v npm &> /dev/null; then
        error "npm no está instalado."
        exit 1
    fi
    
    if ! command -v wrangler &> /dev/null; then
        warning "Wrangler CLI no está instalado. Instalando..."
        npm install -g wrangler
    fi
    
    success "Todas las dependencias están disponibles"
}

# Verificar autenticación con Cloudflare
check_cloudflare_auth() {
    log "Verificando autenticación con Cloudflare..."
    
    if ! wrangler whoami &> /dev/null; then
        warning "No estás autenticado con Cloudflare"
        log "Ejecuta: wrangler login"
        exit 1
    fi
    
    success "Autenticado con Cloudflare"
}

# Build del proyecto Expo para web
build_expo_web() {
    log "Compilando proyecto Expo para web..."
    
    cd frontend/AutoDocOps-Frontend
    
    # Instalar dependencias
    log "Instalando dependencias..."
    npm install
    
    # Verificar que expo esté disponible
    if ! npx expo --version &> /dev/null; then
        error "Expo CLI no está disponible"
        exit 1
    fi
    
    # Build para web
    log "Compilando para web..."
    npx expo export --platform web --output-dir dist
    
    success "Build completado en ./dist"
    cd ../..
}

# Configurar variables de entorno para producción
setup_env_vars() {
    log "Configurando variables de entorno..."
    
    cd frontend/AutoDocOps-Frontend
    
    # Crear archivo .env.production si no existe
    if [ ! -f ".env.production" ]; then
        cat > .env.production << EOF
# Configuración de producción para AutoDocOps Frontend
EXPO_PUBLIC_API_URL=https://autodocops-api.fly.dev
EXPO_PUBLIC_APP_NAME=AutoDocOps
EXPO_PUBLIC_APP_VERSION=1.0.0
EXPO_PUBLIC_ENVIRONMENT=production
EXPO_PUBLIC_SUPABASE_URL=https://your-project.supabase.co
EXPO_PUBLIC_SUPABASE_ANON_KEY=your-supabase-anon-key
EOF
        warning "Archivo .env.production creado con valores de ejemplo"
        warning "DEBES actualizar las variables con valores reales"
    fi
    
    cd ../..
    success "Variables de entorno configuradas"
}

# Optimizar build para producción
optimize_build() {
    log "Optimizando build para producción..."
    
    cd frontend/AutoDocOps-Frontend/dist
    
    # Comprimir archivos estáticos
    if command -v gzip &> /dev/null; then
        log "Comprimiendo archivos CSS y JS..."
        find . -name "*.css" -o -name "*.js" -o -name "*.html" | while read file; do
            gzip -k "$file"
        done
        success "Archivos comprimidos"
    fi
    
    cd ../../..
}

# Crear configuración de Cloudflare Pages
create_pages_config() {
    log "Creando configuración de Cloudflare Pages..."
    
    cd frontend/AutoDocOps-Frontend
    
    # Crear wrangler.toml para Pages
    cat > wrangler.toml << EOF
name = "autodocops-frontend"
compatibility_date = "2024-01-01"

[env.production]
name = "autodocops-frontend"

[env.staging]
name = "autodocops-frontend-staging"

# Configuración de Pages
[[pages_build_output_dir]]
directory = "dist"

# Headers personalizados para seguridad
[[headers]]
for = "/*"
[headers.values]
X-Frame-Options = "DENY"
X-Content-Type-Options = "nosniff"
X-XSS-Protection = "1; mode=block"
Referrer-Policy = "strict-origin-when-cross-origin"
Permissions-Policy = "camera=(), microphone=(), geolocation=()"

# Cache para assets estáticos
[[headers]]
for = "/static/*"
[headers.values]
Cache-Control = "public, max-age=31536000, immutable"

# Cache para HTML
[[headers]]
for = "/*.html"
[headers.values]
Cache-Control = "public, max-age=3600"

# Redirects para SPA
[[redirects]]
from = "/*"
to = "/index.html"
status = 200
EOF
    
    cd ../..
    success "Configuración de Pages creada"
}

# Desplegar a Cloudflare Pages
deploy_to_pages() {
    log "Desplegando a Cloudflare Pages..."
    
    cd frontend/AutoDocOps-Frontend
    
    # Verificar que el directorio dist existe
    if [ ! -d "dist" ]; then
        error "Directorio dist no encontrado. Ejecuta el build primero."
        exit 1
    fi
    
    # Deploy usando wrangler
    ENVIRONMENT=${1:-production}
    
    if [ "$ENVIRONMENT" = "production" ]; then
        wrangler pages deploy dist --project-name=autodocops-frontend
    else
        wrangler pages deploy dist --project-name=autodocops-frontend-staging
    fi
    
    cd ../..
    success "Despliegue completado"
}

# Verificar despliegue
verify_deployment() {
    log "Verificando despliegue..."
    
    ENVIRONMENT=${1:-production}
    
    if [ "$ENVIRONMENT" = "production" ]; then
        APP_URL="https://autodocops-frontend.pages.dev"
    else
        APP_URL="https://autodocops-frontend-staging.pages.dev"
    fi
    
    # Esperar un momento para que la app se propague
    sleep 15
    
    log "Verificando aplicación en $APP_URL"
    
    if curl -f -s "$APP_URL" > /dev/null; then
        success "Aplicación accesible"
        success "Frontend desplegado en: $APP_URL"
    else
        warning "La aplicación puede tardar unos minutos en estar disponible"
        log "URL: $APP_URL"
    fi
}

# Configurar dominio personalizado
setup_custom_domain() {
    if [ ! -z "$CUSTOM_DOMAIN" ]; then
        log "Configurando dominio personalizado: $CUSTOM_DOMAIN"
        warning "Configura el dominio manualmente en el dashboard de Cloudflare Pages"
        warning "Dashboard: https://dash.cloudflare.com/pages"
    fi
}

# Función principal
main() {
    log "Iniciando despliegue del frontend AutoDocOps..."
    
    # Verificar argumentos
    ENVIRONMENT=${1:-production}
    
    if [ "$ENVIRONMENT" != "production" ] && [ "$ENVIRONMENT" != "staging" ]; then
        error "Ambiente inválido. Usa: production o staging"
        exit 1
    fi
    
    log "Desplegando en ambiente: $ENVIRONMENT"
    
    # Ejecutar pasos
    check_dependencies
    check_cloudflare_auth
    setup_env_vars
    build_expo_web
    optimize_build
    create_pages_config
    deploy_to_pages "$ENVIRONMENT"
    verify_deployment "$ENVIRONMENT"
    setup_custom_domain
    
    success "🎉 Despliegue del frontend completado exitosamente!"
    
    if [ "$ENVIRONMENT" = "production" ]; then
        success "🌐 Frontend disponible en: https://autodocops-frontend.pages.dev"
        success "📊 Dashboard: https://dash.cloudflare.com/pages"
    else
        success "🌐 Frontend staging disponible en: https://autodocops-frontend-staging.pages.dev"
    fi
}

# Mostrar ayuda
show_help() {
    echo "Script de despliegue del frontend AutoDocOps"
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
    echo "  CUSTOM_DOMAIN=app.autodocops.com $0 production"
    echo "  $0 staging"
}

# Verificar argumentos
if [ "$1" = "-h" ] || [ "$1" = "--help" ]; then
    show_help
    exit 0
fi

# Ejecutar función principal
main "$@"

