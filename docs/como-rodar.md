# Como rodar

Voltar ao [README central](../README.md). Secrets em [configuracao.md](configuracao.md).

## Pré-requisitos

- .NET 10 SDK
- PostgreSQL 16 (local ou Docker)
- Git
- (Opcional) microserviço Python de busca, se for testar `POST /Pesquisa/busca`

## Local

```bash
git clone <url-do-repo>
cd APIFORD

docker run --name apiford-db -e POSTGRES_PASSWORD=postgres -p 5432:5432 -d postgres:16

dotnet user-secrets set "ConnectionStrings:DefaultConnection" \
  "Host=localhost;Port=5432;Database=APIFORD;Username=postgres;Password=postgres" --project APIFORD
dotnet user-secrets set "SymmetricSecurityKey" "troque-por-uma-chave-com-pelo-menos-32-chars" --project APIFORD

dotnet restore
dotnet ef database update --project APIFORD
dotnet run --project APIFORD
```

Swagger: `https://localhost:<porta>/swagger` (porta no `launchSettings.json`).

Front local (`http://localhost:5173`) já está no CORS, junto com `https://beyond-compare.vercel.app`.

## Docker da API

`Dockerfile` multi-stage (.NET 10 SDK → `aspnet:10.0`). Expõe `8080` e respeita `PORT`.

```bash
docker build -t apiford .
docker run --rm -p 8080:8080 \
  -e ConnectionStrings__DefaultConnection="Host=host.docker.internal;Port=5432;Database=APIFORD;Username=postgres;Password=postgres" \
  -e SymmetricSecurityKey="troque-por-uma-chave-longa" \
  apiford
```

A API precisa alcançar o Postgres (e o Python, se a busca estiver ligada).

## Testes

Ver [testes.md](testes.md). Atalho:

```bash
dotnet test --filter "FullyQualifiedName~Unit"   # sem banco
dotnet test                                      # unitário + integração (precisa Postgres)
```
