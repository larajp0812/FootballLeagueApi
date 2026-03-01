# Football League API

Football League API is a .NET 8 REST backend for managing teams, players, seasons, venues, matches, and match events.

It follows a layered architecture with Controllers, Services, and Repositories, and uses Entity Framework Core with SQLite.

## Highlights

- RESTful CRUD endpoints across core football entities
- ASP.NET Core Identity for user management
- JWT authentication with role-based authorization (User/Admin)
- Optional SMTP email delivery for welcome/JWT onboarding messages
- Global rate limiting (per IP)
- DTO-based contracts and DataAnnotations validation
- Global exception middleware + structured logging
- Swagger/OpenAPI documentation
- Unit tests using xUnit + Moq
- Docker multi-stage build

## Tech Stack

- .NET 8 / ASP.NET Core Web API
- Entity Framework Core 8
- SQLite
- ASP.NET Core Identity
- JWT Bearer Authentication
- Swashbuckle (Swagger)
- xUnit + Moq

## Project Structure

```
Controllers/   HTTP endpoints
Services/      Business logic
Repositories/  Data access abstractions
Models/        Domain models
DTOs/          API request/response contracts
Data/          DbContext and EF configuration
Migrations/    EF Core migrations
Middleware/    Cross-cutting concerns
Tests/         Unit tests
```

## Getting Started

### Prerequisites

- .NET SDK 8.0+
- (Optional) Docker

### Run Locally

1. Restore packages

   ```bash
   dotnet restore
   ```

2. Apply migrations

   ```bash
   dotnet ef database update
   ```

3. Build and run

   ```bash
   dotnet build
   dotnet run
   ```

Swagger UI is available at:

- `https://localhost:5240/swagger` (or your launch profile HTTPS port)

## Configuration

Configuration is read from `appsettings.json`, `appsettings.Development.json`, environment variables, and user secrets.

### Required JWT settings

```json
"Jwt": {
  "Key": "<long-random-secret>",
  "Issuer": "FootballLeagueApi",
  "Audience": "FootballLeagueApiUsers",
  "ExpiresMinutes": 60
}
```

### Optional email settings (SMTP)

If configured, registration sends a welcome email that includes the JWT and quick Swagger authorization steps.

```json
"EmailSettings": {
  "SmtpHost": "smtp.example.com",
  "SmtpPort": 587,
  "SmtpUsername": "smtp-user",
  "SmtpPassword": "smtp-password",
  "FromEmail": "no-reply@example.com",
  "FromName": "Football League API",
  "EnableSsl": true
}
```

### Admin seed user

```json
  "AdminUser": {
    "UserName": "admin",
    "Email": "admin@admin.com",
    "Password": "Password123!"
  }
```

> Security note: do not commit real secrets. Prefer environment variables or user secrets for sensitive values.

## Authentication & Authorization

- `POST /api/auth/register` registers a user and returns JWT token
- `POST /api/auth/login` returns JWT token and expiry
- Protected endpoints require: `Authorization: Bearer <token>`
- Role checks are enforced on admin-only operations
- Roles `User` and `Admin` are seeded on startup
- If SMTP is configured, registration also sends a formatted welcome email containing the JWT and Swagger usage steps

### Register response

```json
{
  "token": "<jwt-token>"
}
```

### Login response

```json
{
  "token": "<jwt-token>",
  "expires": "2026-03-01T10:00:00Z"
}
```

### Swagger Bearer Token Usage

1. Open Swagger UI
2. Click **Authorize**
3. Paste token value only (without `Bearer `)
4. Authorize and call protected endpoints

If SMTP is configured, the registration email also includes these Swagger steps.

## HTTPS & Transport Security

- HTTPS redirection is enabled in the middleware pipeline.
- JWT bearer options require HTTPS metadata.
- Local development runs on HTTPS launch profile ports (see `launchSettings.json`).

For production, terminate TLS at the edge (App Service/Ingress/Reverse Proxy) and keep HTTPS enforced end-to-end.

## Rate Limiting

- A global fixed-window limiter is enabled.
- Limit: **100 requests per minute per client IP**.
- Exceeded requests return **HTTP 429 Too Many Requests**.
- Policy applies globally to all API endpoints.

Implementation is configured in [Program.cs](Program.cs).

### Quick verification

You can verify throttling by sending repeated requests quickly to any endpoint (for example with Swagger or a REST client). Once the per-minute limit is exceeded, the API responds with `429 Too Many Requests`.

Example response (simplified):

```json
{
  "status": 429,
  "title": "Too Many Requests"
}
```

## Database

- SQLite database file: `league.db`
- Migrations are stored in `Migrations/`

Create a new migration:

```bash
dotnet ef migrations add <MigrationName>
dotnet ef database update
```

## Testing

Run all tests:

```bash
dotnet test
```

Run with coverage:

```bash
dotnet test --collect:"XPlat Code Coverage"
```

## Docker

Build image:

```bash
docker build -t football-league-api:latest .
```

Run container:

```bash
docker run -p 8080:8080 \
  -e "Jwt__Key=<your-secret>" \
  football-league-api:latest
```

## CI/CD

- Azure DevOps pipeline definition is included in `azure-pipelines.yml`
- The pipeline restores, builds, tests, publishes artifacts, and deploys to staging/production environments

## API Samples

Ready-to-run request examples are available in:

- `FootballLeagueApi.http`

## License

This project is provided for educational and portfolio purposes.
