# Spin up the backend environment
dev-up:
	cd infra/compose && docker compose -f docker-compose.dev.backend.yml up -d

# Stop all containers
dev-down:
	cd infra/compose && docker compose -f docker-compose.dev.backend.yml down

# Run frontend locally
dev-web:
	cd apps/web && pnpm dev

# Full dev cycle (backend + frontend)
dev:
	make dev-up && make dev-web