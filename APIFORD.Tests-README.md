# Testes automatizados — APIFORD.Tests

Guia completo: [docs/testes.md](../docs/testes.md).

Atalho:

```bash
dotnet test --filter "FullyQualifiedName~Unit"
dotnet test
dotnet test --logger "trx;LogFileName=resultados.trx"
```

Integração precisa de Postgres. Variável opcional: `APIFORD_TEST_CONNECTION`.
