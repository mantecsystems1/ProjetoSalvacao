# Deploy — Docker Swarm + Traefik

O arquivo `docker/swarm/stack.yml` é um **exemplo** de stack com:

- Traefik (TLS Let’s Encrypt via challenge TLS)
- PostgreSQL, Meilisearch
- API (.NET) e Web (Next.js) com labels de roteamento

## Pré-requisitos

1. Swarm inicializado: `docker swarm init`
2. Rede overlay criada se necessário (o compose declara `biblia_public` e `biblia_internal`).
3. Imagens publicadas no registry:
   - `API_IMAGE`
   - `WEB_IMAGE`

Build local (exemplo):

```bash
docker build -f docker/backend.Dockerfile -t registry.example.com/biblia-online-api:1.0.0 .
docker build -f docker/frontend.Dockerfile \
  --build-arg NEXT_PUBLIC_API_URL=https://www.seudominio.com \
  --build-arg NEXT_PUBLIC_SITE_URL=https://www.seudominio.com \
  -t registry.example.com/biblia-online-web:1.0.0 .
```

## Variáveis sensíveis

Exporte antes do deploy:

- `DOMAIN`, `ACME_EMAIL`
- `POSTGRES_PASSWORD`, `JWT_SIGNING_KEY`, `MEILI_MASTER_KEY`
- `API_IMAGE`, `WEB_IMAGE`

## Deploy

```bash
export DOMAIN=www.seudominio.com
export ACME_EMAIL=voce@dominio.com
export API_IMAGE=registry.example.com/biblia-online-api:1.0.0
export WEB_IMAGE=registry.example.com/biblia-online-web:1.0.0
# ...

docker stack deploy -c docker/swarm/stack.yml biblia
```

## Notas Traefik v3 + Swarm

A configuração exata do provedor Swarm pode variar por versão do Traefik. Valide na documentação oficial (`providers.swarm`) e ajuste os `command:` do serviço `traefik` conforme seu cluster.

Regra importante no exemplo: o site (`web`) não captura prefixo `/api`; as rotas da API usam `PathPrefix(/api)`.
