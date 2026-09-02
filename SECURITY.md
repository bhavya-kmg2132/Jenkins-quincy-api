# Security Policy

## Supported versions

Only the current release branch receives security fixes.

| Version | Supported |
|---|---|
| .NET 10 (current) | Yes |
| .NET 9 and below | No |

---

## Reporting a vulnerability

**Do not open a public GitHub issue for security vulnerabilities.**

Email **manish.bisht@kmgus.com** with the subject line `[SECURITY] <brief description>`. Include:

- A description of the vulnerability and the affected component (e.g., `netcoreapi`, `ApiGateway`, `netauthlib`)
- Steps to reproduce or a proof-of-concept
- Potential impact (data exposure, authentication bypass, privilege escalation, etc.)
- Your suggested remediation if you have one

You will receive an acknowledgement within **2 business days** and a resolution update within **7 business days**. Critical vulnerabilities (authentication bypass, RCE, tenant data leakage) will be treated as P0 and patched immediately.

---

## Authentication & authorization

### Modes

The system supports two mutually exclusive authentication modes, controlled by `SelfAuthentication` in `appsettings.json`:

| Mode | Mechanism | Configuration section |
|---|---|---|
| `SelfAuthentication: true` | JWT Bearer via `NetAuth.Lib` | `JwtConfig` |
| `SelfAuthentication: false` | Azure AD / OpenID Connect | `AzureAd` |

### JWT configuration (self-hosted mode)

- The signing key (`JwtConfig:Key`) must be at least 32 characters.
- `Issuer` and `Audience` must be set and validated on every request.
- Tokens are validated by `NetAuth.Lib` (`Auth/netauthlib/NetAuth.Lib.dll`).
- **Never store the JWT key in source control.** Use `dotnet user-secrets` locally and environment variables or Azure Key Vault in production.

### Azure AD (federated mode)

- Uses `Microsoft.Identity.Web` with `AllowWebApiToBeAuthorizedByACL: true`.
- `ExposedApiScope` must match the scope configured in the Azure App Registration.
- `ClientSecret` must be rotated on a scheduled basis and never committed.

### Global authorization

When `SelfAuthentication` is enabled, a global `[Authorize]` filter is applied to all controllers in `Startup.cs`. Endpoints that must be publicly accessible require an explicit `[AllowAnonymous]` attribute.

---

## Multi-tenancy & data isolation

- All database queries must filter by `TenantId`, which is sourced exclusively from `ICurrentUserService.TenantId` (populated from the validated JWT claim).
- **Never accept `TenantId` as a client-supplied parameter.** It must always come from the authenticated token.
- Cross-tenant queries require an explicit permission check via `ICurrentUserService.HasPermissionAsync()`.
- `AuditableEntity` stores `TenantId` on every write — if a new entity omits this, it will be invisible or readable across tenants.

---

## Field-level permissions

Field visibility and writability are controlled by `FieldPermissionAttribute` and evaluated by the `Helper` utility at response time. When adding new sensitive fields to an entity or DTO:

1. Decorate the property with the appropriate `FieldPermissionAttribute`.
2. Verify that `Helper.FilterByPermission()` is called before the response is returned.
3. Add test coverage for both the permitted and denied states.

---

## Secrets management

| Environment | Method |
|---|---|
| Local development | `dotnet user-secrets` (UserSecretsId in each Api.csproj) |
| CI / staging | Environment variables injected by the pipeline |
| Production | Azure Key Vault via `Azure.Identity` (DefaultAzureCredential) |

Secrets that must **never** appear in committed files:

- `JwtConfig:Key`
- `AzureAd:ClientSecret`
- `ConnectionStrings:*`
- `KafkaSettings:SaslPassword`
- `NotificationSettings:Password`
- `BrevoApi:ApiKey`, `ZeptoMailSettings:ApiKey`
- `AwsSettings:SecretAccessKey`
- `MicrosoftGraphSettings:ClientSecret`
- `Zoho:ClientSecret`, `Zoho:AccessToken`, `Zoho:RefreshToken`

---

## API Gateway security

The Ocelot gateway (`ApiGateway/Gateway`) is the single public entry point. It enforces:

- JWT / Azure AD token validation before forwarding requests downstream.
- `CheckApiPermission: true` — permission checks are evaluated at the gateway before the request reaches any microservice.
- Cache-control headers via `FileCacheOptions` (TTL: 120 s).
- CORS origin allowlist (`AllowOrigins`) — requests from unlisted origins are rejected.

Downstream microservices should be network-isolated and not directly reachable from the public internet.

---

## Input validation

Every command that enters the application layer has a corresponding FluentValidation `AbstractValidator<TRequest>`. The MediatR validation pipeline behavior runs validators before the handler executes and throws `ValidationException` on failure — which `ApiExceptionFilterAttribute` maps to a `400 Bad Request` with `ProblemDetails`.

**Do not bypass this pipeline.** Never call a handler directly from a controller without going through `Mediator.Send()`.

SQL injection is mitigated by Dapper's parameterized query support. All SQL is defined in named XML files (`Infrastructure/MsSqlServer/` and `Infrastructure/PostgreSql/`). **Never interpolate user input into SQL strings.**

---

## Dependency security

- Packages are pinned to explicit versions in each `.csproj`.
- Run `dotnet list package --vulnerable` periodically and before each release.
- `NetAuth.Lib.dll` is a pre-built binary in `Auth/netauthlib/`. Keep it updated; verify its hash after any update.

---

## Audit logging

When `EventConfiguration:AddEventDataForAuditLog` is enabled, every command writes an audit entry via `IPublishEventDataAccess`. The audit record includes `CreatedBy`, `TenantId`, `CorrelationId`, `RequestId`, `RequestName`, and timestamp. Do not disable this in production environments.

---

## Sensitive data handling

- Passwords are hashed with `BCrypt.Net-Next` (work factor ≥ 12). Never store plaintext passwords.
- Do not log request or response bodies that may contain PII, credentials, or tokens.
- Use `System.Text.Json` for serialization; do not deserialize untrusted JSON into dynamic types.
- Excel and CSV imports (`ExcelDataReader`, `CsvHelper`) must validate file content before processing — do not pass uploaded paths directly to file-system APIs.
- Python interop (`pythonnet`) in `Infrastructure/Python/` executes with the application's process privileges. Validate all data passed to Python scripts.
