# Configuração

Voltar ao [README central](../README.md).

Não commitar senha, chave JWT, API key nem connection string. Preferir **User Secrets** (dev) ou variável de ambiente (deploy).

```bash
dotnet user-secrets set "CHAVE" "valor" --project APIFORD
```

No ambiente, o .NET aceita `__` no lugar de `:` (`ConnectionStrings__DefaultConnection`).

## Obrigatório para a API subir

| Chave | Uso |
|---|---|
| `ConnectionStrings:DefaultConnection` | Postgres (Host, Port, Database, Username, Password) |
| `SymmetricSecurityKey` | Assinatura HMAC do JWT — longa, estável entre deploys |

`TokenExpiracaoHoras` default 12 se omitido.

## Opcional conforme o módulo

| Chave | Módulo |
|---|---|
| `GoogleClientId` | `POST /User/login/google` |
| `EmailSettings:*` | SMTP (host, porta, remetente). Senha do Gmail **não** vai no `appsettings` versionado |
| `Armazenamento:Endpoint`, `BucketName`, `UrlPublicaBase` + credenciais S3/R2 | foto de perfil, imagens de anotação |
| `Python:Url` | base do microserviço de busca |
| `PythonInternalApiKey` | header `X-Api-Key` da API → Python |
| chave lida por `InternalApiKeyAttribute` | header `X-Internal-Api-Key` no `POST /internal/carros/notificar-atualizacao` |

## O que **não** configurar no Git

O `appsettings.json` versionado ainda tem IDs de cliente Google e endpoint R2. Trate isso como legado: o certo é secret / env no deploy e valor vazio no arquivo commitado.

## CORS

Política `FrontEnd`:

- `http://localhost:5173`
- `https://beyond-compare.vercel.app`

Origem nova = incluir em `Program.cs`.
