# 🚀 Guía para Subir AutoDocOps a tu GitHub

## 📋 Pasos para Transferir el Proyecto

### 1. Descargar el Proyecto del Sandbox

El proyecto completo está comprimido en:
```
/home/ubuntu/AutoDocOps-Complete.tar.gz
```

**Descarga este archivo** desde el sandbox a tu máquina local.

### 2. Extraer y Preparar el Proyecto

```bash
# Extraer el archivo
tar -xzf AutoDocOps-Complete.tar.gz
cd AutoDocOps

# Limpiar archivos temporales (si los hay)
find . -name "bin" -type d -exec rm -rf {} + 2>/dev/null || true
find . -name "obj" -type d -exec rm -rf {} + 2>/dev/null || true
find . -name "node_modules" -type d -exec rm -rf {} + 2>/dev/null || true
```

### 3. Inicializar Repositorio Git

```bash
# Inicializar Git
git init

# Crear .gitignore
cat > .gitignore << 'EOF'
# Build results
[Dd]ebug/
[Dd]ebugPublic/
[Rr]elease/
[Rr]eleases/
x64/
x86/
[Ww][Ii][Nn]32/
[Aa][Rr][Mm]/
[Aa][Rr][Mm]64/
bld/
[Bb]in/
[Oo]bj/
[Ll]og/
[Ll]ogs/

# Visual Studio 2015/2017 cache/options directory
.vs/

# .NET Core
project.lock.json
project.fragment.lock.json
artifacts/

# Node.js
node_modules/
npm-debug.log*
yarn-debug.log*
yarn-error.log*
.pnpm-debug.log*

# Environment variables
.env
.env.local
.env.development.local
.env.test.local
.env.production.local

# IDE
.vscode/
.idea/
*.swp
*.swo

# OS
.DS_Store
Thumbs.db

# Expo
.expo/
dist/
web-build/

# Logs
*.log

# Dependencies
package-lock.json
yarn.lock

# Temporary files
*.tmp
*.temp
EOF

# Agregar archivos
git add .
git commit -m "Initial commit: AutoDocOps MVP complete"
```

### 4. Crear Repositorio en GitHub

1. Ve a [GitHub](https://github.com)
2. Haz clic en "New repository"
3. Nombre: `AutoDocOps`
4. Descripción: `Generador automático de documentación viva para APIs .NET y bases SQL Server`
5. Selecciona "Public" o "Private" según prefieras
6. **NO** inicialices con README (ya tenemos uno)
7. Haz clic en "Create repository"

### 5. Conectar y Subir el Proyecto

```bash
# Agregar remote origin (reemplaza TU-USUARIO con tu username de GitHub)
git remote add origin https://github.com/TU-USUARIO/AutoDocOps.git

# Subir el código
git branch -M main
git push -u origin main
```

### 6. Configurar Secrets para CI/CD (Opcional)

Si quieres usar el CI/CD automático, configura estos secrets en GitHub:

1. Ve a tu repositorio → Settings → Secrets and variables → Actions
2. Agrega estos secrets:

```
DATABASE_URL=postgresql://user:pass@host:5432/autodocops
SUPABASE_URL=https://your-project.supabase.co
SUPABASE_KEY=your-supabase-anon-key
OPENAI_API_KEY=your-openai-api-key
JWT_KEY=your-super-secret-jwt-key-at-least-32-chars
FLY_API_TOKEN=your-fly-api-token
CLOUDFLARE_API_TOKEN=your-cloudflare-api-token
```

## 🔧 Configuración Local para Desarrollo

### Prerrequisitos

Instala en tu máquina local:
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js 18+](https://nodejs.org/)
- [Git](https://git-scm.com/)

### Configurar Backend

```bash
cd backend

# Restaurar dependencias
dotnet restore

# Crear archivo de configuración local
cat > src/AutoDocOps.Api/AutoDocOps.Api/.env << 'EOF'
DATABASE_URL=postgresql://localhost:5432/autodocops_dev
SUPABASE_URL=https://your-project.supabase.co
SUPABASE_KEY=your-supabase-anon-key
OPENAI_API_KEY=your-openai-api-key
JWT_KEY=your-super-secret-jwt-key-at-least-32-chars
JWT_ISSUER=AutoDocOps
JWT_AUDIENCE=AutoDocOps-Users
EOF

# Compilar
dotnet build

# Ejecutar tests
dotnet test

# Ejecutar API
cd src/AutoDocOps.Api/AutoDocOps.Api
dotnet run
```

### Configurar Frontend

```bash
cd frontend/AutoDocOps-Frontend

# Instalar dependencias
npm install

# Crear archivo de configuración local
cat > .env.local << 'EOF'
EXPO_PUBLIC_API_URL=http://localhost:5000
EXPO_PUBLIC_APP_NAME=AutoDocOps
EXPO_PUBLIC_APP_VERSION=1.0.0
EXPO_PUBLIC_ENVIRONMENT=development
EXPO_PUBLIC_SUPABASE_URL=https://your-project.supabase.co
EXPO_PUBLIC_SUPABASE_ANON_KEY=your-supabase-anon-key
EOF

# Ejecutar en web
npm run web

# O ejecutar en móvil
npx expo start
```

## 🚀 Despliegue a Producción

### Opción 1: Despliegue Manual

```bash
# Instalar herramientas
npm install -g @expo/cli wrangler
curl -L https://fly.io/install.sh | sh

# Autenticar
fly auth login
wrangler login

# Desplegar
./scripts/deploy-all.sh production
```

### Opción 2: Despliegue Automático

El proyecto incluye GitHub Actions que despliegan automáticamente:
- Push a `main` → Producción
- Push a `develop` → Staging

## 📁 Estructura del Proyecto

```
AutoDocOps/
├── backend/                    # Backend .NET 8
│   ├── src/
│   │   ├── AutoDocOps.Api/     # API endpoints
│   │   ├── AutoDocOps.Application/ # Lógica de aplicación
│   │   ├── AutoDocOps.Domain/  # Entidades de dominio
│   │   └── AutoDocOps.Infrastructure/ # Servicios externos
│   ├── tests/                  # Tests unitarios e integración
│   └── AutoDocOps.sln         # Solución .NET
├── frontend/                   # Frontend Expo + React
│   └── AutoDocOps-Frontend/    # Aplicación React Native/Web
├── database/                   # Scripts de base de datos
├── scripts/                    # Scripts de despliegue
├── tests/                      # Tests E2E y performance
├── docs/                       # Documentación
├── .github/                    # GitHub Actions
├── fly.toml                    # Configuración Fly.io
└── README.md                   # Documentación principal
```

## 🔗 URLs Importantes

Una vez desplegado, tendrás:
- **API**: https://tu-app.fly.dev
- **Frontend**: https://tu-app.pages.dev
- **Docs**: https://tu-app.fly.dev/swagger

## 📞 Soporte

Si tienes problemas:
1. Revisa la documentación en `/docs/`
2. Verifica los logs con `fly logs` o en Cloudflare
3. Consulta los issues en GitHub
4. Contacta al equipo de desarrollo

---

¡Listo! Ahora tienes AutoDocOps en tu GitHub y puedes desarrollar y desplegar desde tu propia cuenta. 🎉

