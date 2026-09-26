# Testes

Voltar ao [README central](../README.md). Texto operacional também em [`APIFORD.Tests/README.md`](../APIFORD.Tests/README.md).

## O que existe

### Unitário (`APIFORD.Tests/Unit`) — sem banco

- `ExceptionMiddlewareTests` — cada exceção de domínio → status certo; 500 sem vazar detalhe; corpo `{ status, message }`.
- `TokenServiceTests` — claims, role, expiração 12 h / customizada, assinatura aceita com a chave certa e recusada com chave errada.

### Integração (`APIFORD.Tests/Integration`) — API real em memória

Sobe `WebApplicationFactory<Program>`. Precisa de **Postgres** (jsonb / `.ToJson()` não rodam no InMemory).

- `AuthFlowTests` — cadastro/login: 200, 409 e-mail duplicado, 400 e-mail inválido, 401 senha errada, 404 usuário inexistente.
- `AuthorizationTests` — `GET /Carro/listar` 200 sem token; `POST /Carro/registrar` 401 sem token / token lixo; 403 com usuário comum; 200 com Admin; 404 carro inexistente.

O factory cria um banco com nome aleatório e apaga no dispose.

## Como rodar

```bash
# só unitário
dotnet test --filter "FullyQualifiedName~Unit"

# tudo (sobe Postgres se ainda não tiver)
docker run --rm -d --name apiford-postgres-teste \
  -e POSTGRES_USER=postgres -e POSTGRES_PASSWORD=postgres \
  -p 5432:5432 postgres:16

# se não for localhost:5432 / postgres/postgres
export APIFORD_TEST_CONNECTION="Host=localhost;Port=5432;Database=apiford_testes;Username=postgres;Password=postgres"

dotnet test
```

## Evidência para a sprint

A rubrica pede prova de execução. O terminal do `dotnet test` já serve. Para arquivo:

```bash
dotnet test --logger "trx;LogFileName=resultados.trx"
```

O `.trx` sai em `APIFORD.Tests/TestResults/`. Anexar print do resumo + o trx na tarefa do Teams.
