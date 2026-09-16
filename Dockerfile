# Estágio 1: build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY APIFORD/*.csproj ./APIFORD/
RUN dotnet restore

COPY . .
RUN dotnet publish -c Release -o /app/publish

# Estágio 2: runtime (imagem final, bem menor que a de build)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENTRYPOINT ["sh", "-c", "ASPNETCORE_URLS=http://+:${PORT:-8080} dotnet APIFORD.dll"]
