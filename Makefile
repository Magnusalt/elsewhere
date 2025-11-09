COMPOSE_FILE=infra/compose/docker-compose.dev.backend.yml

# Default: show available commands
help:
	@echo "Usage:"
	@echo "  make build SERVICE=<name>     - Build one service"
	@echo "  make rebuild SERVICE=<name>   - Force rebuild one service (no cache)"
	@echo "  make up SERVICE=<name>        - Start one service"
	@echo "  make logs SERVICE=<name>      - Tail logs for a service"

# Rebuild specific service (with cache)
build:
	docker compose -f $(COMPOSE_FILE) build $(SERVICE)

# Rebuild specific service (ignore cache)
rebuild:
	docker compose -f $(COMPOSE_FILE) build --no-cache $(SERVICE)

# Start a service (detached)
up:
	docker compose -f $(COMPOSE_FILE) up -d $(SERVICE)

# Tail logs
logs:
	docker compose -f $(COMPOSE_FILE) logs -f $(SERVICE)

up-all:
	docker compose -f $(COMPOSE_FILE) up -d

down:
	docker compose -f $(COMPOSE_FILE) down --remove-orphans