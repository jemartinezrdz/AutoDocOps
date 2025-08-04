# AutoDocOps - Sistema de Documentación Automática

AutoDocOps es una plataforma avanzada que automatiza la generación de documentación para proyectos de software, implementando las mejores prácticas de desarrollo, rendimiento y seguridad.

## 🏗️ Arquitectura

El proyecto implementa **Clean Architecture (Arquitectura Hexagonal)** con las siguientes capas:

- **Domain**: Lógica de negocio central, entidades y objetos de valor
- **Application**: Casos de uso, interfaces y DTOs (usando MediatR + Mapster)
- **Infrastructure**: Dependencias externas (PostgreSQL, Supabase, APIs)
- **API**: Controladores, middleware y configuración

## 🚀 Tecnologías Principales

### Backend (.NET 8)
- **Clean Architecture** con MediatR y Mapster
- **System.Text.Json Source Generators** para rendimiento optimizado
- **Serilog** con filtrado de PII para logging seguro
- **JWT Authentication** con Supabase
- **Health Checks** con métricas detalladas
- **OpenAPI/Swagger** con documentación automática

### Frontend (React/Expo)
- **TypeScript estricto** con configuración avanzada
- **ESLint + Prettier** para calidad de código
- **Componentes funcionales** con hooks
- **Lazy loading** y optimizaciones de rendimiento

### Base de Datos (PostgreSQL)
- **Row Level Security (RLS)** para autorización
- **Índices compuestos** para consultas optimizadas
- **UUID v7** para mejor rendimiento
- **Connection pooling** con Npgsql

### Infraestructura
- **Terraform** para Infrastructure as Code
- **Fly.io** para deployment de aplicaciones
- **Cloudflare** para CDN y seguridad
- **Supabase** para autenticación y base de datos

## 🛡️ Seguridad

### Medidas Implementadas
- **TLS 1.3** con HSTS obligatorio
- **Content Security Policy (CSP)** estricta
- **Headers de seguridad** completos (X-Frame-Options, X-Content-Type-Options, etc.)
- **Autenticación JWT** con renovación cada 24h
- **Gestión de secretos** con Doppler (dev/CI) y Vault (prod)
- **Escaneo de dependencias** semanal con Dependabot
- **Análisis de seguridad** con OWASP ZAP

## 🧪 Testing

### Cobertura de Pruebas
- **Unit Tests**: xUnit (backend) + Vitest (frontend) - **80% cobertura mínima**
- **Contract Tests**: Validación de endpoints OpenAPI - **100% cobertura**
- **E2E Tests**: Playwright con 2 escenarios principales
- **Load Tests**: k6 para 100 repositorios concurrentes - **p95 < 300ms**
- **Security Tests**: OWASP ZAP - **0 hallazgos críticos**

### Pipeline CI/CD
```yaml
1. Build & Unit Tests (Backend + Frontend)
2. Contract Tests (API endpoint validation)
3. E2E Tests (Playwright scenarios)
4. Load Tests (k6 performance testing)
5. Security Tests (OWASP ZAP scanning)
6. Deploy to Production (if main branch)
7. Post-deployment Health Checks
8. Performance Monitoring
```

## 📊 Objetivos de Rendimiento

| Métrica | Objetivo | Implementación |
|---------|----------|----------------|
| API p95 | < 200ms | AOT compilation, JSON Source Generators |
| DB Queries | < 10ms | Índices compuestos, UUID v7, connection pooling |
| First Load | < 1.5s | Lazy loading, React 18, CDN Cloudflare |
| Assets Cache | > 95% | Cache-control: 1 año, optimización PNG/SVG |

## 🏃‍♂️ Quick Start

### Prerrequisitos
- .NET 8 SDK
- Node.js 20+
- PostgreSQL 15+
- Docker (opcional)

### Backend
```bash
cd backend
dotnet restore
dotnet build
dotnet test
dotnet run --project src/AutoDocOps.Api/AutoDocOps.Api
```

### Frontend
```bash
cd frontend/AutoDocOps-Frontend
npm install
npm run lint
npm run type-check
npm run test
npm start
```

### Tests E2E
```bash
npx playwright install
npx playwright test
```

### Load Testing
```bash
k6 run tests/load/api-load-test.js
```

## 📁 Estructura del Proyecto

```
AutoDocOps/
├── backend/                    # API .NET 8
│   ├── src/
│   │   ├── AutoDocOps.Domain/     # Entidades y lógica de negocio
│   │   ├── AutoDocOps.Application/ # Casos de uso (MediatR)
│   │   ├── AutoDocOps.Infrastructure/ # Datos y servicios externos
│   │   └── AutoDocOps.Api/        # API Controllers y configuración
│   └── tests/                     # Tests unitarios e integración
├── frontend/                   # React/Expo App
│   └── AutoDocOps-Frontend/
├── infrastructure/             # Terraform IaC
├── tests/                      # Tests E2E y Load
├── docs/                       # Documentación y ADRs
└── .github/workflows/          # CI/CD Pipelines
```

## 📚 Documentación

### Architecture Decision Records (ADRs)
- [ADR-001: Clean Architecture](docs/adrs/001-clean-architecture.md)
- [ADR-002: API Versioning](docs/adrs/002-api-versioning.md)
- [ADR-003: Performance Optimization](docs/adrs/003-performance-optimization.md)
- [ADR-004: Security Strategy](docs/adrs/004-security-strategy.md)

### API Documentation
- Swagger UI: `/swagger` (development)
- OpenAPI Spec: `/swagger/v1/swagger.json`
- Health Check: `/health`

## 🔧 Configuración

### Variables de Entorno
```bash
# Database
ConnectionStrings__DefaultConnection=...

# Supabase
Supabase__Url=...
Supabase__AnonKey=...
Supabase__ServiceRoleKey=...

# CORS
Cors__AllowedOrigins__0=http://localhost:3000
```

### Desarrollo
Copia `.env.example` a `.env` y configura las variables necesarias.

## 🚀 Deployment

### Terraform
```bash
cd infrastructure
terraform init
terraform plan -var-file="prod.tfvars"
terraform apply -var-file="prod.tfvars"
```

### Fly.io
```bash
flyctl deploy --config fly.toml
```

## 📈 Monitoring

- **Health Checks**: `/health` con métricas detalladas
- **Logging**: Serilog con filtrado de PII
- **Performance**: Métricas de respuesta en tiempo real
- **Security**: Alertas automáticas de vulnerabilidades

## 🤝 Contribución

1. Fork el proyecto
2. Crea una rama feature (`git checkout -b feature/AmazingFeature`)
3. Commit los cambios (`git commit -m 'Add AmazingFeature'`)
4. Push a la rama (`git push origin feature/AmazingFeature`)
5. Abre un Pull Request

### Estándares de Código
- Usa EditorConfig para formateo consistente
- Ejecuta linting antes de commit
- Mantén 80%+ cobertura de tests
- Documenta funciones públicas

## 📝 Licencia

Este proyecto está bajo la Licencia MIT - ver [LICENSE](LICENSE) para detalles.

## 🏆 Estado del Proyecto

[![CI/CD](https://github.com/jemartinezrdz/AutoDocOps/workflows/AutoDocOps%20CI/CD/badge.svg)](https://github.com/jemartinezrdz/AutoDocOps/actions)
[![Security Scan](https://github.com/jemartinezrdz/AutoDocOps/workflows/Security%20Scanning/badge.svg)](https://github.com/jemartinezrdz/AutoDocOps/actions)
[![Code Quality](https://img.shields.io/badge/code%20quality-A+-brightgreen)](https://github.com/jemartinezrdz/AutoDocOps)

---

**AutoDocOps** - Generación automática de documentación con las mejores prácticas de desarrollo moderno.
