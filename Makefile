include .env
export

.PHONY: help setup dev restore install-tools run watch-run test ef-migrations ef-update-database ef-remove-migrations docker-db-up docker-db-down secrets-help

STARTUP_PROJECT=src/TaskFlow.Api/TaskFlow.Api.csproj
MIGRATIONS_PROJECT=src/TaskFlow.Infrastructure/TaskFlow.Infrastructure.csproj
MIGRATIONS_OUTPUT_DIR=Persistence/Migrations


help: ## Mostrar uma lista de comandos disponíveis
	@echo "Comandos disponíveis (Rodar todos na raiz do projeto):"	
	@grep -h -E '^[a-zA-Z_-]+:.*?## .*$$' $(MAKEFILE_LIST) | \
	awk 'BEGIN {FS = ":.*?## "}; {printf "\033[36m%-20s\033[0m %s\n", $$1, $$2}'


setup: docker-db-up restore install-tools ef-update-database ## Setup completo do projeto
	@echo "Ambiente pronto! Configure os secrets com: make secrets-help"


dev: ## Rodar aplicação em modo de desenvolvimento
	@echo "Rodando aplicação em modo de desenvolvimento..."
	@echo ""
	@echo "Subindo container do banco de dados..."
	@make docker-db-up
	@echo ""
	@echo "Rodando aplicação em watch mode..."
	@make watch-run


restore: ## Instalar pacotes NuGet
	@echo "Instalando pacotes NuGet..."
	@dotnet restore


install-tools: ## Instalar ferramentas .NET global tools
	@echo "Instalando .NET tools..."
	@dotnet tool restore


run: ## Rodar a aplicação
	@echo "Rodando aplicação..."
	@dotnet run --project $(STARTUP_PROJECT)


watch-run: ## Rodar a aplicação em watch mode
	@echo "Rodando aplicação em watch mode..."
	@dotnet watch --project $(STARTUP_PROJECT) run


test: ## Rodar testes unitários
	@echo "Rodando testes..."
	@dotnet test


ef-migrations: ## Criar nova migração. (Exemplo: make ef-migrations name=MigrationName)
	@if [ -z "$(name)" ]; then \
		echo "Migration name is required. Usage: make ef-migrations name=MigrationName"; \
		exit 1; \
	fi
	@echo "Criando nova migração: $(name)..."
	@dotnet ef migrations add $(name) \
		--project $(MIGRATIONS_PROJECT) \
		--startup-project $(STARTUP_PROJECT) \
		--output-dir $(MIGRATIONS_OUTPUT_DIR) \
		--context AppDbContext


ef-update-database: ## Atualizar banco de dados com a última migração
	@echo "Atualizando banco de dados com a última migração..."
	@dotnet ef database update \
		--project $(MIGRATIONS_PROJECT) \
		--startup-project $(STARTUP_PROJECT) \
		--context AppDbContext


ef-remove-migrations: ## Remover a última migração
	@echo "Removendo última migração..."
	@dotnet ef migrations remove \
		--project $(MIGRATIONS_PROJECT) \
		--startup-project $(STARTUP_PROJECT) \
		--context AppDbContext

docker-db-up: ## Subir container do banco de dados usando Docker
	@echo "Subindo container do banco de dados..."
	@docker compose -p taskflow-db up -d


docker-db-down: ## Parar e remover container do banco de dados usando Docker
	@echo "Parando e removendo container do banco de dados..."
	@docker compose -p taskflow-db down


secrets-help: ## Instruções para configurar secrets de desenvolvimento
	@echo ""
	@echo "Configure os secrets com os comandos abaixo:"
	@echo ""
	@echo "  dotnet user-secrets init --project $(STARTUP_PROJECT)"
	@echo ""
	@echo "  dotnet user-secrets set \"ConnectionStrings:Default\" \"Host=localhost;Port=$(POSTGRES_PORT);Database=$(POSTGRES_DB);Username=$(POSTGRES_USER);Password=$(POSTGRES_PASSWORD)\" --project $(STARTUP_PROJECT)"
	@echo ""
	@echo "  dotnet user-secrets set \"Jwt:Key\" \"sua-chave-jwt\" --project $(STARTUP_PROJECT)"
	@echo ""