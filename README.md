# 🚀 AutoDocOps

**Generador automático de documentación viva para APIs .NET y bases de datos SQL Server**

[![.NET 8](https://img.shields.io/badge/.NET-8.0-blue.svg)](https://dotnet.microsoft.com/download/dotnet/8.0)
[![React 19](https://img.shields.io/badge/React-19.0-blue.svg)](https://reactjs.org/)
[![TypeScript](https://img.shields.io/badge/TypeScript-5.0-blue.svg)](https://www.typescriptlang.org/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![Deploy Status](https://img.shields.io/badge/Deploy-Ready-green.svg)](https://autodocops-api.fly.dev)

## 📖 Descripción

AutoDocOps es una plataforma SaaS que automatiza la generación de documentación viva para APIs .NET y bases de datos SQL Server. Combina análisis de código inteligente con IA generativa para crear documentación completa, actualizada y accesible.

### 🎯 Propuesta de Valor

- **Conectividad Universal**: Se conecta directamente a repositorios .NET y bases de datos SQL Server
- **Documentación Completa**: Genera OpenAPI 3.1, Swagger UI, colecciones Postman y SDKs en TypeScript/C#
- **Análisis Inteligente**: Crea diagramas ER y diccionarios de datos en español e inglés
- **Chat Semántico**: Guías de uso interactivas con GPT-4o y búsqueda vectorial
- **Ventaja Competitiva**: Funcionalidades únicas que competidores como SwaggerHub ($75-399/mes) no ofrecen

## ✨ Características Principales

### 🔍 Análisis Automático
- Extracción de metadatos de APIs .NET
- Análisis de esquemas SQL Server
- Generación de OpenAPI 3.1 compliant
- Detección automática de endpoints y modelos

### 📚 Documentación Generada
- **Swagger UI** interactivo y personalizable
- **Colecciones Postman** listas para usar
- **SDKs** en TypeScript y C# auto-generados
- **Guías de uso** detalladas en múltiples idiomas

### 🗄️ Base de Datos
- **Diagramas ER** en formato Mermaid
- **Diccionarios de datos** completos
- Análisis de relaciones y constraints
- Documentación de procedimientos almacenados

### 🤖 IA Integrada
- **Chat semántico** con contexto de documentación
- **Búsqueda vectorial** con pgvector
- **GPT-4o-mini** para generación de contenido
- Cache inteligente para optimización de costos

## 🏗️ Arquitectura

### Stack Tecnológico

**Backend**
- .NET 8 LTS con Minimal API y AOT
- Arquitectura Clean/Hexagonal
- Entity Framework Core con Npgsql
- Autenticación JWT + Supabase Auth

**Frontend**
- Expo + React 19 (Native + Web)
- TypeScript estricto
- React Navigation 6
- Componentes funcionales modernos

**Base de Datos**
- Supabase Postgres 14 con pgvector
- Row Level Security (RLS)
- Índices vectoriales optimizados
- Backup automático

**Infraestructura**
- Hosting: Fly.io + Cloudflare Pages + R2
- CI/CD: GitHub Actions + Terraform
- Observabilidad: Grafana Cloud (Loki + Tempo)
- Seguridad: TLS 1.3 + HSTS + CSP

## 🚀 Inicio Rápido

### Prerrequisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js 18+](https://nodejs.org/)
- [Docker](https://docker.com/) (opcional)
- Cuenta en [Supabase](https://supabase.com/)
- API Key de [OpenAI](https://openai.com/)

### Instalación Local

```bash
# Clonar repositorio
git clone https://github.com/tu-usuario/AutoDocOps.git
cd AutoDocOps

# Configurar backend
cd backend
dotnet restore
dotnet build

# Configurar frontend
cd ../frontend/AutoDocOps-Frontend
npm install

# Configurar variables de entorno
cp .env.example .env.local
# Editar .env.local con tus configuraciones
```

### Configuración

1. **Base de Datos**:
   ```bash
   # Ejecutar script de configuración en Supabase
   psql -f database/supabase-setup.sql
   ```

2. **Variables de Entorno**:
   ```bash
   # Backend (.env)
   DATABASE_URL=postgresql://...
   SUPABASE_URL=https://...
   OPENAI_API_KEY=sk-...
   
   # Frontend (.env.local)
   EXPO_PUBLIC_API_URL=http://localhost:5000
   EXPO_PUBLIC_SUPABASE_URL=https://...
   ```

3. **Ejecutar Aplicación**:
   ```bash
   # Backend
   cd backend/src/AutoDocOps.Api/AutoDocOps.Api
   dotnet run
   
   # Frontend (nueva terminal)
   cd frontend/AutoDocOps-Frontend
   npm run web
   ```

## 📦 Despliegue

### Despliegue Automatizado

```bash
# Despliegue completo a producción
./scripts/deploy-all.sh production

# Solo backend
./scripts/deploy.sh production

# Solo frontend
./scripts/deploy-frontend.sh production
```

### URLs de Producción

- **API**: https://autodocops-api.fly.dev
- **Frontend**: https://autodocops-frontend.pages.dev
- **Documentación**: https://autodocops-api.fly.dev/swagger

Ver [Guía de Despliegue](docs/DEPLOYMENT.md) para instrucciones detalladas.

## 🧪 Testing

### Ejecutar Tests

```bash
# Tests unitarios
cd backend
dotnet test tests/AutoDocOps.Tests.Unit

# Tests de integración
dotnet test tests/AutoDocOps.Tests.Integration

# Tests E2E
npm run test:e2e

# Tests de performance
k6 run tests/performance/load-test.js
```

### Cobertura de Tests

- **Tests Unitarios**: Dominio, Aplicación, Infraestructura
- **Tests de Integración**: Todos los endpoints de API
- **Tests E2E**: Flujos completos de usuario
- **Tests de Performance**: Carga y estrés
- **Tests de Seguridad**: OWASP ZAP scanning

## 📊 Métricas y Monitoreo

### Health Checks

- **API**: `/health` - Estado general del sistema
- **Base de Datos**: Conectividad y latencia
- **IA**: Disponibilidad de OpenAI
- **Cache**: Estado de Redis/memoria

### Métricas Disponibles

- Tiempo de respuesta de endpoints
- Uso de tokens de OpenAI
- Throughput de documentación generada
- Errores y excepciones
- Uso de memoria y CPU

## 💰 Modelo de Costos

### MVP (Tier Gratuito)
- **Fly.io**: $0/mes (plan gratuito)
- **Cloudflare**: $0/mes (plan gratuito)
- **Supabase**: $0/mes (plan gratuito)
- **OpenAI**: $0.50-10/mes (según uso)

**Total: $0.50-10/mes**

### Producción Escalable
- **Fly.io**: $5-20/mes
- **Cloudflare**: $0-20/mes
- **Supabase**: $25/mes (plan Pro)
- **OpenAI**: $10-50/mes

**Total: $40-115/mes**

## 🔒 Seguridad

### Características Implementadas

- ✅ Autenticación JWT con Supabase
- ✅ Row Level Security (RLS) en base de datos
- ✅ Validación de entrada en todos los endpoints
- ✅ Rate limiting y throttling
- ✅ Headers de seguridad (HSTS, CSP, etc.)
- ✅ Secrets gestionados de forma segura
- ✅ Scanning automático de vulnerabilidades

### Compliance

- GDPR ready con manejo de datos personales
- SOC 2 Type II a través de Supabase
- Encriptación en tránsito y reposo
- Auditoría de accesos y cambios

## 🤝 Contribución

### Desarrollo

1. Fork el repositorio
2. Crea una rama feature (`git checkout -b feature/nueva-funcionalidad`)
3. Commit tus cambios (`git commit -am 'Agregar nueva funcionalidad'`)
4. Push a la rama (`git push origin feature/nueva-funcionalidad`)
5. Crea un Pull Request

### Estándares de Código

- **Backend**: Clean Architecture, SOLID principles
- **Frontend**: Functional components, TypeScript estricto
- **Tests**: Cobertura mínima 80%
- **Documentación**: Inline comments + README actualizado

## 📚 Documentación

- [Guía de Despliegue](docs/DEPLOYMENT.md)
- [Documentación de API](https://autodocops-api.fly.dev/swagger)
- [Arquitectura del Sistema](docs/ARCHITECTURE.md)
- [Guía de Contribución](docs/CONTRIBUTING.md)
- [Changelog](CHANGELOG.md)

## 🛣️ Roadmap

### v1.1 (Q1 2025)
- [ ] Soporte para FastAPI (Python)
- [ ] Integración con Azure DevOps
- [ ] Temas personalizables para documentación
- [ ] API de webhooks para CI/CD

### v1.2 (Q2 2025)
- [ ] Soporte para MySQL y PostgreSQL
- [ ] Generación de tests automáticos
- [ ] Integración con Slack/Teams
- [ ] Dashboard de analytics avanzado

### v2.0 (Q3 2025)
- [ ] Soporte multi-tenant
- [ ] Marketplace de templates
- [ ] IA para sugerencias de mejoras
- [ ] Integración con IDEs (VS Code, JetBrains)

## 📄 Licencia

Este proyecto está licenciado bajo la Licencia MIT - ver el archivo [LICENSE](LICENSE) para detalles.

## 👥 Equipo

**Desarrollado por**: AutoDocOps Team  
**Arquitecto Principal**: Manus AI  
**Versión**: 1.0.0  
**Última Actualización**: Enero 2025  

## 📞 Soporte

- **Email**: support@autodocops.com
- **Discord**: [AutoDocOps Community](https://discord.gg/autodocops)
- **Issues**: [GitHub Issues](https://github.com/tu-usuario/AutoDocOps/issues)
- **Documentación**: [docs.autodocops.com](https://docs.autodocops.com)

---

<div align="center">

**¿Te gusta AutoDocOps? ¡Dale una ⭐ en GitHub!**

[🌐 Website](https://autodocops.com) • [📚 Docs](https://docs.autodocops.com) • [💬 Discord](https://discord.gg/autodocops) • [🐦 Twitter](https://twitter.com/autodocops)

</div>

