# Módulos e rotas

Voltar ao [README central](../README.md).

Rotas abaixo são as que existem hoje no código. Prefixo = `[controller]`, salvo onde indicado. CRUD genérico vem de `BaseController` (`api/[controller]` no base + rota própria no `Carro`).

## Conta — `UserController` (`/User`)

| Método | Rota | Acesso |
|---|---|---|
| POST | `/User/cadastro` | público |
| POST | `/User/login` | público |
| POST | `/User/login/google` | público |
| POST | `/User/login/2fa` | público (depois do desafio) |
| POST | `/User/esqueci-senha`, `/User/redefinir-senha` | público |
| POST | `/User/2fa/iniciar`, `/confirmar`, `/desativar` | autenticado |
| POST | `/User/logout` | autenticado |
| PUT | `/User/atualizar`, `/User/trocar-senha` | autenticado |
| POST/DELETE | `/User/foto-perfil` | autenticado |
| GET | `/User/usuarios` | Admin |

## Histórico do usuário — `/user`

Modelos e comparações salvos: GET/POST/DELETE em `/user/modelos` e `/user/comparacoes`, mais contadores.

## Catálogo — `/Carro` (+ base)

Leitura pública: `GET /Carro/listar`, `GET /Carro/{id}`, `GET /Carro/listarPaginado`, `GET /Carro/recente/{linhagemId}`, `GET /Carro/versao/{carroId}`, `GET /Carro/{linhagemId}/versoes`, `GET /Carro/Imagem-Carro/{carroId}`.

Escrita Admin: `POST /Carro/registrar`, `PUT /Carro/atualizar/{id}`, `DELETE /Carro/deletar/{id}`, `DELETE /Carro/anonimizar/{id}`, `PATCH /Carro/edicao-admin/{id}`, `POST /Carro/importar-arquivo`.

`FonteController` herda o mesmo CRUD genérico para fontes.

## Busca — `/Pesquisa`

`POST /Pesquisa/busca` e `GET /Pesquisa/jobs/{id}` — autenticado. Encaminha ao Python e devolve job.

## Comparação — `/Comparacao`

`POST /Comparacao/grupo` e `POST /Comparacao/direta`.

## Exportação — `/Exportacao`

`GET /Exportacao/conflitos` e `POST /Exportacao` — autenticado. Formatos: JSON, JSON Lines, CSV, XLSX, XML (`IExportadorFormatoService`).

## Anotação — `/Anotacao`

CRUD autenticado da anotação e dos blocos (`/Anotacao/{id}/blocos/...`), upload de imagens.

## Workspace e equipes

`WorkspaceController` e `EquipeController`: posts, atividade, membros, código de entrada, papéis de equipe (`PapelEquipe`).

## Agenda

`/AgendamentoPesquisa` — meus agendamentos, executar agora, ligar/desligar, apagar.  
`/EscutaLancamento` — minhas escutas e delete.

O `AgendamentoWorker` dispara o que estiver vencido.

## Notificação — `/Notificacao` + hub

Lista, marcar lida, criar (Admin). Tempo real em `/hubs/notificacao`.

## Interno

`POST /internal/carros/notificar-atualizacao` — chave `X-Internal-Api-Key`, uso do microserviço Python, não do front.
