FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY Bank.Api/Bank.Api.csproj Bank.Api/
RUN dotnet restore Bank.Api/Bank.Api.csproj
COPY . .
RUN dotnet publish Bank.Api/Bank.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "Bank.Api.dll"]
