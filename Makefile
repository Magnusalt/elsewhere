up:
	docker compose -f infra/compose/docker-compose.dev.yml up --build

down:
	docker compose -f infra/compose/docker-compose.dev.yml down

logs: ## logs -f: follow logs
	docker compose -f infra/compose/docker-compose.dev.yml logs -f