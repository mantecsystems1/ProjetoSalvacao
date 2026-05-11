#!/usr/bin/env bash
set -euo pipefail
cd "$(dirname "$0")/.."

if [[ ! -f .env ]]; then
  cp .env.example .env
  echo "Created .env from .env.example — edit JWT_SIGNING_KEY for production."
fi

docker compose up -d --build

echo "API: http://${API_PORT:-8080}"
echo "Web: http://${WEB_PORT:-3000}"
