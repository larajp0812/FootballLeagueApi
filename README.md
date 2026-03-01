# Football League API

A comprehensive REST API for managing football leagues, teams, players, matches, and seasons. Built with ASP.NET Core 8.0, Entity Framework Core, and SQLite.

## Features

- **RESTful API** - Full CRUD operations on leagues, teams, players, matches, venues, and seasons
- **Authentication & Authorization** - JWT token-based authentication with ASP.NET Core Identity
- **Database** - SQLite with Entity Framework Core, migrations, and proper relationships
- **API Documentation** - Swagger/OpenAPI for interactive API testing
- **Error Handling** - Global exception middleware for consistent error responses
- **Logging** - Structured logging across all layers
- **Testing** - Unit tests for controllers, services, and repositories using xUnit and Moq
- **Docker Support** - Multi-stage Dockerfile for containerized deployment
- **CI/CD** - GitHub Actions pipeline for automated build, test, and deployment

## Architecture

```
Controllers/        - HTTP request handlers
Services/           - Business logic layer
Repositories/       - Data access layer
Models/             - Domain entities
DTOs/               - Data transfer objects for API contracts
Data/               - Entity Framework DbContext
Migrations/         - Database version control
Middleware/         - Cross-cutting concerns (error handling)
Tests/              - Unit tests (xUnit + Moq)
```

## Prerequisites

- .NET 8.0 SDK
- Docker (optional, for containerized deployment)
- Git for version control

## Getting Started

### Local Development Setup

1. Clone the repository:

   ```bash
   git clone <repository-url>
   cd FootballLeagueApi
   ```

2. Restore dependencies:

   ```bash
   dotnet restore
   ```

3. Apply database migrations:

   ```bash
   dotnet ef database update
   ```

4. Build the project:

   ```bash
   dotnet build
   ```

5. Run the application:
   ```bash
   dotnet run
   ```

The API will be available at `https://localhost:7128` (HTTPS) or `http://localhost:5128` (HTTP).

### API Documentation

Once the application is running, visit `https://localhost:7128/swagger` to view the Swagger UI and test endpoints interactively.

Deployed Swagger UI: `https://ljp-football-league-avh9c5h2gcawctcv.canadacentral-01.azurewebsites.net/swagger/index.html`

## Database

The application uses SQLite (`league.db`) for data storage. The database is configured via Entity Framework Core with automatic migrations.

### Update Database from Models

```bash
dotnet ef migrations add <MigrationName>
dotnet ef database update
```

## Authentication

### Register a New User

```bash
POST /api/auth/register
Content-Type: application/json

{
  "userName": "johndoe",
  "email": "john@example.com",
  "password": "SecurePass123!"
}
```

### Login & Get JWT Token

```bash
POST /api/auth/login
Content-Type: application/json

{
  "email": "john@example.com",
  "password": "SecurePass123!"
}
```

Response:

```json
{
  "token": "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9...",
  "expires": "2026-02-23T17:30:00Z"
}
```

Use the token in subsequent API requests:

```bash
Authorization: Bearer <token>
```

### Verify Email + Authorization/RBAC (Assessment Checklist)

1. Configure SMTP and optional seeded admin in `appsettings.json`:

```json
"EmailSettings": {
  "SmtpHost": "smtp.example.com",
  "SmtpPort": 587,
  "SmtpUsername": "smtp-user",
  "SmtpPassword": "smtp-password",
  "FromEmail": "noreply@example.com",
  "FromName": "Football League API",
  "EnableSsl": true
},
"AdminUser": {
  "UserName": "admin",
  "Email": "admin@example.com",
  "Password": "AdminPass123!"
}
```

2. Start the API. On startup, roles `User` and `Admin` are seeded. If `AdminUser` is configured, that account is created and assigned `Admin`.
3. Register a normal user via `POST /api/auth/register`.

- Expected: `200 OK`
- Expected: welcome email sent (or warning log if SMTP is not configured correctly).

4. Call a write endpoint without token (example: `POST /api/teams`).

- Expected: `401 Unauthorized`.

5. Login as normal user and call `POST /api/teams` with Bearer token.

- Expected: `201 Created`.

6. Call a read endpoint without token (example: `GET /api/teams`).

- Expected: `200 OK` (GET endpoints are public).

7. Login as normal user and call `DELETE /api/teams/{id}` with Bearer token.

- Expected: `403 Forbidden` (authenticated but not in `Admin` role).

8. Login as admin user and call `DELETE /api/teams/{id}` with Bearer token.

- Expected: `204 No Content` (or `404` if the team ID does not exist).

## API Endpoints

### Teams

- `GET /api/teams` - Get all teams
- `GET /api/teams/{id}` - Get team by ID
- `POST /api/teams` - Create a new team
- `PUT /api/teams/{id}` - Update team
- `DELETE /api/teams/{id}` - Delete team

### Players

- `GET /api/players` - Get all players
- `GET /api/players/{id}` - Get player by ID
- `POST /api/players` - Create a new player
- `PUT /api/players/{id}` - Update player
- `DELETE /api/players/{id}` - Delete player

### Matches

- `GET /api/matches` - Get all matches
- `GET /api/matches/{id}` - Get match by ID
- `POST /api/matches` - Create a new match
- `PUT /api/matches/{id}` - Update match
- `DELETE /api/matches/{id}` - Delete match

### Seasons & Venues

Similar CRUD endpoints are available for Seasons (`/api/seasons`) and Venues (`/api/venues`).

## Testing

Run all tests:

```bash
dotnet test
```

Run tests with detailed output:

```bash
dotnet test --verbosity detailed
```

Run tests from the dedicated test project/folder:

```bash
dotnet test ./Tests
```

Collect code coverage (XPlat):

```bash
dotnet test --collect:"XPlat Code Coverage"
```

## Docker Deployment

### Build Docker Image

```bash
docker build -t football-league-api:latest .
```

### Run Container

```bash
docker run -p 8080:8080 \
  -e "Jwt__Key=your-secret-key" \
  -e "ConnectionStrings__DefaultConnection=Data Source=/data/league.db" \
  -v /app/data:/data \
  football-league-api:latest
```

The API will be accessible at `http://localhost:8080`.

## CI/CD Pipeline

The project includes a GitHub Actions workflow (`.github/workflows/ci-cd.yml`) that:

1. **Builds** the project on every push to `main` or `develop` branches
2. **Runs tests** to ensure code quality
3. **Builds & pushes Docker image** to GitHub Container Registry on successful main branch pushes

## Configuration

### appsettings.json

```json
{
  "Logging": { ... },
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=league.db"
  },
  "Jwt": {
    "Key": "dev_secret_change_this_in_production",
    "Issuer": "FootballLeagueApi",
    "Audience": "FootballLeagueApiUsers",
    "ExpiresMinutes": 60
  }
}
```

**SECURITY NOTE:** Never commit sensitive values like JWT secrets to version control. Use environment variables in production.

## Security

- Passwords are hashed and salted using ASP.NET Core Identity
- JWT tokens provide stateless, distributed authentication
- HTTPS enforced in production
- Input validation on all DTO models with DataAnnotations
- CORS configured for cross-origin requests
- SQL injection prevention via Entity Framework Core parameterized queries

## Project Design Decisions

### Repository Pattern

- Abstracts database access behind interfaces
- Enables easy testing with mock repositories
- Facilitates future database provider changes

### Service Layer

- Separates business logic from HTTP concerns
- Enables code reusability across endpoints
- Enforces dependency injection

### DTOs

- Decouples API contracts from internal domain models
- Prevents over/under-sharing sensitive data
- Cleaner API design with explicit contracts

### Entity Framework Core

- ORM eliminates manual SQL and boilerplate
- Migrations provide version control for schema changes
- Relationships properly configured with Fluent API in `LeagueContext.OnModelCreating()`

## Troubleshooting

### Database Locked

Delete `league.db` and re-run migrations:

```bash
dotnet ef database update
```

### Port Already in Use

Change the port in `Properties/launchSettings.json` or run on different port:

```bash
dotnet run --urls "https://localhost:7129"
```

## Submission & Viva Preparation

This project was developed for the **7SENG014W Web Application Development 2026** coursework. Key discussion points for the viva:

1. **Architecture** - Explain the layered architecture (Controllers → Services → Repositories → Data)
2. **Authentication** - How the JWT token flow works and why it's secure
3. **Database Design** - Relationship configuration between entities (Team-Player, Match-Team, Match-Event)
4. **Error Handling** - Global exception middleware and its benefits
5. **Testing Strategy** - Mocking dependencies and why unit tests matter
6. **Security** - Password hashing, input validation, HTTPS, CORS
7. **Deployment** - Docker containerization and CI/CD automation

## License

Part of 7SENG014W Web Application Development coursework (2026).
