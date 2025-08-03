# Dockerfile simplificado para Render
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 10000

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/AutoDocOps.Api/AutoDocOps.Api/AutoDocOps.Api.csproj", "AutoDocOps.Api/"]
COPY ["src/AutoDocOps.Application/AutoDocOps.Application/AutoDocOps.Application.csproj", "AutoDocOps.Application/"]
COPY ["src/AutoDocOps.Domain/AutoDocOps.Domain/AutoDocOps.Domain.csproj", "AutoDocOps.Domain/"]
COPY ["src/AutoDocOps.Infrastructure/AutoDocOps.Infrastructure/AutoDocOps.Infrastructure.csproj", "AutoDocOps.Infrastructure/"]

RUN dotnet restore "AutoDocOps.Api/AutoDocOps.Api.csproj"
COPY . .
WORKDIR "/src/src/AutoDocOps.Api/AutoDocOps.Api"
RUN dotnet build "AutoDocOps.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AutoDocOps.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENV ASPNETCORE_URLS=http://0.0.0.0:10000
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "AutoDocOps.Api.dll"]

