# Football League API

Football League API is a .NET 8 REST backend for managing teams, players, seasons, venues, matches, and match events.

It follows a layered architecture with Controllers, Services, and Repositories, and uses Entity Framework Core with SQLite.

## Highlights

- RESTful CRUD endpoints across core football entities
- ASP.NET Core Identity for user management
- JWT authentication with role-based authorization (User/Admin)
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

- `https://localhost:7128/swagger` (or your launch profile HTTPS port)

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

### Optional email settings

```json
"EmailSettings": {
  "SmtpHost": "",
  "SmtpPort": 587,
  "SmtpUsername": "",
  "SmtpPassword": "",
  "FromEmail": "",
  "FromName": "Football League API",
  "EnableSsl": true
}
```

### Optional admin seed user

```json
"AdminUser": {
  "UserName": "admin",
  "Email": "",
  "Password": ""
}
```

> Security note: do not commit real secrets. Prefer environment variables or user secrets for sensitive values.

## Authentication & Authorization

- `POST /api/auth/register` registers a user
- `POST /api/auth/login` returns JWT token and expiry
- Protected endpoints require: `Authorization: Bearer <token>`
- Role checks are enforced on admin-only operations
- Roles `User` and `Admin` are seeded on startup

### Swagger Bearer Token Usage

1. Open Swagger UI
2. Click **Authorize**
3. Paste token value only (without `Bearer `)
4. Authorize and call protected endpoints

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
