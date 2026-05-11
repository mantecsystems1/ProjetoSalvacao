# Banco de dados (PostgreSQL)

O schema é gerenciado pelo **Entity Framework Core** (`backend/src/BibliaOnline.Infrastructure/Persistence/Migrations`).

Na primeira execução da API (ou container `api`), o hosted service `DbBootstrapHostedService`:

1. executa `Migrate()`;
2. se não houver idiomas, aplica o seed mínimo (`SampleBibleSeed`);
3. garante índice Meilisearch e, se configurado, reindexa versículos.

## Migrações locais

```bash
cd backend/src/BibliaOnline.Api
dotnet ef migrations add NomeDaMigracao --project ../BibliaOnline.Infrastructure/BibliaOnline.Infrastructure.csproj --output-dir Persistence/Migrations
dotnet ef database update --project ../BibliaOnline.Infrastructure/BibliaOnline.Infrastructure.csproj
```

## Produção

Prefira backups automatizados do volume PostgreSQL e uma política de rotação de credenciais (`POSTGRES_PASSWORD`, `JWT_SIGNING_KEY`, `MEILI_MASTER_KEY`).
