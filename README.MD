q<p align="center">
  <img src="https://raw.githubusercontent.com/LUMEN-7/images/refs/heads/main/logo.png" width="200" />
</p>

# APIFORD

<p align="start">
  <b>API desenvolvida em C# com .NET 10 focada em arquitetura escalável, autenticação, gerenciamento de usuários, integração com banco de dados SQL Server e estrutura baseada em Services + DTOs + AutoMapper.</b><br/>
  Projeto estruturado para servir como API Gateway/BFF, oferecendo uma arquitetura limpa, reutilizável e preparada para expansão.
</p>

---

![Status](https://img.shields.io/badge/STATUS-EM%20DESENVOLVIMENTO-0562D2?style=for-the-badge\&labelColor=00142E)
![Version](https://img.shields.io/badge/VERSION-1.0.0-0562D2?style=for-the-badge\&labelColor=00142E)
![.NET](https://img.shields.io/badge/.NET-10-purple?style=for-the-badge\&logo=dotnet\&labelColor=black)
![CSharp](https://img.shields.io/badge/C%23-239120?style=for-the-badge\&logo=csharp\&logoColor=white\&labelColor=black)
![SQLServer](https://img.shields.io/badge/SQL%20Server-red?style=for-the-badge\&logo=microsoftsqlserver\&logoColor=white\&labelColor=black)

---

## 🔗 Links úteis

* 📄 Documentação via Swagger
* 🔐 Sistema de autenticação com Identity
* 🧠 Arquitetura baseada em Services e DTOs
* ⚡ Estrutura preparada para escalabilidade

---

## ✨ Funcionalidades

* 🔐 Sistema de autenticação utilizando ASP.NET Identity
* 🧩 Arquitetura baseada em Services genéricos
* 📦 CRUD genérico reutilizável com Generic Base Service
* 🧠 AutoMapper para mapeamento entre entidades e DTOs
* 📄 Documentação automática com Swagger
* 🗃️ Integração com SQL Server via Entity Framework Core
* 🧹 Middleware global para tratamento de exceções
* 🔄 Soft Delete de entidades
* 🕶️ Sistema de anonimização de dados
* 📡 Estrutura pronta para JWT Authentication
* ⚡ Compressão de respostas HTTP
* 📚 Conversão automática de ENUM para string em JSON
* 🧱 Arquitetura preparada para expansão de módulos e microsserviços

---

## 🧱 Arquitetura

```text
Cliente / Frontend
        ↓
Controllers (API)
        ↓
Services
        ↓
Entity Framework Core
        ↓
SQL Server
```

### Estrutura arquitetural utilizada

* Controllers responsáveis apenas pela camada HTTP
* Services contendo regras de negócio
* DTOs para transferência segura de dados
* AutoMapper para conversão entre DTOs e entidades
* Entity Framework Core para persistência
* Middleware global para exceções
* ASP.NET Identity para autenticação e gerenciamento de usuários
* Dependency Injection nativa do .NET

---

## 🛠️ Tecnologias

### Backend

[![CSharp](https://img.shields.io/badge/C%23-239120?style=for-the-badge\&logo=csharp\&logoColor=white\&labelColor=black)](https://learn.microsoft.com/dotnet/csharp/)
[![DotNet](https://img.shields.io/badge/.NET-512BD4?style=for-the-badge\&logo=dotnet\&logoColor=white\&labelColor=black)](https://dotnet.microsoft.com/)
[![ASP.NET](https://img.shields.io/badge/ASP.NET-5C2D91?style=for-the-badge\&logo=dotnet\&logoColor=white\&labelColor=black)](https://dotnet.microsoft.com/apps/aspnet)
[![EntityFramework](https://img.shields.io/badge/Entity%20Framework-6DB33F?style=for-the-badge\&logo=.net\&logoColor=white\&labelColor=black)](https://learn.microsoft.com/ef/)
[![SQLServer](https://img.shields.io/badge/SQL%20Server-CC2927?style=for-the-badge\&logo=microsoftsqlserver\&logoColor=white\&labelColor=black)](https://www.microsoft.com/sql-server)

### Bibliotecas e Ferramentas

[![Swagger](https://img.shields.io/badge/Swagger-85EA2D?style=for-the-badge\&logo=swagger\&logoColor=black\&labelColor=black)](https://swagger.io/)
[![AutoMapper](https://img.shields.io/badge/AutoMapper-blue?style=for-the-badge\&labelColor=black)](https://automapper.org/)
[![JWT](https://img.shields.io/badge/JWT-black?style=for-the-badge\&logo=jsonwebtokens\&logoColor=white\&labelColor=black)](https://jwt.io/)
[![Git](https://img.shields.io/badge/Git-F05032?style=for-the-badge\&logo=git\&logoColor=white\&labelColor=black)](https://git-scm.com/)
[![GitHub](https://img.shields.io/badge/GitHub-181717?style=for-the-badge\&logo=github\&logoColor=white\&labelColor=black)](https://github.com/)

---

## 📂 Estrutura do Projeto

```text
APIFORD/
│
├── Controllers/
├── Data/
│   ├── DTOs/
│   └── FordDbContext.cs
│
├── Middleware/
├── Model/
├── Perfils/
├── Services/
│   ├── Interfaces/
│   └── BaseService.cs
│
├── Migrations/
├── appsettings.json
├── Program.cs
└── APIFORD.csproj
```

---

## ⚙️ Configurações Principais

### Banco de Dados

O projeto utiliza SQL Server com Entity Framework Core.

Exemplo de connection string:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=APIFORD;Trusted_Connection=True;TrustServerCertificate=True"
}
```

---

### Chave JWT

A aplicação está preparada para autenticação JWT.

```json
"SymmetricSecurityKey": "SUA_CHAVE_SECRETA"
```

---

## 🚀 Como rodar o projeto

### 🔧 Pré-requisitos

* .NET 10 SDK
* SQL Server
* Git
* Visual Studio 2022+ ou VS Code

---

### ▶️ Clonar o repositório

```bash
git clone https://github.com/user/APIFORD.git
```

---

### ▶️ Entrar na pasta do projeto

```bash
cd APIFORD
```

---

### ▶️ Restaurar dependências

```bash
dotnet restore
```

---

### ▶️ Aplicar migrations

```bash
dotnet ef database update
```

---

### ▶️ Executar o projeto

```bash
dotnet run
```

---

## 📄 Swagger

Após iniciar o projeto, acesse:

```text
https://localhost:xxxx/swagger
```

A documentação será gerada automaticamente.

---

## 🔐 Segurança

A aplicação possui estrutura preparada para:

* JWT Authentication
* ASP.NET Identity
* Authorization Policies
* Middleware de tratamento de exceções
* Soft Delete
* Anonimização de dados

---

## 🧠 Generic Base Service

O projeto utiliza um serviço genérico reutilizável:

```csharp
BaseService<TEntity, TCreateDTO, TReadDTO, TUpdateDTO, TKey>
```

Esse serviço oferece:

* CreateAsync
* GetAllAsync
* GetByIdAsync
* UpdateAsync
* SoftDeleteAsync
* AnonymizeAsync

Permitindo reaproveitamento de código e padronização das operações CRUD.

---

## 🧪 Executar Testes

```bash
dotnet test
```

---

## 📈 Roadmap

* [x] Estrutura inicial da API
* [x] Integração com SQL Server
* [x] ASP.NET Identity
* [x] Swagger Documentation
* [x] Generic Base Service
* [x] Middleware global de exceções
* [ ] JWT Authentication completo
* [ ] Logs centralizados
* [ ] Testes automatizados
* [ ] Dockerização
* [ ] CI/CD
* [ ] Rate Limiting
* [ ] Cache distribuído
* [ ] Integração com microsserviços

---

## 🧩 Padrões Utilizados

* Repository Pattern (parcial)
* Service Layer Pattern
* Dependency Injection
* DTO Pattern
* Middleware Pattern
* SOLID
* Clean Architecture Concepts

---

## 📚 Pacotes Utilizados

```text
AutoMapper
EntityFrameworkCore
Microsoft.AspNetCore.Identity
Microsoft.AspNetCore.Authentication.JwtBearer
Swashbuckle.AspNetCore
Newtonsoft.Json
```

---

## 🛡️ Middleware Global de Exceções

O projeto possui middleware personalizado responsável por:

* Captura global de erros
* Retorno padronizado
* Tratamento de exceções inesperadas
* Melhor organização da API

---

## 📌 Observações

* O projeto foi estruturado visando reutilização e escalabilidade.
* A arquitetura facilita a criação de novos módulos.
* A camada de serviços reduz duplicação de código.
* O uso de DTOs melhora segurança e desacoplamento.
* A estrutura suporta expansão futura para microsserviços.

---

## 🤝 Como contribuir

1. Fork o projeto
2. Crie uma branch

```bash
git checkout -b feature/minha-feature
```

3. Commit suas alterações

```bash
git commit -m "feat: minha nova feature"
```

4. Push da branch

```bash
git push origin feature/minha-feature
```

5. Abra um Pull Request

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

## 📜 Licença

Este projeto está sob a licença MIT.

---

<p align="center">
  Desenvolvido com ❤️ utilizando C#, .NET 10 e SQL Server.
</p>
