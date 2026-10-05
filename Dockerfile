FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY Bank.Api/Bank.Api.csproj Bank.Api/
RUN dotnet restore Bank.Api/Bank.Api.csproj

COPY Bank.Api/ Bank.Api/
RUN dotnet publish Bank.Api/Bank.Api.csproj \
    --configuration Release \
    --output /app/publish \
    --no-restore \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

RUN apt-get update \
    && apt-get install --yes --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

COPY --from=build --chown=app:app /app/publish .

ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

USER app

HEALTHCHECK --interval=15s --timeout=5s --start-period=30s --retries=5 \
    CMD curl --fail --silent http://localhost:8080/api/health > /dev/null || exit 1

ENTRYPOINT ["dotnet", "Bank.Api.dll"]
