# 🚀 Guía de Despliegue - AutoDocOps

Esta guía te ayudará a desplegar AutoDocOps en producción usando Fly.io para el backend y Cloudflare Pages para el frontend.

## 📋 Requisitos Previos

### Herramientas Necesarias
- [Fly CLI](https://fly.io/docs/getting-started/installing-flyctl/) instalado y autenticado
- [Wrangler CLI](https://developers.cloudflare.com/workers/wrangler/install-and-update/) instalado y autenticado
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) instalado
- [Node.js 18+](https://nodejs.org/) y npm instalados
- [Git](https://git-scm.com/) configurado

### Cuentas Requeridas
- Cuenta en [Fly.io](https://fly.io/) (plan gratuito disponible)
- Cuenta en [Cloudflare](https://cloudflare.com/) (plan gratuito disponible)
- Cuenta en [Supabase](https://supabase.com/) (plan gratuito disponible)
- Cuenta en [OpenAI](https://openai.com/) con API key

## 🔧 Configuración Inicial

### 1. Autenticación con Servicios

```bash
# Autenticar con Fly.io
fly auth login

# Autenticar con Cloudflare
wrangler login

# Verificar autenticación
fly auth whoami
wrangler whoami
```

### 2. Configurar Supabase

1. Crea un nuevo proyecto en [Supabase](https://supabase.com/dashboard)
2. Ve a Settings > API para obtener:
   - Project URL
   - Anon public key
3. Ve a Settings > Database para obtener la connection string
4. Ejecuta el script SQL de configuración:

```sql
-- Ejecutar en el SQL Editor de Supabase
-- Contenido del archivo: database/supabase-setup.sql
```

### 3. Configurar OpenAI

1. Obtén tu API key de [OpenAI](https://platform.openai.com/api-keys)
2. Asegúrate de tener créditos disponibles para GPT-4o-mini

## 🚀 Despliegue Automatizado

### Opción 1: Despliegue Completo (Recomendado)

```bash
# Desplegar todo a producción
./scripts/deploy-all.sh production

# Desplegar todo a staging
./scripts/deploy-all.sh staging
```

### Opción 2: Despliegue Individual

```bash
# Solo backend
./scripts/deploy.sh production

# Solo frontend
./scripts/deploy-frontend.sh production
```

## ⚙️ Configuración de Secrets

### Backend (Fly.io)

Los secrets se configuran automáticamente durante el despliegue, pero puedes actualizarlos manualmente:

```bash
# Configurar secrets de producción
fly secrets set DATABASE_URL="postgresql://user:pass@host:5432/autodocops" -a autodocops-api
fly secrets set SUPABASE_URL="https://your-project.supabase.co" -a autodocops-api
fly secrets set SUPABASE_KEY="your-supabase-anon-key" -a autodocops-api
fly secrets set OPENAI_API_KEY="your-openai-api-key" -a autodocops-api
fly secrets set JWT_KEY="your-super-secret-jwt-key-at-least-32-chars" -a autodocops-api

# Ver secrets configurados
fly secrets list -a autodocops-api
```

### Frontend (Variables de Entorno)

Edita `frontend/AutoDocOps-Frontend/.env.production`:

```env
EXPO_PUBLIC_API_URL=https://autodocops-api.fly.dev
EXPO_PUBLIC_APP_NAME=AutoDocOps
EXPO_PUBLIC_APP_VERSION=1.0.0
EXPO_PUBLIC_ENVIRONMENT=production
EXPO_PUBLIC_SUPABASE_URL=https://your-project.supabase.co
EXPO_PUBLIC_SUPABASE_ANON_KEY=your-supabase-anon-key
```

## 🌐 Configuración de Dominios Personalizados

### Backend (Fly.io)

```bash
# Agregar dominio personalizado
fly certs create api.tudominio.com -a autodocops-api

# Verificar certificado
fly certs list -a autodocops-api
```

### Frontend (Cloudflare Pages)

1. Ve al [Dashboard de Cloudflare Pages](https://dash.cloudflare.com/pages)
2. Selecciona tu proyecto `autodocops-frontend`
3. Ve a "Custom domains"
4. Agrega tu dominio personalizado
5. Configura los registros DNS según las instrucciones

## 📊 Monitoreo y Logs

### Backend

```bash
# Ver logs en tiempo real
fly logs -a autodocops-api

# Ver métricas
fly status -a autodocops-api

# SSH al contenedor
fly ssh console -a autodocops-api

# Escalar aplicación
fly scale count 2 -a autodocops-api
fly scale memory 2gb -a autodocops-api
```

### Frontend

```bash
# Ver deployments
wrangler pages deployment list --project-name=autodocops-frontend

# Ver logs de build
# Disponible en el dashboard de Cloudflare Pages
```

## 🔄 Actualizaciones

### Actualización Automática (CI/CD)

El proyecto incluye GitHub Actions que despliegan automáticamente:
- `main` branch → Producción
- `develop` branch → Staging

### Actualización Manual

```bash
# Actualizar todo
./scripts/deploy-all.sh production

# Solo backend
fly deploy -a autodocops-api

# Solo frontend
./scripts/deploy-frontend.sh production
```

## 🛠️ Troubleshooting

### Problemas Comunes

#### Backend no inicia
```bash
# Ver logs detallados
fly logs -a autodocops-api

# Verificar secrets
fly secrets list -a autodocops-api

# Verificar configuración
fly status -a autodocops-api
```

#### Frontend no carga
```bash
# Verificar build
cd frontend/AutoDocOps-Frontend
npm run build

# Verificar variables de entorno
cat .env.production
```

#### Error de conexión a base de datos
```bash
# Verificar connection string
fly ssh console -a autodocops-api
# Dentro del contenedor:
echo $DATABASE_URL
```

### Logs y Debugging

```bash
# Logs del backend
fly logs -a autodocops-api --region mia

# Logs de build del frontend
# Ver en dashboard de Cloudflare Pages

# Health checks
curl https://autodocops-api.fly.dev/health
curl https://autodocops-frontend.pages.dev
```

## 💰 Costos Estimados

### Configuración Gratuita (MVP)
- **Fly.io**: $0/mes (plan gratuito)
- **Cloudflare Pages**: $0/mes (plan gratuito)
- **Supabase**: $0/mes (plan gratuito)
- **OpenAI**: ~$0.50-10/mes (según uso)

**Total: $0.50-10/mes**

### Configuración de Producción
- **Fly.io**: $5-20/mes (según tráfico)
- **Cloudflare Pages**: $0-20/mes (según tráfico)
- **Supabase**: $25/mes (plan Pro)
- **OpenAI**: $10-50/mes (según uso)

**Total: $40-115/mes**

## 🔒 Seguridad

### Configuración de Seguridad Implementada

- ✅ HTTPS forzado en todos los endpoints
- ✅ Headers de seguridad (HSTS, CSP, etc.)
- ✅ Secrets gestionados de forma segura
- ✅ Usuario no-root en contenedores
- ✅ Validación de entrada en APIs
- ✅ Rate limiting configurado
- ✅ CORS configurado correctamente

### Recomendaciones Adicionales

1. **Rotar secrets regularmente**
2. **Configurar alertas de monitoreo**
3. **Implementar backup de base de datos**
4. **Configurar WAF en Cloudflare**
5. **Revisar logs de seguridad regularmente**

## 📞 Soporte

### Recursos Útiles
- [Documentación Fly.io](https://fly.io/docs/)
- [Documentación Cloudflare Pages](https://developers.cloudflare.com/pages/)
- [Documentación Supabase](https://supabase.com/docs)
- [Documentación OpenAI](https://platform.openai.com/docs)

### Comandos de Emergencia

```bash
# Rollback del backend
fly releases -a autodocops-api
fly releases rollback <version> -a autodocops-api

# Reiniciar aplicación
fly restart -a autodocops-api

# Escalar a cero (parar aplicación)
fly scale count 0 -a autodocops-api

# Restaurar desde backup
# Ver documentación de Supabase para backups
```

---

## 🎉 ¡Felicidades!

Si has llegado hasta aquí, AutoDocOps debería estar funcionando correctamente en producción. 

**URLs de tu aplicación:**
- 🔗 **API**: https://autodocops-api.fly.dev
- 🌐 **Frontend**: https://autodocops-frontend.pages.dev
- 📚 **Documentación**: https://autodocops-api.fly.dev/swagger

¡Disfruta de tu generador automático de documentación viva! 🚀

