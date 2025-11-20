# Desafio Técnico - FullStack Developer

Sistema de gerenciamento de estudantes e eventos integrado com Microsoft Graph API.

## 🚀 Tecnologias

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
2. (Opcional) Configure user-secrets para não expor credenciais em `appsettings.json`:
```
dotnet user-secrets init
dotnet user-secrets set "Jwt:Key" "<sua-chave-forte>"
dotnet user-secrets set "MicrosoftGraph:ClientId" "2936bb04-ca85-47ae-b117-0330aac01d5d"
dotnet user-secrets set "MicrosoftGraph:ClientSecret" "Ik68Q~yz03c7LZQIWy3IvlF1Pl8OPePzCmklRb43"
dotnet user-secrets set "MicrosoftGraph:TenantId" "302de125-622a-4ac3-a029-4431603ffed3"
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost,1433;Database=StudentEventsDB;User Id=sa;Password=@Password123;TrustServerCertificate=true;MultipleActiveResultSets=true"
```
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
| GET | /api/students | Lista estudantes | Bearer |
| GET | /api/students/{id}/events | Eventos de um estudante | Bearer |

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
