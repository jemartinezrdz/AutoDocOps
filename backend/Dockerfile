# Dockerfile para AutoDocOps Backend
# Usa imagen base de .NET 8 con AOT support
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Configurar usuario no-root para seguridad
RUN addgroup -g 1001 -S appgroup && \
    adduser -S appuser -u 1001 -G appgroup
USER appuser

# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /src

# Copiar archivos de proyecto y restaurar dependencias
COPY ["src/AutoDocOps.Api/AutoDocOps.Api/AutoDocOps.Api.csproj", "src/AutoDocOps.Api/AutoDocOps.Api/"]
COPY ["src/AutoDocOps.Application/AutoDocOps.Application/AutoDocOps.Application.csproj", "src/AutoDocOps.Application/AutoDocOps.Application/"]
COPY ["src/AutoDocOps.Infrastructure/AutoDocOps.Infrastructure/AutoDocOps.Infrastructure.csproj", "src/AutoDocOps.Infrastructure/AutoDocOps.Infrastructure/"]
COPY ["src/AutoDocOps.Domain/AutoDocOps.Domain/AutoDocOps.Domain.csproj", "src/AutoDocOps.Domain/AutoDocOps.Domain/"]
COPY ["AutoDocOps.sln", "./"]

RUN dotnet restore "src/AutoDocOps.Api/AutoDocOps.Api/AutoDocOps.Api.csproj"

# Copiar código fuente
COPY . .

# Build de la aplicación
WORKDIR "/src/src/AutoDocOps.Api/AutoDocOps.Api"
RUN dotnet build "AutoDocOps.Api.csproj" -c Release -o /app/build

# Etapa de publish
FROM build AS publish
RUN dotnet publish "AutoDocOps.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Etapa final
FROM base AS final
WORKDIR /app

# Copiar archivos publicados
COPY --from=publish /app/publish .

# Variables de entorno
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_HTTP_PORTS=8080

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=60s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

# Instalar curl para health check
USER root
RUN apk add --no-cache curl
USER appuser

# Punto de entrada
ENTRYPOINT ["dotnet", "AutoDocOps.Api.dll"]

# Labels para metadatos
LABEL org.opencontainers.image.title="AutoDocOps API"
LABEL org.opencontainers.image.description="API para generación automática de documentación viva"
LABEL org.opencontainers.image.version="1.0.0"
LABEL org.opencontainers.image.vendor="AutoDocOps Team"
LABEL org.opencontainers.image.licenses="MIT"

