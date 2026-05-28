# syntax=docker/dockerfile:1

# ---- build ----------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Restore como camada cacheável: copia só os .csproj primeiro
COPY ObsidianERP.Api/ObsidianERP.Api.csproj ObsidianERP.Api/
COPY ObsidianERP.Application/ObsidianERP.Application.csproj ObsidianERP.Application/
COPY ObsidianERP.Domain/ObsidianERP.Domain.csproj ObsidianERP.Domain/
COPY ObsidianERP.Infrastructure/ObsidianERP.Infrastructure.csproj ObsidianERP.Infrastructure/
RUN dotnet restore ObsidianERP.Api/ObsidianERP.Api.csproj

# Copia o restante do código e publica
COPY . .
RUN dotnet publish ObsidianERP.Api/ObsidianERP.Api.csproj \
    -c Release --no-restore -o /app/publish /p:UseAppHost=false

# ---- runtime --------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final

# curl é usado pelo healthcheck do compose
RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /app
COPY --from=build /app/publish ./

ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

# Executa como usuário não-root (definido na imagem base do .NET)
USER $APP_UID

ENTRYPOINT ["dotnet", "ObsidianERP.Api.dll"]
