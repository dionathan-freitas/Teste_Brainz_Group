<div align="center">
	<h1>Student Events – Desafio FullStack Brainz Group</h1>
	<p>Solução completa: .NET 8 Web API + React (Vite + TS) para gerenciamento de estudantes e eventos sincronizados via Microsoft Graph.</p>
	<img alt="Stack" src="https://img.shields.io/badge/.NET%208-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" />
	<img alt="React" src="https://img.shields.io/badge/React-149ECA?style=for-the-badge&logo=react&logoColor=white" />
	<img alt="Tailwind" src="https://img.shields.io/badge/TailwindCSS-38BDF8?style=for-the-badge&logo=tailwindcss&logoColor=white" />
	<img alt="SQL Server" src="https://img.shields.io/badge/SQL%20Server-CC2927?style=for-the-badge&logo=microsoftsqlserver&logoColor=white" />
</div>

---

## Visão Geral
| Camada | Tecnologias | Destaques |
|--------|-------------|-----------|
| Backend | .NET 8, EF Core, SQL Server, JWT, Hangfire, Swagger | Sincronização agendada, seed de dados, filtros e paginação |
| Frontend | React + Vite + TypeScript, Tailwind + DaisyUI | Design responsivo, loading skeletons, acessibilidade básica |
| Infra | Docker (SQL Server), PowerShell scripts | Inicialização rápida, automação de seed e sync |
| Testes | xUnit (serviços de domínio) | 18 testes cobrindo filtros, paginação e agregações |

---
## Estrutura
```
backend/StudentEventsAPI            # Web API (.NET 8)
backend/StudentEventsAPI.Tests      # Testes xUnit
frontend/                           # Aplicação React/Vite
scripts/                            # Automação (PowerShell)
docker-compose.yml                  # SQL Server
```

---
##  Credenciais de Desenvolvimento
| Usuário | Senha | Perfil |
|---------|-------|--------|
| admin   | admin123 | Admin |

---
## Setup Rápido
```powershell
# 1. Subir SQL Server
docker compose up -d

# 2. Configurar segredos (no diretório da API)
cd backend/StudentEventsAPI
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost,1433;Database=StudentEventsDB;User Id=sa;Password=YourStrong@Password123;TrustServerCertificate=true;MultipleActiveResultSets=true"
dotnet user-secrets set "Jwt:Key" "<chave-super-secreta-long>"
# (Opcional para Graph)
dotnet user-secrets set "MicrosoftGraph:ClientId" "<client-id>"
dotnet user-secrets set "MicrosoftGraph:ClientSecret" "<client-secret>"
dotnet user-secrets set "MicrosoftGraph:TenantId" "<tenant-id>"

# 3. Inicializar backend com build, migrations, testes e seed
cd ../../scripts
./init_backend.ps1

# 4. Rodar frontend
./run_frontend.ps1
```
API: http://localhost:5035  |  Frontend: http://localhost:5173  |  Swagger: http://localhost:5035/swagger  | Jobs Hangfire: http://localhost:5035/jobs

---
## Scripts PowerShell
| Script | Função | Flags |
|--------|--------|-------|
| `util_common.ps1` | Funções compartilhadas (login, chamada API, health wait) | — |
| `init_backend.ps1` | Restore, build, migrations, testes, seed e run | `-SkipTests`, `-SkipSeed`, `-Configuration` |
| `run_frontend.ps1` | Instala dependências e inicia Vite | `-SkipInstall` |
| `dev_full.ps1` | Sobe backend e frontend em jobs paralelos | `-SkipInstall`, `-SkipSeed` |
| `seed_sample.ps1` | Chama endpoint de seed desenvolvimento | — |
| `fetch_students_post_seed.ps1` | Recupera lista de estudantes | — |
| `fetch_first_student_events.ps1` | Mostra eventos do primeiro estudante | — |
| `sync_and_check.ps1` | Health + login + força sync (students/events) | — |
| `login_and_get_debug.ps1` | Depuração login + requisição students | — |
| `post_login_debug.ps1` | Depuração bruta da resposta de login | — |

Defina `STUDENT_EVENTS_API` para alterar o BaseUrl se necessário.

---
## Endpoints Principais
| Método | Rota | Descrição | Auth |
|--------|------|-----------|------|
| GET | /health | Verifica status geral | Público |
| POST | /api/auth/login | Autentica e retorna JWT | Público |
| GET | /api/students | Lista estudantes (paginação + filtros) | Bearer |
| GET | /api/students/{id}/events | Eventos de um estudante | Bearer |
| GET | /api/events | Lista global de eventos | Bearer |
| POST | /api/sync/students | Força sincronização de usuários Graph | Admin |
| POST | /api/sync/events | Força sincronização de eventos Graph | Admin |
| GET | /jobs | Dashboard Hangfire | Admin |

### Paginação & Filtros
`/api/students`:
```
page, pageSize (>=1)
search (contains em nome/email)
department (contains)
```

`/api/events`:
```
page, pageSize
studentId (exato)
startDate, endDate (DateTime ISO)
search (subject/location)
```

### Retornos
Todos os listagens usam `PaginatedResult<T>`:
```json
{
	"data": [...],
	"page": 1,
	"pageSize": 10,
	"totalCount": 42,
	"totalPages": 5
}
```

---
## Sincronização (Hangfire)
Job recorrente: `sync-students` (hourly). Configuração em `Program.cs`.
Janela de eventos (planejada): seção `Sync` em `appsettings`:
```json
"Sync": { "MonthsPast": 1, "MonthsFuture": 3 }
```

Dashboard protegido em `/jobs` (requere JWT com role Admin).

---
## Testes Backend
```powershell
dotnet test
```
Cobertura (18 testes):
- Paginação e filtros de estudantes (search/department)
- Paginação e filtros de eventos (date range, studentId, search)
- Agregação de eventos por estudante
- Ordenações (StartDate asc)

Para nova migration:
```powershell
dotnet ef migrations add NomeAlteracao
dotnet ef database update
dotnet test
```

---
## UX & Acessibilidade
Melhorias aplicadas:
- Debounce (400ms) em filtros de busca e departamento (lista de estudantes)
- Skeleton loaders para listas (desktop e mobile)
- Skip link ("Ir para conteúdo") para navegação assistiva
- ARIA atributos em botões e listagens
- Layout responsivo com cards no mobile e tabela em desktop

Próximos incrementos sugeridos:
- Foco visível aprimorado em componentes interativos
- Toasts para erros (substituir alert inline)
- Paginação acessível com `aria-current`

---
## Segurança
- JWT Bearer com validação de emissor/audiência e chave simétrica.
- Swagger configurado com esquema Bearer.
- Dashboard Hangfire exige autenticação + role Admin.

---
## Arquitetura Backend (Resumo)
| Camada | Pastas |
|--------|--------|
| Models | `Models` (Student, Event) |
| DTOs | `DTOs` (EventDto, StudentDto, PaginatedResult) |
| Services | `Services/Students`, `Services/Events`, `Services/GraphSync` |
| Infra | `Data` (DbContext), `Infrastructure` (Auth filtro Hangfire) |
| Options | `Options/SyncOptions` para janela de sincronização |

Mapping via extension methods (ex.: `ToDto()`), isolando projeção de entidades.

---
## Frontend
| Página | Descrição |
|--------|-----------|
| `/login` | Autenticação e armazenamento do token |
| `/students` | Lista paginada + filtros com debounce |
| `/students/:id/events` | Eventos do estudante selecionado |

Proteção de rotas via componente `ProtectedRoute`.

---
## Microsoft Graph (Planejado/Esqueleto)
Serviço `GraphSyncService` preparado para:
1. Buscar usuários (displayName, mail, department)
2. Fazer upsert em Students
3. Registrar data de sincronização (`LastSyncDate`)

Extensão para eventos (calendário) pode seguir padrão similar usando janela de datas configurável.

---
## Licença
Uso interno para avaliação técnica. Não inclui credenciais reais.

---
## Roadmap Futuro (Sugerido)
- Testes frontend (Vitest + Testing Library)
- CI/CD (GitHub Actions: build + test + lint)
- Cache em listagens (Redis / MemoryCache) para alta escala
- Edição de estudantes / eventos (CRUD completo)
- Webhook Graph para mudanças em tempo real

---
## Troubleshooting Rápido
| Problema | Causa Comum | Solução |
|----------|-------------|---------|
| 401 em endpoints | Token ausente ou expirado | Refazer login / verificar `Jwt:Key` |
| Conexão negada SQL | Docker não iniciado | `docker compose up -d` |
| Seed falha | Endpoint dev não habilitado ou token inválido | Conferir role Admin do usuário |
| Swagger sem botão Authorize | Pacotes incompatíveis | Verificar versão Swashbuckle (6.5.0) |

---
