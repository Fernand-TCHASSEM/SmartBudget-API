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
- [Prerequisites](#prerequisites)
- [Installation & Setup](#installation--setup)
- [Configuration](#configuration)
- [Project Structure](#project-structure)
- [JWT Authentication — Implementation Guide](#jwt-authentication--implementation-guide)
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
| Containerization | Docker + Docker Compose |
| CI/CD | GitHub Actions |
| Hosting | Azure App Service + Azure Static Web Apps |
| File Storage | Azure Blob Storage |
| Monitoring | Azure Application Insights |

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

```bash
cp SmartBudget.API/appsettings.Development.json.example SmartBudget.API/appsettings.Development.json
```

Edit `appsettings.Development.json` with your local values (see [Configuration](#configuration)).

### 3. Start the containers

```bash
docker compose up -d
```

This starts:
- `smart_budget_aspnet` — the .NET API (port 8080)
- `smart_budget_mysql` — the MySQL database (port 3306)

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
| `appsettings.json` | Default values, no secrets | Yes |
| `appsettings.Development.json` | Local development values | No — in `.gitignore` |
| `appsettings.Production.json` | Production values | No — in `.gitignore` |
| `appsettings.Development.json.example` | Template without secrets | Yes |

**`appsettings.Development.json`** (created from the example above):

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=smart_budget_mysql;Port=3306;Database=smart_budget;User Id=smartbudget;Password=SmartBudget123!;"
  },
  "JwtSettings": {
    "SecretKey": "your-super-secret-key-minimum-32-characters",
    "Issuer": "SmartBudget",
    "Audience": "SmartBudgetUsers",
    "AccessTokenExpiryMinutes": 15,
    "RefreshTokenExpiryDays": 7
  },
  "AzureBlob": {
    "ConnectionString": "",
    "ContainerName": "smartbudget-files"
  },
  "Serilog": {
    "LogFilePath": "/app/logs/smartbudget-.log"
  }
}
```

**Docker Compose environment variables** use the `__` (double underscore) convention to map to nested JSON keys:

```yaml
environment:
  - ASPNETCORE_ENVIRONMENT=Development
  - ASPNETCORE_URLS=http://+:8080
  - ConnectionStrings__DefaultConnection=Server=smart_budget_mysql;Port=3306;...
  - JwtSettings__SecretKey=your-secret-key
```

> Never commit `appsettings.Development.json` or `appsettings.Production.json`. Both are listed in `.gitignore`.

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
│   ├── Repositories/
│   │   ├── IRepository.cs
│   │   ├── IUserRepository.cs
│   │   ├── IRefreshTokenRepository.cs
│   │   ├── ITransactionRepository.cs
│   │   ├── ICategoryRepository.cs
│   │   └── IBudgetRepository.cs
│   └── Services/
│       ├── ITokenService.cs
│       └── IPasswordHasher.cs

SmartBudget.Application/
├── Services/
│   ├── AuthService.cs
│   ├── ImportCsvService.cs
│   ├── ImportPdfService.cs
│   ├── CategoryRuleEngine.cs
│   ├── DashboardService.cs
│   └── BudgetService.cs
├── DTOs/
│   ├── Auth/
│   │   ├── RegisterRequest.cs
│   │   └── RegisterResponse.cs
│   ├── Transactions/
│   ├── Dashboard/
│   └── Import/
├── Validators/
│   ├── Auth/
│   │   └── RegisterDtoValidator.cs
│   └── ImportValidator.cs
└── DependencyInjection.cs

SmartBudget.Infrastructure/
├── Persistence/
│   ├── Configurations/
│   │   ├── UserConfiguration.cs
│   │   └── RefreshTokenConfiguration.cs
│   ├── SmartBudgetDbContext.cs
│   ├── SoftDeleteInterceptor.cs
│   └── Migrations/
├── Repositories/
│   ├── UserRepository.cs
│   ├── RefreshTokenRepository.cs
│   ├── TransactionRepository.cs
│   ├── CategoryRepository.cs
│   └── BudgetRepository.cs
├── Services/
│   ├── TokenService.cs
│   └── PasswordHasher.cs
├── Parsers/
│   ├── CsvParser.cs
│   └── PdfParser.cs
└── DependencyInjection.cs

SmartBudget.API/
├── Controllers/
│   ├── AuthController.cs
│   ├── TransactionsController.cs
│   ├── ImportsController.cs
│   ├── DashboardController.cs
│   ├── CategoriesController.cs
│   └── BudgetsController.cs
├── Results/
│   └── UnprocessableEntityResultFactory.cs
├── Middlewares/
│   └── ExceptionMiddleware.cs
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
RegisterResponse.cs  — AccessToken, RefreshToken, ExpiresIn
LoginRequest.cs      — Email, Password
RefreshRequest.cs    — AccessToken (expired), RefreshToken
```

**`AuthService`** (`SmartBudget.Application/Services/AuthService.cs`) — orchestrates:
- `Register`: hash password with `IPasswordHasher`, persist user, return tokens
- `Login`: find user by email, verify password, generate + persist refresh token, return `RegisterResponse`
- `Refresh`: validate expired access token via `GetPrincipalFromExpiredToken`, check refresh token in DB (not revoked, not expired, matches user), revoke old token, issue new pair

### 4. Infrastructure layer

**`TokenService`** (`SmartBudget.Infrastructure/Services/TokenService.cs`):
```csharp
// GenerateAccessToken: build Claims (NameIdentifier, Email, Role),
//   sign with HMACSHA256 key from JwtSettings:SecretKey, 15-min expiry
// GenerateRefreshToken: Convert.ToBase64String(RandomNumberGenerator.GetBytes(64))
// GetPrincipalFromExpiredToken: ValidateToken with ValidateLifetime = false
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

| Method | Route | Body | Description |
|---|---|---|---|
| `POST` | `/api/auth/register` | `RegisterRequest` | Create account, return token pair |
| `POST` | `/api/auth/login` | `LoginRequest` | Authenticate, return token pair |
| `POST` | `/api/auth/refresh` | `RefreshRequest` | Rotate token pair |
| `POST` | `/api/auth/revoke` | — | `[Authorize]` — revoke current refresh token |

### 6. Token flow

```
POST /api/auth/login
  → verify password (BCrypt)
  → generate access token (JWT, 15 min)
  → generate refresh token (random, 7 days), persist to DB
  → return { accessToken, refreshToken }

[client stores both; uses accessToken on every request]

POST /api/auth/refresh  (when accessToken expires)
  → validate expired accessToken signature (lifetime check disabled)
  → look up refreshToken in DB: must be active + match userId
  → revoke old refreshToken (set revokedAt)
  → generate + persist new token pair
  → return new { accessToken, refreshToken }
```

---

## API Documentation

Interactive documentation is automatically generated via **Scalar** (OpenAPI).

```
http://localhost:8080/scalar
```

### Key Endpoints

| Method | Route | Description |
|---|---|---|
| `POST` | `/api/auth/register` | User registration |
| `POST` | `/api/auth/login` | Login and get JWT |
| `POST` | `/api/auth/refresh` | Refresh access token |
| `GET` | `/api/transactions` | Paginated list with filters |
| `PATCH` | `/api/transactions/{id}/category` | Update transaction category |
| `POST` | `/api/imports/csv` | Import CSV bank statement |
| `POST` | `/api/imports/pdf` | Import PDF bank statement |
| `GET` | `/api/dashboard/summary` | Monthly summary |
| `GET` | `/api/dashboard/by-category` | Breakdown by category |
| `GET` | `/api/dashboard/trends` | Trend over N months |
| `GET/POST/DELETE` | `/api/categories` | Categories CRUD |
| `GET/POST/DELETE` | `/api/rules` | Rules CRUD |
| `GET/POST/DELETE` | `/api/budgets` | Budgets CRUD |
| `GET` | `/api/exports/pdf` | Filtered PDF export |

All protected endpoints require an `Authorization: Bearer {token}` header.

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
- [ ] JWT authentication — login / refresh / revoke endpoints
- [ ] Remaining domain entities + EF Core migrations
- [ ] End-to-end CSV import
- [ ] Automatic categorization rule engine
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