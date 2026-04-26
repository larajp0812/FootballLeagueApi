# Football League Platform

Football League Platform is a full-stack application with:

- A .NET 8 REST backend API for managing teams, players, seasons, venues, matches, and match events
- A React single-page frontend that consumes the backend API for authentication, CRUD workflows, standings, and role administration

The backend follows a layered architecture with Controllers, Services, and Repositories, and uses Entity Framework Core with SQLite.

## Highlights

- RESTful CRUD endpoints across core football entities
- ASP.NET Core Identity for user management
- JWT authentication with role-based authorization (User/Admin)
- Optional SMTP email delivery for welcome/JWT onboarding messages
- Global rate limiting (per IP)
- Health checks endpoint for platform readiness probing
- Azure monitoring support with Application Insights
- DTO-based contracts and DataAnnotations validation
- Global exception middleware + structured logging
- Swagger/OpenAPI documentation
- Unit tests using xUnit + Moq
- Docker multi-stage build
- React frontend with route-based SPA navigation
- Frontend unit tests using Vitest + React Testing Library

## Tech Stack

- .NET 8 / ASP.NET Core Web API
- Entity Framework Core 8
- SQLite
- ASP.NET Core Identity
- JWT Bearer Authentication
- Swashbuckle (Swagger)
- xUnit + Moq
- React 19
- React Router 7
- React Bootstrap 2
- Vite 7
- Vitest 4

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
frontend/      React SPA client application
```

## Getting Started

### Prerequisites

- .NET SDK 8.0+
- Node.js 18+
- npm 9+
- (Optional) Docker

### Run Locally

1. Start the backend API

   ```bash
   dotnet restore
   dotnet ef database update
   dotnet build
   dotnet run
   ```

2. Start the frontend app (in a second terminal)

  ```bash
  cd frontend
  npm install
  npm run dev
  ```

Swagger UI is available at:

- `https://localhost:5240/swagger` (or your launch profile HTTPS port)

Frontend app is available at:

- `http://localhost:5173`

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

### Optional Azure monitoring settings

Application Insights is enabled in code. Set the connection string via configuration or environment variable:

```bash
APPLICATIONINSIGHTS_CONNECTION_STRING=<your-app-insights-connection-string>
```

If the connection string is not set, telemetry export is effectively disabled.

### Admin seed user

```json
  "AdminUser": {
    "UserName": "admin",
    "Email": "admin@admin.com",
    "Password": "Password123!"
  }
```

## Authentication & Authorization

- `POST /api/auth/register` registers a user and returns JWT token
- `POST /api/auth/login` returns JWT, expiry, refresh token, and refresh token expiry
- `POST /api/auth/refresh` exchanges a valid refresh token for a new access token
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
  "expires": "2026-03-01T10:00:00Z",
  "refreshToken": "<refresh-token>",
  "refreshTokenExpires": "2026-03-08T10:00:00Z"
}
```

### Refresh request

```json
{
  "email": "user@example.com",
  "refreshToken": "<refresh-token>"
}
```

### Refresh response

```json
{
  "token": "<new-jwt-token>",
  "expires": "2026-03-01T11:00:00Z",
  "refreshToken": "<new-refresh-token>",
  "refreshTokenExpires": "2026-03-08T11:00:00Z"
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

## Health Checks & Monitoring

- Health endpoint: `GET /health`
- Intended for infrastructure probes and readiness checks (for example Azure App Service Health Check path)
- Endpoint is lightweight and returns healthy/unhealthy status from the ASP.NET Core health check pipeline
- Application Insights telemetry can be enabled with `APPLICATIONINSIGHTS_CONNECTION_STRING`

### Azure App Service recommendation

- In Azure App Service, set **Health check path** to `/health`.
- Configure `APPLICATIONINSIGHTS_CONNECTION_STRING` in App Service Configuration.
- Review traces/requests/exceptions in Application Insights and set alerts in Azure Monitor.

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

## Frontend Endpoint Coverage

The React frontend consumes backend endpoints through page-level features and service modules.

| Backend Endpoint                                                                                                                                                                                                            | Frontend Integration                                                                                                 |
| --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | -------------------------------------------------------------------------------------------------------------------- |
| `POST /api/auth/register`                                                                                                                                                                                                   | Registration flow in `frontend/src/pages/RegisterPage.jsx` via `frontend/src/services/authService.js`                |
| `GET /api/auth/confirm-email`                                                                                                                                                                                               | Email confirmation redirect handled in login query-state UX in `frontend/src/pages/LoginPage.jsx`                    |
| `POST /api/auth/login`                                                                                                                                                                                                      | Login form in `frontend/src/pages/LoginPage.jsx` via `frontend/src/services/authService.js`                          |
| `POST /api/auth/refresh`                                                                                                                                                                                                    | Session restore in `frontend/src/contexts/AuthContext.jsx` via `frontend/src/services/authService.js`                |
| `POST /api/auth/forgot-password`                                                                                                                                                                                            | Forgot-password form in `frontend/src/pages/ForgotPasswordPage.jsx` via `frontend/src/services/authService.js`       |
| `POST /api/auth/reset-password`                                                                                                                                                                                             | Reset-password form in `frontend/src/pages/ResetPasswordPage.jsx` via `frontend/src/services/authService.js`         |
| `GET /api/teams` / `POST /api/teams` / `PUT /api/teams/{id}` / `DELETE /api/teams/{id}`                                                                                                                                     | Teams management in `frontend/src/pages/TeamsPage.jsx` via `frontend/src/services/teamService.js`                    |
| `GET /api/players` / `POST /api/players` / `PUT /api/players/{id}` / `DELETE /api/players/{id}`                                                                                                                             | Players management in `frontend/src/pages/PlayersPage.jsx` via `frontend/src/services/playerService.js`              |
| `GET /api/seasons` / `POST /api/seasons` / `PUT /api/seasons/{id}` / `DELETE /api/seasons/{id}`                                                                                                                             | Seasons management in `frontend/src/pages/SeasonsPage.jsx` via `frontend/src/services/seasonService.js`              |
| `GET /api/matches` / `POST /api/matches` / `PUT /api/matches/{id}` / `DELETE /api/matches/{id}`                                                                                                                             | Matches management in `frontend/src/pages/MatchesPage.jsx` via `frontend/src/services/matchService.js`               |
| `GET /api/matchevents` / `POST /api/matchevents` / `PUT /api/matchevents/{id}` / `DELETE /api/matchevents/{id}`                                                                                                             | Match events management in `frontend/src/pages/MatchEventsPage.jsx` via `frontend/src/services/matchEventService.js` |
| `GET /api/standings`                                                                                                                                                                                                        | League table page in `frontend/src/pages/LeagueTablePage.jsx` via `frontend/src/services/standingsService.js`        |
| `GET /api/roles` / `GET /api/roles/id/{roleId}` / `POST /api/roles` / `PUT /api/roles` / `DELETE /api/roles/{roleId}` / `POST /api/roles/assign-role-to-user` / `GET /api/roles/users` / `DELETE /api/roles/users/{userId}` | Role and user administration in `frontend/src/pages/RolesPage.jsx` via `frontend/src/services/roleService.js`        |
| `GET /health`                                                                                                                                                                                                               | API availability indicator in `frontend/src/pages/DashboardPage.jsx` via `frontend/src/services/healthService.js`    |

## API Samples

Ready-to-run request examples are available in:

- `FootballLeagueApi.http`

## License

This project is provided for educational and portfolio purposes.
