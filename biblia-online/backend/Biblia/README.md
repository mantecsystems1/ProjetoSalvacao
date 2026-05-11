# Biblia — Backend .NET 8

Clean Architecture + DDD + **Repository Pattern**, PostgreSQL (EF Core), JWT, Swagger, FluentValidation, tratamento global de erros, Serilog, paginação, cache em memória (`IMemoryCache`) e extensão preparada para Meilisearch (`Search:Provider`, `IVerseSearchIndexer`, stub `MeilisearchSearchAppServiceStub`).

## Projetos

| Projeto | Papel |
|---------|--------|
| `Biblia.Domain` | Entidades e contratos de domínio |
| `Biblia.Application` | DTOs, validações FluentValidation, serviços de aplicação, interfaces de repositório (`Abstractions/Repositories`) |
| `Biblia.Persistence` | `DbContext`, EF configurations, repositórios, migrações, seed, `SqlSearchAppService` |
| `Biblia.Infrastructure` | JWT, BCrypt, cache, stubs Meilisearch |
| `Biblia.API` | Controllers, middleware de exceções, Swagger, Serilog |

## Endpoints

| Método | Rota | Auth |
|--------|------|------|
| GET | `/books?page=&pageSize=` | Não |
| GET | `/books/{id}` | Não |
| GET | `/books/{id}/chapters` | Não |
| GET | `/chapters/{id}/verses?versionId=` | Não |
| GET | `/search?q=&versionId=&page=&pageSize=` | Não |
| POST | `/auth/register` | Não |
| POST | `/auth/login` | Não |
| GET | `/favorites?page=&pageSize=` | JWT |
| POST | `/favorites` body `{ "verseId": "guid" }` | JWT |

## Execução local

1. PostgreSQL rodando (ajuste `ConnectionStrings:DefaultConnection` em `Biblia.API/appsettings.json`).
2. Defina `Jwt:SigningKey` com **pelo menos 32 caracteres**.
3. Na pasta `Biblia.API`:

```bash
dotnet run
```

Migrações e seed rodam no startup (`DbInitializationHostedService`).

## Meilisearch (futuro)

- `appsettings.json`: `"Search": { "Provider": "Sql" }` — busca via PostgreSQL (`ILIKE`).
- Para trocar o modo (quando implementar cliente Meilisearch): `"Provider": "Meilisearch"` — hoje retorna resultado vazio no stub; substitua `MeilisearchSearchAppServiceStub` pela implementação real e registre no DI.
- Use `IVerseSearchIndexer` para jobs de indexação (`NoOpVerseSearchIndexer` por padrão).

## Migrações

```bash
cd Biblia.API
dotnet ef migrations add Nome --project ../Biblia.Persistence/Biblia.Persistence.csproj --output-dir Migrations
dotnet ef database update --project ../Biblia.Persistence/Biblia.Persistence.csproj
```
