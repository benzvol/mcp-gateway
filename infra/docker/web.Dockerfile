# syntax=docker/dockerfile:1
FROM node:24-alpine AS build
RUN corepack enable
WORKDIR /app

COPY pnpm-workspace.yaml pnpm-lock.yaml package.json ./
COPY src/web/package.json src/web/
RUN pnpm install --frozen-lockfile

COPY src/web/ src/web/
RUN pnpm --filter @mcp-gateway/web build

FROM caddy:2-alpine AS final
COPY infra/docker/Caddyfile /etc/caddy/Caddyfile
COPY --from=build /app/src/web/dist /srv
EXPOSE 80
