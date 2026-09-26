# Autenticação e JWT

Voltar ao [README central](../README.md).

## Três níveis

| Nível | Como | Exemplos |
|---|---|---|
| Público | sem atributo ou `[AllowAnonymous]` | `POST /User/cadastro`, `POST /User/login`, `POST /User/login/google`, `POST /User/esqueci-senha`, `GET /Carro/listar`, `GET /Carro/{id}` |
| Autenticado | `[Authorize]` | pesquisa, exportação, anotação, workspace, equipe, agendamento, 2FA, foto, logout |
| Admin | `[Authorize(Roles = "Admin")]` | `POST /Carro/registrar`, PUT/DELETE/anonimizar carro, `PATCH /Carro/edicao-admin/{id}`, `GET /User/usuarios`, `POST /Notificacao/criar` |

A role `Admin` é criada no startup. O e-mail configurado no seed (se existir no banco) recebe a role.

Usuário comum autenticado **não** é Admin: escrita no catálogo responde **403**, não 401. Isso está coberto em `AuthorizationTests`.

## Emissão do token

`TokenService.GenerateTokenAsync`:

- HMAC-SHA256 com `SymmetricSecurityKey`
- expiração `TokenExpiracaoHoras` (padrão **12 h**)
- claims: `username`, `email`, `NameIdentifier`, `loginTimestamp`, `ClaimTypes.Role` para cada role do Identity

`ClockSkew = TimeSpan.Zero` — token expirado cai na hora.

Issuer e audience **não** são validados (`ValidateIssuer` / `ValidateAudience` = false).

## Como o front manda

```http
Authorization: Bearer <accessToken>
```

SignalR não manda header no handshake. O token vai na query:

```text
/hubs/notificacao?access_token=<accessToken>
```

No Swagger: login → copiar `accessToken` → cadeado **Authorize** (sem escrever `Bearer`).

## Outros fluxos de identidade

- Senha: `POST /User/login`
- Google: `POST /User/login/google` (`GoogleClientId`)
- 2FA TOTP: `POST /User/2fa/iniciar` → confirmar → `POST /User/login/2fa`
- Recuperação: `POST /User/esqueci-senha` e `POST /User/redefinir-senha` (públicos)
- Logout: `POST /User/logout` (autenticado)

## Chave interna (serviço a serviço)

Não é JWT. Workers / Python usam:

- `X-Internal-Api-Key` em `POST /internal/carros/notificar-atualizacao`
- `X-Api-Key` (`PythonInternalApiKey`) nas chamadas da API para o Python

Ver [configuracao.md](configuracao.md).
