# Arquitetura — Biblia Online

## Visão geral

- **Frontend**: Next.js 15 (App Router), TypeScript, TailwindCSS v4, Shadcn/UI (Base UI), TanStack Query, Zustand, Axios e PWA (`@ducanh2912/next-pwa`).
- **Backend**: ASP.NET Core (.NET 8), organização em **Clean Architecture** (Domain → Application → Infrastructure → Api).
- **Dados**: PostgreSQL via EF Core; seed mínimo para desenvolvimento.
- **Busca**: Meilisearch indexando versículos (filtro por `bibleVersionId`).
- **Auth**: JWT Bearer (sem Firebase).

## Domínio

Entidades principais: idioma, versão bíblica, livro, título por versão, versículo, usuário, favorito e histórico de capítulo.

## Fluxos

1. **Leitura**: cliente resolve `versionCode` → carrega livros/capítulo por GUID da versão.
2. **Busca**: Meilisearch retorna hits; o frontend abre o capítulo correspondente.
3. **Conta**: endpoints `/api/v1/me/*` exigem JWT; histórico trabalha por capítulo; favoritos por versículo.

## Escalabilidade

- API **stateless** atrás de load balancer (Traefik ou outro).
- PostgreSQL e Meilisearch como serviços dedicados (volumes persistentes).
- Próximos passos típicos de enterprise: fila para reindexação, observabilidade (OpenTelemetry), rate limiting no gateway e políticas de CORS explícitas por ambiente.

## Segurança

- Troque `JWT_SIGNING_KEY`, `MEILI_MASTER_KEY` e senhas de banco em produção.
- Proteja `POST /api/v1/admin/search/reindex` (header `X-Admin-Key` + variável `ADMIN_REINDEX_KEY`).
