bootstrap: ## install toolchains
\tpnpm i --prefix apps/web
\tdotnet restore apps/gateway apps/studio-api apps/recommender-api
\tpython -m venv .venv && . .venv/bin/activate && pip install -r apps/embeddings-svc/requirements.txt

up: ## run full stack
\tdocker compose -f infra/compose/docker-compose.dev.yml up --build

seed:
\tdotnet run --project tools/Seeder/Seeder.csproj data/seeds/seed.json

fmt:
\tdotnet format
\tpnpm -C apps/web format
\trufflehog --no-update # optional secrets scan