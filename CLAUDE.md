# CLAUDE.md — Quincy API

## Project overview

Quincy API is a multi-microservice .NET 10 backend built with Clean Architecture, CQRS, and an event-driven design. The primary service (`netcoreapi`) handles core business logic. Supporting services handle notifications (`netnotificationapi`) and Kafka message brokering (`networker`). An Ocelot-based API Gateway routes external traffic to the appropriate microservice.

All services share the same layered structure: Domain → Application → Infrastructure → Api.

---

## Tech stack

### Core

| Package | Version |
|---|---|
| .NET | 10.0 |
| MediatR | 13.1.0 |
| FluentValidation | 12.1.0 |
| Dapper | 2.1.66 |
| Dapper.Extensions.MSSQL | 5.3.1 |
| AutoMapper | 12.0.1 |
| RulesEngine | 6.0.0 |

### Web & API

| Package | Version |
|---|---|
| Swashbuckle.AspNetCore | 9.0.6 |
| Microsoft.AspNetCore.Mvc.Versioning | 5.1.0 |
| Microsoft.AspNetCore.Authentication.JwtBearer | 9.0.10 |
| Microsoft.Identity.Web | 4.0.1 |
| AspNetCore.HealthChecks.* | 9.0.0 |

### Infrastructure

| Package | Version |
|---|---|
| Microsoft.Data.SqlClient | 6.1.2 |
| Npgsql | 10.0.0 |
| MongoDB.Driver | 3.5.0 |
| MassTransit | 8.5.7 |
| Confluent.Kafka | 2.12.0 |
| Hangfire | 1.8.22 |
| AWSSDK.S3 | 4.0.11 |
| Azure.Storage.Blobs | 12.26.0 |
| dbup-sqlserver / dbup-postgresql | 6.0.16 / 6.1.5 |
| Microsoft.Graph | 5.103.0 |
| MailKit | 4.14.1 |

### Logging

| Package | Version |
|---|---|
| NLog | 6.0.5 |
| NLog.Web.AspNetCore | 6.0.5 |
| NLog.Database | 6.0.3 |
| NLog.Targets.ElasticSearch | 7.7.0 |

### Testing

| Package | Version |
|---|---|
| NUnit | 4.3.2 |
| Moq | 4.20.72 |
| FluentAssertions | 8.1.1 |
| coverlet.collector | 6.0.4 |

---

## Architecture

The solution follows Clean Architecture. Dependency direction is strictly inward: Api → Application → Domain. Infrastructure implements Application interfaces; it never references Domain directly for business rules.

### Directory tree

```
quincy-api/
├── ApiGateway/
│   └── Gateway/                        # Ocelot API Gateway
│       ├── Program.cs
│       ├── Startup.cs
│       └── ocelot.json                 # Route definitions
├── Microservices/
│   ├── netcoreapi/                     # PRIMARY service
│   │   ├── src/
│   │   │   ├── Api/                    # Presentation layer
│   │   │   │   ├── Controllers/        # One controller per aggregate root
│   │   │   │   ├── Filters/            # ApiExceptionFilterAttribute
│   │   │   │   ├── Services/           # CurrentUserService (ICurrentUserService impl)
│   │   │   │   ├── Program.cs
│   │   │   │   └── Startup.cs
│   │   │   ├── Application/            # Use-cases, CQRS, validation
│   │   │   │   ├── {Aggregate}/
│   │   │   │   │   ├── Commands/
│   │   │   │   │   │   └── {Operation}/
│   │   │   │   │   │       ├── {Operation}Request.cs      # IRequest<T>
│   │   │   │   │   │       ├── {Operation}RequestHandler.cs
│   │   │   │   │   │       └── {Operation}Validator.cs
│   │   │   │   │   ├── Queries/
│   │   │   │   │   │   └── {Operation}/
│   │   │   │   │   │       └── {Operation}Query.cs        # Query + Handler combined
│   │   │   │   │   ├── EventHandlers/  # INotificationHandler<DomainEventNotification<>>
│   │   │   │   │   └── Rules/          # Domain-specific rule implementations
│   │   │   │   ├── Common/
│   │   │   │   │   ├── Interfaces/     # IAcmeDataAccess, ICurrentUserService, etc.
│   │   │   │   │   ├── Exceptions/     # ValidationException
│   │   │   │   │   ├── Models/         # NotificationHelper, shared DTOs
│   │   │   │   │   └── Utilities/      # Helper, Utils
│   │   │   │   └── RuleEngine/         # JSON rule definitions per domain
│   │   │   ├── Domain/                 # Entities, events, value objects
│   │   │   │   ├── Common/
│   │   │   │   │   ├── AuditableEntity.cs
│   │   │   │   │   ├── DomainEvent.cs
│   │   │   │   │   └── ValueObject.cs
│   │   │   │   ├── Entities/
│   │   │   │   └── Events/
│   │   │   │       └── {Aggregate}/    # {Aggregate}{Created|Updated|Deleted}Event.cs
│   │   │   └── Infrastructure/
│   │   │       ├── Persistence/
│   │   │       │   └── DataAccess/     # Dapper repository implementations
│   │   │       ├── MsSqlServer/        # XML files with named SQL queries
│   │   │       ├── PostgreSql/         # Equivalent XML files for PostgreSQL
│   │   │       ├── BuildScripts/       # dbup migration SQL files
│   │   │       └── Services/           # Infrastructure service implementations
│   │   └── tests/
│   │       ├── Application.UnitTests/
│   │       └── Application.IntegrationTests/
│   ├── netnotificationapi/             # Email/notification service (same layer structure)
│   └── networker/                      # Kafka producer service
├── Auth/
│   └── netauthlib/                     # Pre-built NetAuth.Lib.dll (custom auth)
├── Messaging.Contract/                 # Shared MassTransit message contracts
└── quincy-api.sln
```

### Layer responsibilities

| Layer | Responsibility |
|---|---|
| **Domain** | Entities, domain events, value objects, `AuditableEntity` base |
| **Application** | Commands, queries, validators, event handlers, data-access interfaces |
| **Infrastructure** | Dapper repositories, dbup migrations, Hangfire, cloud storage, email |
| **Api** | Controllers, `CurrentUserService`, filters, DI wiring, middleware |

---

## Development commands

```powershell
# Build entire solution
dotnet build quincy-api.sln

# Run the primary API (Development)
dotnet run --project Microservices/netcoreapi/src/Api/Api.csproj --environment Development

# Run with a specific environment
dotnet run --project Microservices/netcoreapi/src/Api/Api.csproj --environment Staging

# Run all tests
dotnet test quincy-api.sln

# Run only unit tests
dotnet test Microservices/netcoreapi/tests/Application.UnitTests/

# Run only integration tests
dotnet test Microservices/netcoreapi/tests/Application.IntegrationTests/

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"

# Apply DB migrations (dbup runs automatically on startup; to run manually)
dotnet run --project Microservices/netcoreapi/src/Api/Api.csproj -- --migrate-only

# Run the API Gateway
dotnet run --project ApiGateway/Gateway/Gateway.csproj --environment Development
```

**EF Core is not used for migrations.** Schema versioning is handled by dbup (`dbup-sqlserver` / `dbup-postgresql`). Migration SQL files live in `Infrastructure/BuildScripts/`. They run on app startup automatically.

---

## Code style and conventions

### Naming

| Concept | Convention | Example |
|---|---|---|
| Controller | `{Aggregate}Controller` | `AcmeProductController` |
| Command request | `{Operation}{Aggregate}Request` | `CreateAcmeProductRequest` |
| Command handler | `{Operation}{Aggregate}RequestHandler` | `CreateAcmeProductRequestHandler` |
| Query | `Get{Aggregate}By{Key}Query` | `GetAcmeProductByIdQuery` |
| Validator | `{Operation}{Aggregate}Validator` | `CreateAcmeProductValidator` |
| Domain event | `{Aggregate}{Past-tense}Event` | `AcmeProductCreatedEvent` |
| Event handler | `{Aggregate}{Past-tense}EventHandler` | `AcmeProductCreatedEventHandler` |
| Data access interface | `I{Aggregate}DataAccess` | `IAcmeDataAccess` |
| DTO | `{Aggregate}Dto` | `AcmeProductDto` |

### File and class organization

- One class per file, file name matches class name.
- Commands and queries are co-located with their handlers: `Commands/CreateAcmeProduct/` contains `CreateAcmeProductRequest.cs`, `CreateAcmeProductRequestHandler.cs`, and `CreateAcmeProductValidator.cs`.
- Query handler may be in the same file as the query class (`GetAcmeProductByIdQuery.cs` contains both query and handler).
- Event handlers live in `{Aggregate}/EventHandlers/`.

### Types

- **Records** for immutable DTOs and domain events.
- **Classes** for entities, handlers, services, validators.
- **Structs** only for value objects that are small and stack-allocated.

### Nullable reference types

Nullable reference types are **enabled** (`<Nullable>enable</Nullable>`). Use `?` explicitly where null is a valid value. Do not suppress warnings with `!` unless you are certain.

### Async/await

- All I/O operations are async. Method names end with `Async`.
- Use `ConfigureAwait(false)` in library/application-layer code.
- Never block on async with `.Result` or `.Wait()`.

### Error handling

- **Validation errors**: thrown as `ValidationException` from the MediatR validation pipeline behavior. FluentValidation catches rule failures and raises this before the handler runs.
- **Business logic errors**: return meaningful strings or typed results — do not throw exceptions for expected failure paths.
- **Unhandled exceptions**: caught by `ApiExceptionFilterAttribute` registered globally in `Startup.cs`. Returns a structured `ProblemDetails` response.
- **Never throw** generic `Exception` or `ApplicationException` for business failures.

### Logging

Use `ILogger<T>` (from `Microsoft.Extensions.Logging`). NLog provides the actual sink. Structured logging syntax only:

```csharp
_logger.LogInformation("Creating AcmeProduct {Name} for tenant {TenantId}", request.Name, tenantId);
```

Never use string interpolation in log calls. Log levels:

- `LogDebug` — verbose flow tracing (dev only)
- `LogInformation` — key state changes, successful operations
- `LogWarning` — recoverable anomalies
- `LogError` — failures that require attention; include the exception object

---

## API design conventions

### Routes

```
GET     /api/v1/{aggregate}           # list / paginated query
GET     /api/v1/{aggregate}/{id}      # single resource
POST    /api/v1/{aggregate}           # create
PUT     /api/v1/{aggregate}/{id}      # full update
PATCH   /api/v1/{aggregate}/{id}      # partial update
DELETE  /api/v1/{aggregate}/{id}      # soft delete (sets IsDeleted)
DELETE  /api/v1/{aggregate}/{id}/permanent  # hard delete
```

### Versioning

URL-segment versioning via `Microsoft.AspNetCore.Mvc.Versioning`. Controllers declare both unversioned and versioned routes:

```csharp
[Route("api/[controller]")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
```

### Request/response DTOs

- Incoming requests are MediatR `IRequest<T>` implementations (they are both the DTO and the command/query).
- Outgoing responses use `{Aggregate}Dto` mapped by AutoMapper.
- Never return domain entities directly from controllers.

### Validation

FluentValidation via the MediatR pipeline behavior. Every command has a matching `AbstractValidator<TRequest>`. Error messages come from the `Resources.ErrorMessages` resource file — no inline string literals in validators.

### Pagination and filtering

Pass page number and page size as query string parameters. Queries return a paginated envelope. Keep response payloads under ~100 KB.

---

## Authentication & authorization

### JWT Bearer (self-hosted mode)

Enabled when `SelfAuthentication: true` in `appsettings.json`. The custom `NetAuth.Lib` (`Auth/netauthlib/NetAuth.Lib.dll`) handles token issuance and validation. Configure JWT options:

```json
"JwtConfig": {
  "Key": "<32-char-secret>",
  "Issuer": "netapi",
  "Audience": "netapi"
}
```

### Azure AD (federated mode)

Enabled when `SelfAuthentication: false`. Configured via the `AzureAd` section:

```json
"AzureAd": {
  "TenantId": "...",
  "ClientId": "...",
  "ExposedApiScope": "api://..."
}
```

### Protecting endpoints

A global `[Authorize]` filter is applied in `Startup.cs` when `SelfAuthentication` is enabled. To exempt an endpoint: `[AllowAnonymous]`.

### Current user

Inject `ICurrentUserService` to access:

```csharp
string userId     = _currentUserService.UserId;
string userName   = _currentUserService.UserName;
string email      = _currentUserService.Email;
string tenantId   = _currentUserService.TenantId;
string correlationId = _currentUserService.CorrelationId;
bool hasPermission = await _currentUserService.HasPermissionAsync(permission);
```

### Field-level permissions

`FieldPermissionAttribute` and `FieldPermission` domain types control which fields are visible or writable per role/claim. The `Helper` utility in `Application/Common/Utilities/` filters entity properties at response time.

---

## Testing conventions

### Unit vs integration

- **Unit tests** (`Application.UnitTests`): test handlers, validators, domain logic in isolation. Mock all external dependencies with Moq.
- **Integration tests** (`Application.IntegrationTests`): test full request pipeline including the database. Use a real (test) database; do not mock Dapper repositories.

### Mocking

Use **Moq 4.x**. Set up mocks in `[SetUp]`, verify in `[TearDown]` only when call count matters:

```csharp
var dataAccess = new Mock<IAcmeDataAccess>();
dataAccess.Setup(x => x.GetAcmeProductById(id)).ReturnsAsync(entity);
```

### Test structure

Follow Arrange / Act / Assert with blank lines separating phases. Name tests with the pattern:

```
{MethodUnderTest}_Should{ExpectedBehavior}_When{Condition}
```

Example: `Handle_ShouldReturnProductId_WhenRequestIsValid`

### Assertions

Use **FluentAssertions** — not NUnit's built-in `Assert.*`:

```csharp
result.Should().NotBeNull();
result.Should().Be("expected-id");
```

### Test fixtures

Use `[TestFixture]` at the class level. Shared state goes in `[OneTimeSetUp]`; per-test state in `[SetUp]`. Integration tests may use `WebApplicationFactory<Program>` for full-stack HTTP tests.

---

## Configuration & secrets

### appsettings structure

```
appsettings.json               # Defaults — committed to source control
appsettings.Development.json   # Local dev overrides
appsettings.Staging.json       # Staging
appsettings.Production.json    # Production (secrets injected via env vars / Key Vault)
```

### Key configuration sections

| Section | Purpose |
|---|---|
| `ConnectionStrings` | `SqlDBConnection`, `PostgreSqlDBConnection`, `MongoDBConnection` |
| `JwtConfig` | JWT key, issuer, audience |
| `AzureAd` | Tenant/client IDs for Azure AD auth |
| `SelfAuthentication` | `true` = use JWT, `false` = use Azure AD |
| `SqlDatabaseServer` | `"MsSqlServer"` or `"PostgreSql"` |
| `EventConfiguration` | Toggles event store, audit log, event publishing |
| `MassTransitTransport` | Transport type: `PostGreSQL`, `MSSQL`, `Kafka`, etc. |
| `HangfireConfiguration` | Background job transport |
| `KafkaSettings` | Bootstrap servers, topics, SASL credentials |
| `CachingSettings` | Toggle in-memory, Redis, SQL Server, PostgreSQL caches |
| `NotificationSettings` | SMTP, email provider credentials |

### Secrets management

- **Local dev**: `dotnet user-secrets` — the `<UserSecretsId>` is set in each Api.csproj.
- **Production**: Environment variables or Azure Key Vault (via `Azure.Identity`).
- **Never commit** connection strings, JWT keys, or API keys to source control.

---

## Git and branching

### Branch naming

```
feature/{ticket-id}-short-description
bugfix/{ticket-id}-short-description
hotfix/{ticket-id}-short-description
release/{version}
```

### Commit message format

```
<type>: <short summary> (#<ticket-id>)

Types: feat | fix | refactor | test | docs | chore
```

Examples:
```
feat: add field-level permission check to AcmeProduct (#42)
fix: handle null custom fields in AuditableEntity serialization (#55)
```

### PR guidelines

- Keep PRs focused: one feature or fix per PR.
- All tests must pass before merge.
- Squash commits on merge to keep `main` history clean.
- Target `develop`; `main` is production.

---

## Important notes for Claude

### Always follow these patterns

1. **Every command goes through MediatR.** Controllers only call `Mediator.Send(request)` — no business logic in controllers.
2. **Every command has a FluentValidation validator** registered in the same folder. Validation runs automatically via the pipeline behavior before the handler executes.
3. **Data access only through the interface** (`IAcmeDataAccess`, etc.). Handlers never instantiate repositories or call `Dapper` directly.
4. **SQL queries live in XML files**, not in C# code. `MsSqlServer/{Aggregate}.xml` and `PostgreSql/{Aggregate}.xml` contain named queries loaded at startup.
5. **All entities extend `AuditableEntity`.** Set `CreatedBy`, `CorrelationId`, `RequestId`, and `TenantId` from `ICurrentUserService` before persisting.
6. **Raise domain events on entities** (`entity.DomainEvents.Add(new AcmeProductCreatedEvent(entity))`). The pipeline behavior dispatches them after the handler completes.
7. **AutoMapper for entity → DTO mapping.** Define mapping profiles; never manually map properties in handlers.
8. **Return the entity ID (string) from create commands**, `Unit` from update/delete commands.
9. **Use `ICurrentUserService` for all user context** — never read claims directly from `HttpContext` inside the application layer.
10. **Database type is configurable** (`SqlDatabaseServer` in appsettings). Infrastructure repositories must support both MSSQL and PostgreSQL via the XML query selection pattern.

### Never do these things

- Do not throw exceptions for business validation failures — validate via FluentValidation before the handler and return results.
- Do not put SQL strings inside C# handler or repository code — use XML query files.
- Do not call infrastructure services directly from the Domain layer.
- Do not use `Newtonsoft.Json` for new code — use `System.Text.Json`.
- Do not use `Task.Result` or `Task.Wait()`.
- Do not add properties to domain entities without updating the corresponding XML query file and AutoMapper profile.
- Do not skip the `AuditableEntity` base for new entities — all persisted entities must have audit fields.

### MediatR pipeline behaviors (registered order)

1. **LoggingBehavior** — logs request entry/exit with elapsed time
2. **ValidationBehavior** — runs FluentValidation; throws `ValidationException` on failure
3. **UnhandledExceptionBehavior** — catches and logs unexpected exceptions
4. **DomainEventDispatchBehavior** (post-handler) — dispatches domain events raised during handler execution

### Dapper + XML query pattern

Queries are loaded from XML files by entity and database type, then referenced by name:

```csharp
// Infrastructure/MsSqlServer/AcmeProduct.xml
// <query name="GetById">SELECT * FROM AcmeProducts WHERE Id = @Id AND IsDeleted = 0</query>

// DataAccess
var result = await _db.QueryFirstOrDefaultAsync<AcmeProduct>(
    _queries["AcmeProduct.GetById"], new { Id = id });
```

When adding a new query, add it to **both** `MsSqlServer/{Aggregate}.xml` and `PostgreSql/{Aggregate}.xml` with syntax appropriate for each database.

### Multi-tenancy

All database queries must filter by `TenantId`. `AuditableEntity` carries `TenantId` from `ICurrentUserService`. Never query across tenants unless the caller has an explicit cross-tenant permission.

### Event-driven notifications

After a domain event is dispatched, the corresponding `{Aggregate}{Event}EventHandler` in `Application/EventHandlers/`:
1. Persists the event to the EventStore (`IPublishEventDataAccess`) if `AddEventInEventStore_EventDB` is enabled.
2. Publishes a `MassTransitEvent` via `IMassTransitPublisher` which routes to `netnotificationapi` for email/notification delivery.
3. Optionally produces a Kafka message if `UseKafka` is enabled.

### External integrations

| Integration | Toggle | Configuration Section |
|---|---|---|
| Kafka | `KafkaSettings:UseKafka` | `KafkaSettings` |
| Azure Blob Storage | Injected via infra DI | `AzureStorageSettings` |
| AWS S3 | Injected via infra DI | `AwsSettings` |
| Microsoft Graph (email) | Provider selection | `MicrosoftGraphSettings` |
| Brevo (Sendinblue) | Provider selection | `BrevoApi` |
| ZeptoMail | Provider selection | `ZeptoMailSettings` |
| Hangfire | Always on | `HangfireConfiguration` |
| Python interop | `pythonnet` | Infrastructure/Python/ |

### Rule engine

Business rules are defined as JSON in `Application/RuleEngine/{Domain}/`. The `IRuleEngine` interface wraps `RulesEngine` and is called from command handlers before persistence. Add new rules by creating or updating the JSON file — do not hardcode conditional logic in handlers.
