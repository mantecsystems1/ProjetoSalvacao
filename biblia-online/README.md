# Biblia Online

Plataforma de leitura bíblica com **Next.js 15**, **.NET 8 (Clean Architecture)**, **PostgreSQL**, **Meilisearch**, **Docker** e exemplo de **Docker Swarm + Traefik**.

## Requisitos

- Docker / Docker Compose  
- (Opcional) .NET 8 SDK e Node 20 para desenvolvimento local sem containers

## Início rápido (Compose)

1. Copie variáveis:

```powershell
cd biblia-online
copy .env.example .env
```

Edite `.env` e defina **`JWT_SIGNING_KEY`** (mínimo 32 caracteres).

2. Suba os serviços:

```powershell
.\scripts\dev-up.ps1
```

Ou:

```bash
chmod +x ./scripts/dev-up.sh
./scripts/dev-up.sh
```

3. Acesse:

- Frontend: `http://localhost:3000`
- API Swagger: `http://localhost:8080/swagger`

Na primeira subida, a API aplica migrações EF, seed mínimo e sincroniza o Meilisearch (se `Meilisearch__SyncOnStartup=true`).

## Estrutura

```
biblia-online/
├── frontend/          # Next.js 15 + Tailwind + Shadcn + PWA
├── backend/           # .NET 8 (Domain, Application, Infrastructure, Api)
├── database/          # Notas sobre migrações
├── imports/           # Esquemas/payloads para importação PD
├── docker/            # Dockerfiles, Traefik de referência, stack Swarm
├── docs/              # Arquitetura e deploy
├── scripts/           # Scripts de setup
└── docker-compose.yml
```

## Variáveis principais

Veja `.env.example`. Destaques:

- `PUBLIC_API_URL` / `PUBLIC_SITE_URL`: URLs usadas pelo browser (JSON e SEO).
- `JWT_SIGNING_KEY`: segredo HMAC do JWT.
- `MEILI_MASTER_KEY`: chave do Meilisearch (Compose dev usa valor padrão — troque em produção).

## Licença de conteúdo

O seed inclui apenas **trechos curtos** para desenvolvimento. Textos completos devem ser **importados por você** a partir de fontes em **domínio público** ou licença compatível (`imports/README.md`).

## Documentação

- `docs/arquitetura.md`
- `docs/deploy-swarm.md`

## Stack tecnológica (conforme especificação)

- Frontend: Next.js 15, TypeScript, TailwindCSS, Shadcn/UI, Zustand, TanStack Query, Axios, `@ducanh2912/next-pwa`
- Backend: .NET 8, EF Core, JWT, Swagger
- Busca: Meilisearch
- Infra: Docker, exemplo Swarm + Traefik
