# Estágio 1: build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY APIFORD/*.csproj ./APIFORD/
RUN dotnet restore APIFORD/APIFORD.csproj

COPY . .
RUN dotnet publish APIFORD/APIFORD.csproj -c Release -o /app/publish

# Estágio 2: runtime (imagem final, bem menor que a de build)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENTRYPOINT ["sh", "-c", "ASPNETCORE_URLS=http://+:${PORT:-8080} dotnet APIFORD.dll"]
