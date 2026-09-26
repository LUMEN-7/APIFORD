<p align="center">
  <img src="https://raw.githubusercontent.com/LUMEN-7/images/refs/heads/main/logo.png" width="200" />
</p>

# APIFORD

API em **ASP.NET Core / .NET 10** do projeto **Beyond Compare (BCI)** — Ford FIAP Challenge 2026, Desafio 01 (inteligência competitiva automotiva).

Orquestra catálogo de veículos, busca (microserviço Python), comparação, exportação, workspace, agendamentos e conta do usuário. Persistência em **PostgreSQL**. Autenticação **JWT** (Identity + Google + 2FA).

![Status](https://img.shields.io/badge/STATUS-EM%20DESENVOLVIMENTO-0562D2?style=for-the-badge&labelColor=00142E)
![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?style=for-the-badge&labelColor=00142E)
![Postgres](https://img.shields.io/badge/PostgreSQL-Npgsql-336791?style=for-the-badge&labelColor=00142E)

---

| Tema | O que tem |
|---|---|
| [Arquitetura (SOA)](docs/arquitetura.md) | Camadas, diagrama, workers, integração Python |
| [Autenticação e JWT](docs/autenticacao.md) | Público × autenticado × Admin, claims, expiração |
| [Módulos e rotas](docs/modulos.md) | Controllers e o que cada um faz |
| [Como rodar](docs/como-rodar.md) | Local, Docker, migrations |
| [Configuração](docs/configuracao.md) | Connection string, JWT, e-mail, R2, Python — via secrets |
| [API, Swagger e erros](docs/api-e-erros.md) | OpenAPI, formato `{ status, message }`, prints |
| [Testes](docs/testes.md) | Unitário, integração, como gerar evidência |

Testes também têm um guia curto em [`APIFORD.Tests/README.md`](APIFORD.Tests/README.md).

---

## Tecnologias

### Backend

[![CSharp](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=csharp&logoColor=white&labelColor=black)](https://learn.microsoft.com/dotnet/csharp/)
[![DotNet](https://img.shields.io/badge/.NET-512BD4?style=for-the-badge&logo=dotnet&logoColor=white&labelColor=black)](https://dotnet.microsoft.com/)
[![ASP.NET](https://img.shields.io/badge/ASP.NET-5C2D91?style=for-the-badge&logo=dotnet&logoColor=white&labelColor=black)](https://dotnet.microsoft.com/apps/aspnet)
[![EntityFramework](https://img.shields.io/badge/Entity%20Framework-6DB33F?style=for-the-badge&logo=.net&logoColor=white&labelColor=black)](https://learn.microsoft.com/ef/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-4169E1?style=for-the-badge&logo=postgresql&logoColor=white&labelColor=black)](https://www.postgresql.org/)

### Bibliotecas e Ferramentas

[![Swagger](https://img.shields.io/badge/Swagger-85EA2D?style=for-the-badge&logo=swagger&logoColor=black&labelColor=black)](https://swagger.io/)
[![AutoMapper](https://img.shields.io/badge/AutoMapper-blue?style=for-the-badge&labelColor=black)](https://automapper.org/)
[![JWT](https://img.shields.io/badge/JWT-black?style=for-the-badge&logo=jsonwebtokens&logoColor=white&labelColor=black)](https://jwt.io/)
[![Git](https://img.shields.io/badge/Git-F05032?style=for-the-badge&logo=git&logoColor=white&labelColor=black)](https://git-scm.com/)
[![GitHub](https://img.shields.io/badge/GitHub-181717?style=for-the-badge&logo=github&logoColor=white&labelColor=black)](https://github.com/)

---

## Arranque rápido

```bash
# Postgres
docker run --name apiford-db -e POSTGRES_PASSWORD=postgres -p 5432:5432 -d postgres:16

# secrets (nunca commitar chave/senha)
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=APIFORD;Username=postgres;Password=postgres" --project APIFORD
dotnet user-secrets set "SymmetricSecurityKey" "troque-por-uma-chave-longa" --project APIFORD

dotnet restore
dotnet ef database update --project APIFORD
dotnet run --project APIFORD
```

Swagger: `https://localhost:<porta>/swagger`  
Testes: `dotnet test` (detalhes em [docs/testes.md](docs/testes.md))

---

## Solução

```text
APIFORD.slnx
├── APIFORD/                 API
└── APIFORD.Tests/           unitário + integração
```

Front esperado no CORS: `http://localhost:5173` e `https://beyond-compare.vercel.app`.

---

<h2>
  Conheça o time
  <img src="https://raw.githubusercontent.com/LUMEN-7/images/refs/heads/main/logo-lumen.png" width="200" align="center" />
</h2>

| Foto                                                                                                     | Nome                                                                      | RM        |
| -------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------- | --------- |
| <img src="https://avatars.githubusercontent.com/AnaTorresLoureiro" width="80" style="border-radius:50%;">| [Ana Laura Torres Loureiro](https://github.com/AnaTorresLoureiro)         | RM 554375 |
| <img src="https://avatars.githubusercontent.com/MuriloCngp" width="80" style="border-radius:50%;">       | [Murilo Cordeiro Ferreira](https://github.com/MuriloCngp)                 | RM 556727 |
| <img src="https://avatars.githubusercontent.com/Geronimo-augusto" width="80" style="border-radius:50%;"> | [Geronimo Augusto Nascimento Santos](https://github.com/Geronimo-augusto) | RM 557170 |
| <img src="https://avatars.githubusercontent.com/iannyrfs" width="80" style="border-radius:50%;">         | [Ianny Raquel Ferreira De Souza](https://github.com/iannyrfs)             | RM 559096 |
| <img src="https://avatars.githubusercontent.com/Vitorr-AF" width="80" style="border-radius:50%;">        | [Vitor Augusto França de Oliveira](https://github.com/Vitorr-AF)          | RM 555469 |


---

<p align="center">
  Desenvolvido utilizando C#, .NET 10 e SQL Server.
</p>
