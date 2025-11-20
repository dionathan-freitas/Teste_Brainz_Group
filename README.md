# Desafio Técnico - FullStack Developer

Sistema de gerenciamento de estudantes e eventos integrado com Microsoft Graph API.

##  Tecnologias

### Frontend
- React 18
- Vite
- TypeScript
- Tailwind CSS

### Backend
- .NET 8 Web API
- Entity Framework Core
- SQL Server
- JWT Authentication
- Hangfire (sincronização periódica)

## 📋 Funcionalidades

- Autenticação de usuários
- Listagem de estudantes
- Visualização de eventos por estudante
- Sincronização automática com Microsoft Graph

## 🔧 Como executar

### Pré-requisitos
- Docker Desktop instalado e rodando
- .NET 8 SDK

### 1. Banco de Dados (SQL Server via Docker)

Na raiz do projeto:
```powershell
docker-compose up -d
```

Isso iniciará SQL Server 2022 em `localhost:1433` com:
- User: `sa`
- Password: `@Password123`
- Database: será criada automaticamente pela API

Para parar o container:
```powershell
docker-compose down
```

### 2. Backend API

1. Acesse a pasta da API:
```powershell
cd backend/StudentEventsAPI
```
2. Configure user-secrets (obrigatório em dev – valores foram removidos de `appsettings.json`):
```
dotnet user-secrets init
dotnet user-secrets set "Jwt:Key" "<sua-chave-forte>"
dotnet user-secrets set "MicrosoftGraph:ClientId" "<client-id>"
dotnet user-secrets set "MicrosoftGraph:ClientSecret" "<client-secret>"
dotnet user-secrets set "MicrosoftGraph:TenantId" "<tenant-id>"
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost,1433;Database=StudentEventsDB;User Id=sa;Password=<SuaSenhaForte>;TrustServerCertificate=true;MultipleActiveResultSets=true"
```
Se qualquer chave permanecer como `__SECRET__` em runtime a aplicação poderá falhar nas operações que dependem da configuração (JWT ou Graph).
3. Executar a API:
```powershell
dotnet run
```
4. Testar saúde:
```
GET http://localhost:5099/health
```
5. Login inicial (seed admin):
```
POST http://localhost:5099/api/auth/login
Body: {"username":"admin","password":"admin123"}
```
6. Usar token Bearer para acessar estudantes:
```
GET http://localhost:5099/api/students
```

Próximas seções (Front-end, sync Graph, testes) serão adicionadas conforme implementação.

## 🔌 Endpoints iniciais

| Método | Rota | Descrição | Auth |
|--------|------|-----------|------|
| GET | /health | Verifica status da API | Livre |
| POST | /api/auth/login | Autentica e retorna JWT | Livre |
| GET | /api/students | Lista estudantes paginada | Bearer |
| GET | /api/students/{id}/events | Todos os eventos do estudante | Bearer |
| GET | /api/events | Lista global de eventos (paginado) | Bearer |
| POST | /api/sync/students | Força sync de usuários Graph | Admin |
| POST | /api/sync/events | Força sync de eventos Graph | Admin |

### Parâmetros de Paginação & Filtros

Students (`GET /api/students`):
```
page (int >=1)
pageSize (int >=1)
search (opcional) - nome ou email (contains)
department (opcional) - match exato do departamento
```

Events (`GET /api/events`):
```
page, pageSize (obrigatórios)
studentId (opcional) - restringe a um estudante
start (opcional, ISO 8601) - filtra início >= start
end (opcional, ISO 8601)   - filtra fim <= end
search (opcional) - busca em subject ou location
```

Exemplo:
```
GET /api/events?page=1&pageSize=20&start=2025-01-01&end=2025-01-31&search=reunião
```

Student Events (`GET /api/students/{id}/events`): retorna todos os eventos do estudante ordenados por data ascendente (sem paginação ainda, otimização futura possível).

### Sincronização Manual vs Automática
- Automática: Hangfire job `sync-students` executa de hora em hora (`Cron.Hourly`). Pode ser ajustado em `Program.cs`.
- Manual: endpoints `/api/sync/students` e `/api/sync/events` (requer usuário com role Admin). Útil para testes ou após alterar a janela de sincronização.

### Janela de Sincronização de Eventos
Configurável em `appsettings.json` (seção `Sync`) ou via outros providers:
```
"Sync": {
	"MonthsPast": 1,
	"MonthsFuture": 3
}
```
GraphSyncService buscará eventos dentro de `[UtcNow - MonthsPast .. UtcNow + MonthsFuture]`.

### Segurança do Dashboard Hangfire
- Rota: `/jobs`
- Protegido por filtro que exige usuário autenticado com role `Admin`.
Adicionar autorização ao chamar: incluir header `Authorization: Bearer <token-admin>`.

### Migrations & Evolução de Schema
- Primeira migration: `InitialCreate` já gerada.
- Aplicação chama `Database.MigrateAsync()` em startup (DataSeeder).
- Nova alteração de modelo:
```
dotnet ef migrations add AddCampoX
dotnet ef database update
```
Commitar a migration para manter histórico.

## 🔁 Sincronização – Esqueleto Implementado

Status:
- Serviço `GraphSyncService` criado (sincroniza usuários do Microsoft Graph -> Students)
- Propriedade `GraphUserId` adicionada ao modelo `Student` para vínculo permanente
- Job recorrente Hangfire configurado (`sync-students`) executa de hora em hora
- Dashboard Hangfire disponível em `/jobs` (proteção futura via auth a definir)
- Próximo passo: implementar sincronização de eventos (calendário) por usuário

Fluxo atual (usuários):
1. Obtém até 50 usuários do Graph (`displayName`, `mail`, `department`)
2. Faz upsert (inclusão ou atualização) na tabela Students
3. Atualiza `LastSyncDate` para cada registro processado

Planejado para eventos:
- Ler calendários / eventos futuros por usuário
- Persistir em tabela Events vinculada ao Student
- Otimização: evitar reprocessar eventos antigos (janela de tempo configurável)

Frequência (temporária): hourly via `Cron.Hourly` – poderá ser ajustada conforme necessidade.

## 📝 Estrutura do Projeto

```
/frontend - Aplicação React
/backend  - API .NET 8
```
