# syntax=docker/dockerfile:1

FROM node:20-alpine AS deps
WORKDIR /app

COPY frontend/package*.json ./

RUN npm ci

FROM node:20-alpine AS build
WORKDIR /app

COPY --from=deps /app/node_modules ./node_modules
COPY frontend .

ARG NEXT_PUBLIC_API_URL=https://api-biblia.portalmantec.com.br
ARG NEXT_PUBLIC_SITE_URL=https://biblia.portalmantec.com.br

ENV NEXT_PUBLIC_API_URL=$NEXT_PUBLIC_API_URL
ENV NEXT_PUBLIC_SITE_URL=$NEXT_PUBLIC_SITE_URL

RUN npm run build

FROM node:20-alpine AS runtime
WORKDIR /app

ENV NODE_ENV=production
ENV NEXT_TELEMETRY_DISABLED=1
ENV PORT=3000

COPY --from=build /app/public ./public
COPY --from=build /app/.next/standalone ./
COPY --from=build /app/.next/static ./.next/static

EXPOSE 3000

CMD ["node", "server.js"]