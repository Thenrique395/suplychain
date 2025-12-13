# Supply Chain Portal – API (.NET 8) + Frontend (Angular)

Aplicação completa para o desafio de gestão de colaboradores. Principais features:

1. **API .NET 8** com camadas Domain/Application/Infrastructure, EF Core 8 + PostgreSQL, validações FluentValidation, JWT Auth e Serilog.
2. **CRUD completo** com regras de negócio (dois telefones obrigatórios, maioridade, hierarquia de cargos, documento único).
3. **Frontend Angular 19** consumindo a API (login, listagem, criação/edição), tema customizado e validações reativas.
4. **Docker Compose** sobe Postgres, API e dev server Angular em containers.
5. **Documentação Swagger** disponível em `/swagger`.
6. **Testes unitários** (xUnit) cobrindo controllers, serviços e validações.

## Pré-requisitos

- .NET 8 SDK
- Node.js 20.x (recomendado) + npm
- Docker Desktop (para execução via compose)

## Executando com Docker

```bash
docker-compose up --build
```

- API: http://localhost:5000  
- Swagger: http://localhost:5000/swagger  
- Frontend: http://localhost:4200  

As migrações do EF são aplicadas automaticamente na subida do container da API.

## Executando localmente (sem Docker)

1. **Banco/Postgres**: suba via Docker (`docker-compose up db`) ou instale localmente e atualize `ConnectionStrings:Default`.  
   - Em desenvolvimento o backend usa `Host=db;Port=5432...` como fallback; em produção use variáveis de ambiente (`ConnectionStrings__Default`).
2. **API**
   ```bash
   cd supplychain
   dotnet restore
   dotnet run
   ```
3. **Frontend**
   ```bash
   cd frontend
   npm install
   npm start
   ```
   - O ambiente padrão (`src/app/core/config/environment.ts`) aponta para `http://localhost:5000`.
   - Para consumir uma API remota (ex.: servidor externo em `http://192.168.1.85:5000`) use:
     ```bash
     ng serve --configuration production
     (Observacao so alterar o ip)
     ```
     Esse build troca automaticamente para `environment.prod.ts`.

## Testes

```bash
dotnet test supplychainTest
```

## Estrutura

- `supplychain/` – API .NET 8  
- `frontend/` – Angular 19  
- `docker-compose.yml` – Orquestra banco/API/frontend  

## Notas

- Criação e edição de funcionários agora aplicam hashing de senha via `AuthService`.
- A API respeita automaticamente a hierarquia de cargos na criação e não permite menores de idade.
- `EmployeesController` valida o mínimo de dois telefones tanto na criação quanto na atualização.
- Quando o banco começa vazio, um usuário administrador padrão é criado automaticamente:

  ```json
  {
    "firstName": "Melissa",
    "lastName": "Costa",
    "email": "melissa.costa@company.com",
    "docNumber": "MELISSA-001",
    "role": "Director",
    "password": "ChangeMe123!"
  }
  ```
  - Substitua via variáveis de ambiente (`Seed:AdminDoc`, `Seed:AdminEmail`, `Seed:AdminPassword`, etc.) quando necessário.
