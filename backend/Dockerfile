# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files
COPY src/AutoDocOps.Api/AutoDocOps.Api/AutoDocOps.Api.csproj src/AutoDocOps.Api/AutoDocOps.Api/
RUN dotnet restore src/AutoDocOps.Api/AutoDocOps.Api/AutoDocOps.Api.csproj

# Copy source code
COPY . .
WORKDIR /src/src/AutoDocOps.Api/AutoDocOps.Api
RUN dotnet build -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

# Create a non-root user
RUN groupadd -r appuser && useradd -r -g appuser appuser

WORKDIR /app
COPY --from=publish /app/publish .

# Change ownership of the app directory to the non-root user
RUN chown -R appuser:appuser /app

# Switch to non-root user
USER appuser

# Expose port
EXPOSE 8080
ENV ASPNETCORE_URLS=http://0.0.0.0:8080

ENTRYPOINT ["dotnet", "AutoDocOps.Api.dll"]

