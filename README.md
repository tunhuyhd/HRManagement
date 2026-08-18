# HRManagement API

ASP.NET Core 8 Web API using PostgreSQL, ASP.NET Core Identity, JWT authentication,
and role-based authorization.

## Structure

```text
HRManagement.Api/
|-- Common/Auth/          JWT options and current-user abstraction
|-- Constants/            Application-wide constants such as role names
|-- Controllers/          HTTP endpoints only
|-- Data/                 EF Core DbContext, migrations, and database seeding
|-- Entities/             Persistence entities
|-- Features/
|   |-- Auth/
|   |   |-- Models/       Authentication request and response models
|   |   `-- Services/     Authentication and JWT business logic
|   `-- Users/
|       |-- Models/       User-management request and response models
|       `-- Services/     User-management business logic
|-- Repositories/         Data-access implementations
|   `-- Interfaces/       Data-access contracts
`-- Program.cs            Dependency injection and HTTP pipeline
```

## Run locally

Requirements: .NET 8 SDK and Docker Desktop (or PostgreSQL listening on port 5432).

```powershell
docker compose up -d
dotnet run --project .\HRManagement.Api --urls http://localhost:5000
```

On startup, the API applies EF Core migrations and seeds:

- Role: `ADMIN`
- Email: `admin@gmail.com`
- Password: `admin123`

## Swagger

Swagger is enabled when the application runs in the `Development` environment.

When running from Visual Studio with the `https` profile:

```text
https://localhost:7029/swagger
```

When running from Visual Studio with the `http` profile:

```text
http://localhost:5159/swagger
```

When running with the command shown above on port `5000`:

```text
http://localhost:5000/swagger
```

Visual Studio is configured not to open a browser automatically. After starting the
API, open the appropriate Swagger URL manually. The active addresses are also shown
in the application output as `Now listening on: ...`.

Ready-to-run request examples are in `HRManagement.Api/HRManagement.Api.http`.

## Endpoints

| Method | Endpoint | Authorization | Description |
| --- | --- | --- | --- |
| POST | `/api/auth/login` | Anonymous | Login and receive a JWT |
| GET | `/api/auth/me` | Authenticated | Return the current user |
| POST | `/api/users` | `ADMIN` | Create an account with the `USER` role |

New users can log in immediately with the email and password assigned by an admin.

## Configuration

Local PostgreSQL defaults are configured as requested:

```text
Host=localhost;Port=5432;Database=hr_management;Username=postgres;Password=admin
```

Before production deployment, override these settings with environment variables or
a secret manager. Do not reuse the committed development credentials or JWT key.

```text
ConnectionStrings__DefaultConnection=...
Jwt__Issuer=...
Jwt__Audience=...
Jwt__Key=at-least-32-characters-and-random
```

## EF Core migrations

The initial migration is committed. For later schema changes:

```powershell
dotnet ef migrations add [MigrationName] `
  --project .\HRManagement.Api\HRManagement.Api.csproj `
  --startup-project .\HRManagement.Api\HRManagement.Api.csproj
dotnet ef database update `
  --project .\HRManagement.Api\HRManagement.Api.csproj `
  --startup-project .\HRManagement.Api\HRManagement.Api.csproj
```
