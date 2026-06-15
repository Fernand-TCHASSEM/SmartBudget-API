# 💰 SmartBudget

> A personal finance management web application — import bank statements, automatically categorize transactions, and visualize your finances through an interactive dashboard.

![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?style=flat-square&logo=dotnet)
![Angular](https://img.shields.io/badge/Angular-17+-DD0031?style=flat-square&logo=angular)
![MySQL](https://img.shields.io/badge/MySQL-8.4-4479A1?style=flat-square&logo=mysql&logoColor=white)
![Docker](https://img.shields.io/badge/Docker-Compose-2496ED?style=flat-square&logo=docker&logoColor=white)
![License](https://img.shields.io/badge/License-MIT-green?style=flat-square)

---

## Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Tech Stack](#tech-stack)
- [Architecture](#architecture)
- [AI Integration](#ai-integration)
- [Prerequisites](#prerequisites)
- [Installation & Setup](#installation--setup)
- [Configuration](#configuration)
- [Project Structure](#project-structure)
- [JWT Authentication — Implementation Guide](#jwt-authentication--implementation-guide)
- [Automatic Timestamp Tracking](#automatic-timestamp-tracking)
- [Resource-based Authorization](#resource-based-authorization)
- [Rate Limiting — Redis Sliding Window](#rate-limiting--redis-sliding-window)
- [API Documentation](#api-documentation)
- [Testing](#testing)
- [Roadmap](#roadmap)
- [Author](#author)

---

## Overview

SmartBudget lets you import bank statements exported by Canadian banks (TD, RBC, Scotiabank, BMO...) in **CSV or PDF** format, automatically categorize each transaction using a configurable rule engine, and visualize your finances through a clean, interactive dashboard.

This project is built as a technical portfolio piece demonstrating proficiency in **ASP.NET Core 10**, **Angular 17+**, **Entity Framework Core**, **MySQL**, and deployment on **Azure**.

---

## Features

### Bank Statement Import
- Upload **CSV** files (auto-detected separator, column mapping)
- Upload native **PDF** files (text extraction via PdfPig)
- Data preview before confirmation
- Automatic deduplication via SHA-256 hash
- Import history with detailed statistics

### Automatic Categorization
- Rule engine using keywords or regular expressions
- 12 pre-configured system categories (groceries, transport, entertainment...)
- User-defined custom rules
- Manual correction with automatic rule learning
- Cleaning of raw bank statement labels

### Dashboard
- Monthly summary: income, expenses, net balance
- Pie chart by category
- 6-month trend chart (bar graph)
- Combinable filters: period, category, amount, type
- Full-text search across transaction labels

### Budgets & Alerts
- Configurable monthly budget per category
- Progress bar with alerts at 75% and 100%
- In-app notifications on budget overrun

### Exports
- CSV export of filtered transactions
- PDF export of monthly summary report

---

## Tech Stack

| Layer | Technology |
|---|---|
| Backend API | ASP.NET Core 10 (Web API) |
| ORM | Entity Framework Core 9 + Pomelo MySQL |
| Authentication | Custom JWT — BCrypt.Net + JwtBearer (access + refresh token) |
| Validation | FluentValidation 12 + AutoValidation |
| API Documentation | Scalar (OpenAPI) |
| Logging | Serilog (console + file sink) |
| Frontend | Angular 17+ (standalone components) |
| UI Components | Angular Material |
| Charts | ng2-charts / Chart.js |
| Database | MySQL 8.4 |
| Pagination | System.Linq.Dynamic.Core (string-driven ORDER BY + reflection whitelist) |
| Cache / Rate Limiting | Redis 7 + StackExchange.Redis |
| Containerization | Docker + Docker Compose |
| CI/CD | GitHub Actions |
| Hosting | Azure App Service + Azure Static Web Apps |
| File Storage | Azure Blob Storage |
| Monitoring | Azure Application Insights |
| AI Categorization | Ollama (local dev) + Google Gemini Free Tier (production) |

---

## Architecture

This project follows **Clean Architecture** principles — each layer has a single responsibility and dependencies always point inward toward the domain.

```
SmartBudget/
├── SmartBudget.API/            # HTTP entry point — Controllers, Program.cs, Middlewares
├── SmartBudget.Application/    # Business logic — Services, DTOs, Interfaces, Validators
├── SmartBudget.Domain/         # Entities, Enums, pure Interfaces (no external dependencies)
├── SmartBudget.Infrastructure/ # EF Core, Repositories, CSV/PDF Parsers, Azure Blob
├── SmartBudget.Tests/          # xUnit unit tests
└── SmartBudget.slnx            # .NET 10 solution file
```

**Dependency flow:**
```
API → Application → Domain ← Infrastructure
```

**Soft Delete:** all business entities implement `ISoftDeletable` (`deleted_at`). A Global Query Filter in EF Core automatically excludes soft-deleted records from all queries — equivalent to Laravel's `SoftDeletes` trait.

---

## AI Integration

SmartBudget uses a two-layer AI strategy for transaction categorization — rule-based engine first, AI fallback for unrecognized labels.

A provider-agnostic interface (`IAiCategorizationService`) allows switching between providers via configuration with no code changes.

| Environment | Provider | Cost |
|---|---|---|
| Local development | Ollama (llama3.2 — runs in Docker) | Free |
| Production (Azure) | Google Gemini 1.5 Flash | Free tier — 1500 req/day |

The AI receives the raw bank label and the list of available categories, and returns a category suggestion with a confidence score. Accepted suggestions are persisted as new categorization rules to reduce future API calls.

---

## Prerequisites

- [Docker](https://www.docker.com/get-started) 24+
- [Docker Compose](https://docs.docker.com/compose/) v2+
- [VS Code](https://code.visualstudio.com/) with the following extensions:
  - [C# Dev Kit](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csdevkit)
  - [Dev Containers](https://marketplace.visualstudio.com/items?itemName=ms-vscode-remote.remote-containers)

> The .NET 10 SDK is included in the Dev Container — no local installation required.

---

## Installation & Setup

### 1. Clone the repository

```bash
git clone https://github.com/fernandtchassem/smartbudget.git
cd smartbudget
```

### 2. Configure the application settings

`appsettings.Development.json` is tracked with Docker defaults — **no action needed** if you run via Docker Compose.

If you run outside Docker (local .NET), copy the example and fill in your values:

```bash
cp SmartBudget.API/appsettings.Development.json.example SmartBudget.API/appsettings.Development.json
```

### 3. Start the containers

```bash
docker compose up -d
```

This starts:
- `smart_budget_aspnet` — the .NET API (port 8080)
- `smart_budget_mysql` — the MySQL database (port 3306)
- `smart_budget_redis` — the Redis cache / rate-limit store (port 6379)

### 4. Open in VS Code (Dev Container)

```
Ctrl+Shift+P → Dev Containers: Reopen in Container
```

### 5. Apply EF Core migrations

Inside the container terminal:

```bash
# Move into the startup project (provides DI + connection string)
cd SmartBudget.API

# Generate the migration — reads the DbContext from Infrastructure and creates
# the migration files (Up/Down SQL + model snapshot) in SmartBudget.Infrastructure/Migrations/
dotnet ef migrations add InitialCreate --project ../SmartBudget.Infrastructure

# Apply pending migrations to the MySQL database
dotnet ef database update --project ../SmartBudget.Infrastructure
```

### 6. Run the API

```bash
dotnet run
```

The API is available at `http://localhost:8080`
The Scalar documentation is available at `http://localhost:8080/scalar`

---

## Configuration

ASP.NET Core uses a layered configuration system — no `.env` file required.

| File | Purpose | Committed to Git |
|---|---|---|
| `appsettings.json` | Base defaults — all keys, no secrets | Yes |
| `appsettings.Development.json` | Docker dev overrides (container hostnames, dev JWT key) | Yes |
| `appsettings.Development.json.example` | Template for running outside Docker — copy and fill in | Yes |
| `appsettings.Production.json` | Production secrets | No — in `.gitignore` |

**`appsettings.Development.json`** — tracked, Docker dev defaults (container hostnames, passwordless root):

```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft.AspNetCore": "Warning",
        "Microsoft.EntityFrameworkCore.Database.Command": "Warning"
      }
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=mysql_database;Port=3306;Database=smart_budget;User=root;Password=;"
  },
  "Redis": {
    "ConnectionString": "redis_database:6379"
  },
  "JwtSettings": {
    "SecretKey": "your-dev-secret-key-minimum-32-characters"
  }
}
```

**`appsettings.Development.json.example`** — copy this when running outside Docker, then fill in your local values:

```bash
cp SmartBudget.API/appsettings.Development.json.example SmartBudget.API/appsettings.Development.json
```

**`appsettings.Production.json`** — gitignored, never committed. Fill in production secrets on the server or Azure App Service:

```json
{
  "Serilog": { "MinimumLevel": { "Default": "Warning" } },
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_HOST;Port=3306;Database=smart_budget;User=smartbudget;Password=YOUR_PASSWORD;"
  },
  "Redis": {
    "ConnectionString": "YOUR_REDIS_HOST:6380,password=YOUR_PASSWORD,ssl=True,abortConnect=False"
  },
  "JwtSettings": {
    "SecretKey": "your-production-secret-key-minimum-32-characters"
  },
  "AzureBlob": {
    "ConnectionString": "DefaultEndpointsProtocol=https;AccountName=YOUR_ACCOUNT;AccountKey=YOUR_KEY;EndpointSuffix=core.windows.net",
    "ContainerName": "smartbudget-files"
  }
}
```

**Docker Compose environment variables** use the `__` (double underscore) convention to map to nested JSON keys:

```yaml
environment:
  - ASPNETCORE_ENVIRONMENT=Development
  - ASPNETCORE_URLS=http://+:8080
  - ConnectionStrings__DefaultConnection=Server=mysql_database;Port=3306;...
  - JwtSettings__SecretKey=your-secret-key
  - Redis__ConnectionString=redis_database:6379
```

> `appsettings.Development.json` is committed with Docker defaults so `docker compose up` works out of the box. `appsettings.Production.json` is gitignored — inject production secrets via environment variables or Azure App Service configuration.

---

## Project Structure

```
SmartBudget.Domain/
├── Entities/
│   ├── User.cs
│   ├── RefreshToken.cs
│   ├── Transaction.cs
│   ├── Category.cs
│   ├── CategoryRule.cs
│   ├── BankAccount.cs
│   ├── ImportBatch.cs
│   └── Budget.cs
├── Enums/
│   ├── Currency.cs
│   ├── TransactionType.cs
│   └── ImportStatus.cs
├── Interfaces/
│   ├── ISoftDeletable.cs
│   ├── IHasTimestamps.cs
│   ├── IDataSeeder.cs
│   ├── Repositories/
│   │   ├── IRepository.cs
│   │   ├── IUserRepository.cs
│   │   ├── IRefreshTokenRepository.cs
│   │   ├── ITransactionRepository.cs
│   │   ├── ICategoryRepository.cs
│   │   ├── ICategoryRuleRepository.cs
│   │   └── IBudgetRepository.cs
│   └── Services/
│       ├── ITokenService.cs
│       ├── IPasswordHasher.cs
│       └── IRateLimitService.cs

SmartBudget.Application/
├── Services/
│   ├── AuthService.cs
│   ├── UserService.cs
│   ├── CategoryService.cs
│   ├── CategoryRuleService.cs
│   ├── ImportCsvService.cs
│   ├── ImportPdfService.cs
│   ├── CategoryRuleEngine.cs
│   ├── DashboardService.cs
│   └── BudgetService.cs
├── DTOs/
│   ├── Auth/
│   │   ├── RegisterRequest.cs
│   │   ├── AuthResponse.cs
│   │   ├── LoginRequest.cs
│   │   ├── RefreshRequest.cs
│   │   └── RevokeRequest.cs
│   ├── User/
│   │   ├── UserResponse.cs
│   │   └── UpdateUserRequest.cs
│   ├── Category/
│   │   ├── CategoryResponse.cs
│   │   ├── CreateCategoryRequest.cs
│   │   └── UpdateCategoryRequest.cs
│   ├── CategoryRule/
│   │   ├── CategoryRuleResponse.cs
│   │   └── CreateCategoryRuleRequest.cs
│   ├── Transactions/
│   ├── Dashboard/
│   └── Import/
├── Errors/
│   ├── AuthError.cs
│   └── CategoryError.cs
├── Validators/
│   ├── Auth/
│   │   ├── RegisterDtoValidator.cs
│   │   ├── LoginDtoValidator.cs
│   │   ├── RefreshTokenDtoValidator.cs
│   │   └── RevokeTokenDtoValidator.cs
│   ├── User/
│   │   └── UpdateUserDtoValidator.cs
│   ├── Category/
│   │   ├── CreateCategoryDtoValidator.cs
│   │   └── UpdateCategoryDtoValidator.cs
│   └── CategoryRule/
│       └── CreateCategoryRuleDtoValidator.cs
└── DependencyInjection.cs

SmartBudget.Infrastructure/
├── Persistence/
│   ├── Configurations/
│   │   ├── UserConfiguration.cs
│   │   ├── RefreshTokenConfiguration.cs
│   │   └── CategoryConfiguration.cs
│   ├── SmartBudgetDbContext.cs
│   ├── SoftDeleteInterceptor.cs
│   ├── HasTimestampsInterceptor.cs
│   └── Migrations/
├── Repositories/
│   ├── Repository.cs
│   ├── UserRepository.cs
│   ├── RefreshTokenRepository.cs
│   ├── TransactionRepository.cs
│   ├── CategoryRepository.cs
│   ├── CategoryRuleRepository.cs
│   └── BudgetRepository.cs
├── Seeders/
│   ├── CategorySeeder.cs
│   └── CategoryRuleSeeder.cs
├── Services/
│   ├── TokenService.cs
│   ├── PasswordHasher.cs
│   └── RateLimitService.cs
├── Parsers/
│   ├── CsvParser.cs
│   └── PdfParser.cs
└── DependencyInjection.cs

SmartBudget.API/
├── Authorization/
│   ├── Operation/
│   │   ├── CategoryOperations.cs
│   │   ├── CategoryRuleOperations.cs
│   │   └── UserOperations.cs
│   ├── CategoryAuthorizationHandler.cs
│   ├── CategoryRuleAuthorizationHandler.cs
│   └── UserAuthorizationHandler.cs
├── Controllers/
│   ├── AuthController.cs
│   ├── UserController.cs
│   ├── CategoryController.cs
│   ├── CategoryRuleController.cs
│   ├── TransactionsController.cs
│   ├── ImportsController.cs
│   ├── DashboardController.cs
│   └── BudgetsController.cs
├── Results/
│   └── UnprocessableEntityResultFactory.cs
├── Middlewares/
│   ├── ExceptionMiddleware.cs
│   └── RateLimitMiddleware.cs
├── DependencyInjection.cs
├── appsettings.json
├── appsettings.Development.json.example
└── Program.cs
```

---

## JWT Authentication — Implementation Guide

This project uses a **custom JWT approach** — no ASP.NET Identity. The domain `User` entity stays clean, password hashing is handled by `BCrypt.Net`, and token management is fully owned by the application layer.

> Reference: [Building a Secure API with ASP.NET Core, JWT and Refresh Tokens](https://medium.com/@MatinGhanbari/building-a-secure-api-with-asp-net-core-jwt-and-refresh-tokens-03dac37b4055)

### 1. Packages to install

```bash
# SmartBudget.API
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer

# SmartBudget.Infrastructure
dotnet add package BCrypt.Net-Next
```

### 2. Domain layer

**`RefreshToken` entity** (`SmartBudget.Domain/Entities/RefreshToken.cs`):
```csharp
public class RefreshToken
{
    public string Id { get; init; } = Guid.NewGuid().ToString();
    public required string Token { get; set; }       // cryptographically random string
    public required string UserId { get; set; }
    public User User { get; set; } = null!;
    public required DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsRevoked { get; set; } = false;
}
```

Add the navigation property on `User`:
```csharp
public ICollection<RefreshToken> RefreshTokens { get; set; } = [];
```

**`ITokenService` interface** (`SmartBudget.Domain/Interfaces/Services/ITokenService.cs`):
```csharp
public interface ITokenService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
}
```

### 3. Application layer

**DTOs** (`SmartBudget.Application/DTOs/Auth/`):
```
RegisterRequest.cs   — Email, Password, FirstName, LastName, Currency
AuthResponse.cs      — AccessToken, RefreshToken, ExpiresIn, TokenType, User
LoginRequest.cs      — Email, Password
RefreshRequest.cs    — Token (expired access token), RefreshToken
```

**User DTO** (`SmartBudget.Application/DTOs/User/`):
```
UserResponse.cs      — Id, Email, FirstName, LastName, Currency, MonthStartDay, CreatedAt
```

**`AuthService`** (`SmartBudget.Application/Services/AuthService.cs`) — orchestrates:
- `Register`: hash password with `IPasswordHasher`, persist user, generate token pair, return `AuthResponse`
- `Login`: find user by email, verify BCrypt hash, generate + persist refresh token, return `AuthResponse`
- `Refresh`: validate expired access token via `GetPrincipalFromExpiredToken`, check refresh token in DB (not revoked, not expired, matches user), revoke old token, issue new pair, return `AuthResponse`

### 4. Infrastructure layer

**`TokenService`** (`SmartBudget.Infrastructure/Services/TokenService.cs`):
```csharp
// GenerateAccessToken: build Claims (NameIdentifier, Email, Role),
//   sign with HMACSHA256 key from JwtSettings:SecretKey, 15-min expiry
// GenerateRefreshToken: Convert.ToBase64String(RandomNumberGenerator.GetBytes(64))
// GetPrincipalFromExpiredToken: ValidateToken with ValidateLifetime = false,
//   ValidIssuer = config["JwtSettings:Issuer"],
//   ValidAudience = config["JwtSettings:Audience"]
//   — ValidIssuer and ValidAudience MUST be set even with ValidateLifetime = false;
//     omitting them causes ValidateToken to throw, returning null principal
```

**`RefreshTokenConfiguration`** (`SmartBudget.Infrastructure/Persistence/Configurations/RefreshTokenConfiguration.cs`):
```csharp
builder.ToTable("refresh_tokens");
builder.HasKey(r => r.Id);
builder.Property(r => r.Token).IsRequired().HasMaxLength(128);
builder.Property(r => r.ExpiresAt).IsRequired();
builder.HasOne(r => r.User)
       .WithMany(u => u.RefreshTokens)
       .HasForeignKey(r => r.UserId)
       .OnDelete(DeleteBehavior.Cascade);
```

Add `DbSet<RefreshToken> RefreshTokens` to `SmartBudgetDbContext`.

### 5. API layer

**`appsettings.json`** — add the `JwtSettings` block:
```json
"JwtSettings": {
  "SecretKey": "your-super-secret-key-minimum-32-characters",
  "Issuer": "SmartBudget",
  "Audience": "SmartBudgetUsers",
  "AccessTokenExpiryMinutes": 15,
  "RefreshTokenExpiryDays": 7
}
```

**`DependencyInjection.cs`** (`SmartBudget.API/DependencyInjection.cs`) — JWT auth is registered in `AddPresentation(IConfiguration config)`:
```csharp
services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer           = true,
            ValidateAudience         = true,
            ValidateLifetime         = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer              = config["JwtSettings:Issuer"],
            ValidAudience            = config["JwtSettings:Audience"],
            IssuerSigningKey         = new SymmetricSecurityKey(
                                           Encoding.UTF8.GetBytes(config["JwtSettings:SecretKey"]!))
        };
    });
```

**`Program.cs`** — lean bootstrap, no inline configuration:
```csharp
builder.Services.AddPresentation(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

app.UseAuthentication();
app.UseAuthorization();
```

**`AuthController`** (`SmartBudget.API/Controllers/AuthController.cs`):

| Method | Route | Auth | Body | Description |
|---|---|---|---|---|
| `POST` | `/api/auth/register` | — | `RegisterRequest` | Create account, return token pair |
| `POST` | `/api/auth/login` | — | `LoginRequest` | Authenticate, return token pair |
| `POST` | `/api/auth/refresh` | — | `RefreshRequest` | Rotate token pair |
| `POST` | `/api/auth/revoke` | Bearer | `{ refreshToken }` | Revoke a specific refresh token (logout) |

### 6. Token flow

```
POST /api/auth/login
  → validate credentials via FluentValidation (422 on failure)
  → verify password with BCrypt (401 if mismatch)
  → generate access token (JWT, 15 min, claims: userId + email)
  → generate refresh token (32 random bytes → base64, 7 days), persist to DB
  → return { accessToken, refreshToken, expiresIn, tokenType, user }

[client stores both; uses accessToken on every request]

POST /api/auth/refresh  (when accessToken expires)
  → validate expired accessToken signature (lifetime check disabled)
  → extract userId from NameIdentifier claim
  → look up user in DB: refresh token must match, not revoked, not expired
  → revoke old refresh token (set IsRevoked = true)
  → generate + persist new token pair
  → return new { accessToken, refreshToken, expiresIn, tokenType, user }

POST /api/auth/revoke  [Authorize]
  → extract userId from JWT (Bearer token required)
  → look up refresh token in DB, verify it belongs to the current user
  → set IsRevoked = true
  → return 204 No Content
```

---

## Automatic Timestamp Tracking

All business entities that need an `updated_at` column implement `IHasTimestamps`. A dedicated EF Core interceptor sets the value automatically on every save — no service or controller ever touches it manually.

```
SmartBudget.Domain/Interfaces/IHasTimestamps.cs   ← interface: UpdatedAt
SmartBudget.Infrastructure/Persistence/HasTimestampsInterceptor.cs  ← sets UpdatedAt on EntityState.Modified
```

**Key rule:** `HasTimestampsInterceptor` runs **before** `SoftDeleteInterceptor` in the interceptor chain. When a soft delete occurs, the entity is still `EntityState.Deleted` when the timestamps interceptor runs — so `UpdatedAt` is intentionally **not** updated on soft delete.

To add timestamp tracking to a new entity:
1. Implement `IHasTimestamps` on the entity class — no other changes needed.

---

## Resource-based Authorization

Protected resources (User, Category, CategoryRule) use ASP.NET Core's `IAuthorizationService` with resource-based handlers. Authorization is evaluated in the controller **before** the service is called — the service contains no ownership logic.

```
SmartBudget.API/Authorization/
├── Operation/
│   ├── CategoryOperations.cs       ← View, Update, Delete
│   ├── CategoryRuleOperations.cs   ← Delete
│   └── UserOperations.cs           ← View, Update
├── CategoryAuthorizationHandler.cs
├── CategoryRuleAuthorizationHandler.cs
└── UserAuthorizationHandler.cs
```

**Request flow for a mutating endpoint:**
```
[Authorize] middleware     ← validates JWT (authentication)
       ↓
Controller loads resource  ← GetByIdAsync → 404 if missing
       ↓
authorizationService       ← runs matching IAuthorizationHandler
  .AuthorizeAsync(User, resource, Operation)
       ↓
Handler evaluates rules    ← Succeed / implicit Fail
       ↓
Service executes           ← pure business logic, no auth concern
```

**`context.Fail()` vs doing nothing:**
- **Do nothing** — "I have no opinion"; other handlers can still succeed the requirement. Use for ownership checks.
- **`context.Fail()`** — hard denial that overrides all other handlers. Use only for absolute invariants (e.g. system categories that nobody can modify).

To add authorization for a new resource:
1. Create `Operation/XxxOperations.cs` with the required operations.
2. Create `XxxAuthorizationHandler.cs` implementing `AuthorizationHandler<OperationAuthorizationRequirement, XxxResponse>`.
3. Register in `DependencyInjection.cs`: `services.AddSingleton<IAuthorizationHandler, XxxAuthorizationHandler>();`

---

## Rate Limiting — Redis Sliding Window

All API endpoints are protected by a distributed rate limiter backed by **Redis**. It uses a sliding window algorithm implemented with a Lua script (atomic execution — no race conditions under concurrent requests).

### How it works

```
Request → RateLimitMiddleware
               ↓
   Determine key + limits
   (IP or userId / auth vs global)
               ↓
   Redis Lua script (atomic)
   ZREMRANGEBYSCORE + ZCARD + ZADD + PEXPIRE
               ↓
   count < limit → 200 + X-RateLimit-* headers
   count ≥ limit → 429 + Retry-After header
```

### Policies

| Endpoint group | Key | Limit | Window |
|---|---|---|---|
| `/api/auth/*` | Client IP | 10 requests | 60 s |
| All other endpoints (anonymous) | Client IP | 200 requests | 60 s |
| All other endpoints (authenticated) | User ID (from JWT) | 200 requests | 60 s |

Auth endpoints are always keyed by IP regardless of authentication state — this prevents an attacker with a stolen token from bypassing brute-force protection.

### Response headers

Every response includes:

| Header | Description |
|---|---|
| `X-RateLimit-Limit` | Maximum requests allowed in the window |
| `X-RateLimit-Remaining` | Requests remaining in the current window |
| `Retry-After` | Seconds until the window resets (only on `429`) |

### 429 response body

```json
{
  "type": "https://tools.ietf.org/html/rfc6585#section-4",
  "title": "Too Many Requests",
  "status": 429,
  "retryAfterSeconds": 42
}
```

### Files

```
SmartBudget.Domain/Interfaces/Services/IRateLimitService.cs   ← interface
SmartBudget.Infrastructure/Services/RateLimitService.cs       ← Redis implementation (Lua script)
SmartBudget.API/Middlewares/RateLimitMiddleware.cs            ← middleware (runs before authentication)
```

### Configuration

Configured via `RateLimitSettings` in `appsettings.json`:

```json
"RateLimitSettings": {
  "Enabled": true,
  "GlobalWindowSeconds": 60,
  "GlobalMaxRequests": 200,
  "AuthWindowSeconds": 60,
  "AuthMaxRequests": 10
}
```

Set `"Enabled": false` to disable rate limiting entirely (e.g. during integration tests).

---

## API Documentation

Interactive documentation is automatically generated via **Scalar** (OpenAPI).

```
http://localhost:8080/scalar
```

### Key Endpoints

| Method | Route | Auth | Description |
|---|---|---|---|
| `POST` | `/api/auth/register` | — | User registration, returns token pair |
| `POST` | `/api/auth/login` | — | Login, returns token pair |
| `POST` | `/api/auth/refresh` | — | Rotate access + refresh token |
| `POST` | `/api/auth/revoke` | Bearer | Revoke refresh token (logout) |
| `GET` | `/api/users/{id}` | Bearer + owner | Get user profile |
| `PUT` | `/api/users/{id}` | Bearer + owner | Update profile (name, currency, password) |
| `GET` | `/api/categories` | Bearer | Paginated list (own + system) — `?page`, `pageSize`, `sortBy`, `search`, `isIncome`, `isDefault` |
| `GET` | `/api/categories/{id}` | Bearer + owner/default | Get category by ID |
| `POST` | `/api/categories` | Bearer | Create user-defined category |
| `PUT` | `/api/categories/{id}` | Bearer + owner | Update user-defined category |
| `DELETE` | `/api/categories/{id}` | Bearer + owner | Soft-delete user-defined category |
| `GET` | `/api/transactions` | Bearer | Paginated list with filters |
| `PATCH` | `/api/transactions/{id}/category` | Bearer | Update transaction category |
| `POST` | `/api/imports/csv` | Bearer | Import CSV bank statement |
| `POST` | `/api/imports/pdf` | Bearer | Import PDF bank statement |
| `GET` | `/api/dashboard/summary` | Bearer | Monthly summary |
| `GET` | `/api/dashboard/by-category` | Bearer | Breakdown by category |
| `GET` | `/api/dashboard/trends` | Bearer | Trend over N months |
| `GET` | `/api/categories/{id}/rules` | Bearer + view | Paginated rules for a category (own + system) |
| `POST` | `/api/categories/{id}/rules` | Bearer + view | Add a rule to a category |
| `DELETE` | `/api/categories/rules/{ruleId}` | Bearer + owner | Soft-delete a rule |
| `GET/POST/DELETE` | `/api/budgets` | Bearer | Budgets CRUD |
| `GET` | `/api/exports/pdf` | Bearer | Filtered PDF export |

All protected endpoints require an `Authorization: Bearer {token}` header.

### Error responses

All error responses follow **RFC 7807 ProblemDetails** — no plain strings, no empty bodies:

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.5",
  "title": "Not Found",
  "status": 404,
  "detail": "Category not found."
}
```

| Status | Meaning |
|---|---|
| `401` | Missing or invalid JWT / bad credentials |
| `403` | Authenticated but not allowed (ownership check failed) |
| `404` | Resource does not exist |
| `422` | Validation failed — body is `ValidationProblemDetails` with per-field errors |
| `429` | Rate limit exceeded — includes `Retry-After` header |
| `500` | Unexpected server error |

---

## Testing

```bash
# Inside the container
cd /app
dotnet test

# With code coverage
dotnet test --collect:"XPlat Code Coverage"
```

Coverage target: **>= 80%** on business services (`SmartBudget.Application`).

---

## Roadmap

- [x] Database schema (v2 with soft delete)
- [x] Clean Architecture setup + Docker Dev Container
- [x] Core domain entities (User, RefreshToken) + EF Core configurations
- [x] JWT authentication — register endpoint (access + refresh token)
- [x] JWT authentication — login endpoint
- [x] JWT authentication — refresh endpoint (token rotation)
- [x] JWT authentication — revoke endpoint
- [x] Automatic `UpdatedAt` tracking via `IHasTimestamps` + `HasTimestampsInterceptor`
- [x] Resource-based authorization (`IAuthorizationHandler`) for User and Category
- [x] Category domain entity + EF Core configuration + seeder (12 system categories)
- [x] Category CRUD endpoints (GET, POST, PUT, DELETE) with ownership policies
- [x] Category rule endpoints — GET/POST nested under `/api/categories/{id}/rules`, DELETE at `/api/categories/rules/{ruleId}` — with resource-based authorization
- [x] User profile endpoints (GET, PUT) with ownership policies
- [x] Redis rate limiting — sliding window middleware (global + per-auth-endpoint policies)
- [x] Generic pagination — `PagedResponse<T>` + `PaginationFilter` in Domain, `QueryableExtensions` (sort + paginate on `IQueryable<T>`), opt-in per repository
- [ ] Remaining domain entities + EF Core migrations (Transaction, Budget, BankAccount…)
- [ ] End-to-end CSV import
- [ ] Automatic categorization rule engine
- [ ] AI-powered transaction categorization (Ollama + Gemini)
- [ ] PDF import (PdfPig)
- [ ] Angular dashboard + charts
- [ ] Monthly budgets + alerts
- [ ] PDF / CSV export
- [ ] GitHub Actions CI/CD pipeline
- [ ] Azure deployment
- [ ] E2E tests (Playwright)

---

## Author

**Fernand Tchassem** — Senior Full-Stack Developer

Ottawa, ON, Canada

[![Email](https://img.shields.io/badge/Email-fernandtchassem%40gmail.com-D14836?style=flat-square&logo=gmail&logoColor=white)](mailto:fernandtchassem@gmail.com)
[![Portfolio](https://img.shields.io/badge/Portfolio-fernandtchassem.onrender.com-000000?style=flat-square&logo=vercel&logoColor=white)](https://fernandtchassem.onrender.com)
[![Phone](https://img.shields.io/badge/Phone-(613)%20880--3072-25D366?style=flat-square&logo=whatsapp&logoColor=white)](tel:+16138803072)

---

*Portfolio project — .NET 10 · Angular 17 · MySQL · Azure · Docker*