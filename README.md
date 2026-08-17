# Tribuna Backend

API REST em **ASP.NET Core (.NET 10)** para o portal de notícias **Tribuna**. Serve o conteúdo consumido pelo frontend Angular ([Tribuna-Portal](https://github.com/T0ddi1/Tribuna-Portal)) — artigos, categorias/editorias, comentários e newsletter — com autenticação de equipe editorial e segurança como prioridade.

## Stack

- ASP.NET Core Web API (.NET 10, Controllers)
- Entity Framework Core + **SQLite** — banco é um único arquivo `.db` local, em `App_Data/`, sem necessidade de servidor de banco separado
- ASP.NET Core Identity (usuários/roles)
- JWT (access token) + refresh token rotativo em cookie `HttpOnly`

## Por que não tem cadastro público de usuário

Não existe autocadastro de leitor: a tela `/cadastre-se` do frontend é, na prática, um login. Contas (`Admin`/`Editor`) são criadas por um Admin já autenticado via `POST /api/auth/usuarios`. O primeiro Admin nasce automaticamente no primeiro `dotnet run`, a partir de configuração (nunca de senha fixa no código) — veja [Configuração de segredos](#configuração-de-segredos).

## Modelo de domínio

| Entidade | Descrição |
|---|---|
| `Artigo` | Notícia: slug, título, subtítulo, resumo, corpo (parágrafos), imagem de capa, categoria, autor, flags `Destaque`/`Patrocinado`/`Publicada` |
| `Categoria` | Editoria/vertical (ex.: Capital, Esportes, Tech), com campos de tema (cores) usados nas páginas de vertical do front |
| `Comentario` | Comentário de leitor em um artigo — entra **pendente** e só aparece publicamente após moderação |
| `NewsletterAssinante` | E-mails inscritos na newsletter |
| `ApplicationUser` / `Roles` | Contas de equipe editorial, com role `Admin` ou `Editor` |
| `RefreshToken` | Sessões ativas (hash do token, nunca o valor em texto puro) |

## Endpoints principais

Público (leitura):
- `GET /api/artigos?categoria=&busca=&pagina=&tamanhoPagina=` — listagem paginada
- `GET /api/artigos/destaque` — artigos em destaque (home)
- `GET /api/artigos/{slug}` — detalhe do artigo
- `GET /api/categorias` / `GET /api/categorias/{slug}`
- `GET /api/artigos/{artigoId}/comentarios` — só comentários aprovados
- `POST /api/artigos/{artigoId}/comentarios` — envia comentário (vai para moderação)
- `POST /api/newsletter` — inscrição por e-mail

Autenticação:
- `POST /api/auth/login`
- `POST /api/auth/refresh`
- `POST /api/auth/logout`
- `GET /api/auth/me`

Protegido (`Admin`/`Editor`):
- `POST/PUT/DELETE /api/artigos/{id}` — CRUD de artigos
- `POST/PUT/DELETE /api/categorias/{id}` — CRUD de categorias (`Admin`)
- `GET /api/comentarios/pendentes`, `POST /api/comentarios/{id}/aprovar`, `DELETE /api/comentarios/{id}` — moderação
- `POST /api/auth/usuarios` — criar novo usuário editorial (`Admin`)

## Segurança

- **Sem autocadastro público** de contas privilegiadas.
- **JWT de curta duração** (15 min) + **refresh token opaco rotativo**, armazenado como hash SHA-256 no banco e entregue em cookie `HttpOnly`, `SameSite=Strict` e `Secure` (fora de desenvolvimento). Reuso de um refresh token já trocado revoga toda a sessão (indício de roubo de token).
- **Rate limiting**: login/refresh limitados a 5 req/min por IP; comentários e newsletter a 10 req/min por IP.
- **Lockout de conta** após 5 tentativas de login inválidas (15 min).
- **Política de senha forte** para contas editoriais (mínimo 10 caracteres, maiúscula, minúscula, número e símbolo).
- Mensagens de erro de login genéricas (não revelam se o e-mail existe).
- **Moderação obrigatória de comentários** + honeypot anti-bot + remoção de HTML do texto enviado (mitigação de XSS armazenado).
- **CORS restrito** à origem configurada do frontend.
- Headers de segurança (`X-Frame-Options`, `X-Content-Type-Options`, `Referrer-Policy`, `Permissions-Policy`) e `HSTS` em produção.
- Erros internos nunca vazam stack trace fora de desenvolvimento.
- Segredos (chave JWT, credenciais do Admin inicial) sempre fora do código — via `dotnet user-secrets` em dev, variáveis de ambiente em produção.

## Como rodar localmente

Pré-requisitos: [.NET SDK 10](https://dotnet.microsoft.com/download).

```bash
dotnet restore
dotnet tool install --global dotnet-ef
```

### Configuração de segredos

```bash
dotnet user-secrets init
dotnet user-secrets set "Jwt:Key" "<uma chave aleatória com pelo menos 32 caracteres>"
dotnet user-secrets set "Admin:Email" "admin@seudominio.com"
dotnet user-secrets set "Admin:Senha" "<uma senha forte>"
```

Sem `Admin:Email`/`Admin:Senha`, nenhum Admin inicial é criado e você fica sem acesso às rotas protegidas.

### Subir a API

```bash
dotnet run
```

O `.db` SQLite é criado/migrado automaticamente em `App_Data/` na primeira execução — não é necessário abrir nenhuma ferramenta de SQL. A API sobe em `http://localhost:5299` por padrão (ajustável em `Properties/launchSettings.json` ou `--urls`).

### Migrations

Ao alterar os modelos em `Models/`:

```bash
dotnet ef migrations add NomeDaMigracao
dotnet run
```

## Configuração (`appsettings.json`)

| Chave | Descrição |
|---|---|
| `ConnectionStrings:DefaultConnection` | Caminho do arquivo SQLite |
| `Jwt:Issuer` / `Jwt:Audience` | Identificadores do token |
| `Jwt:AccessTokenMinutos` / `Jwt:RefreshTokenDias` | Duração dos tokens |
| `Cors:AllowedOrigins` | Origens permitidas (URL do frontend) |

`Jwt:Key`, `Admin:Email` e `Admin:Senha` **não** ficam no `appsettings.json` — sempre em `user-secrets` (dev) ou variáveis de ambiente (produção), por exemplo `Jwt__Key`, `Admin__Email`, `Admin__Senha`.

## Estrutura

```
Controllers/   Endpoints da API
Models/        Entidades EF Core
Data/          DbContext
DTOs/          Contratos de entrada/saída
Services/      TokenService, SeedService
Migrations/    Histórico de schema do EF Core
```
