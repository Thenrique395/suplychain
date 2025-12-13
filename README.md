# Supply Chain Portal – API (.NET 8) + Frontend (Angular)

Aplicação completa para o desafio de gestão de colaboradores. Principais features:

1. **API .NET 8** com camadas Domain/Application/Infrastructure, EF Core 8 + PostgreSQL, validações FluentValidation, JWT Auth e Serilog.
2. **CRUD completo** com regras de negócio (dois telefones obrigatórios, maioridade, hierarquia de cargos, documento único).
3. **Frontend Angular 19** consumindo a API (login, listagem, criação/edição), tema customizado e validações reativas.
4. **Docker Compose** sobe Postgres, API e dev server Angular em containers.
5. **Documentação Swagger** disponível em `/swagger`.
6. **Testes unitários** (xUnit) cobrindo controllers, serviços e validações.

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
