# VIVA Preparation Notes

**Football League API - 7SENG014W Web Application Development 2026**

This document contains key design decisions, architectural patterns, and explanations for the oral exam.

---

## 1. Project Overview & Context

### What is the Football League API?
A REST API for managing football leagues, teams, players, matches, and seasons. Built to demonstrate:
- Clean architecture (Controllers → Services → Repositories → Data)
- Secure authentication (JWT + ASP.NET Identity)
- Professional software engineering practices (SOLID principles, testing, documentation)

### Why These 6 Entities?
- **Team** - Fundamental entity (has players, plays matches)
- **Player** - Belongs to team, appears in matches via MatchEvents
- **Season** - Contains multiple matches, provides temporal context
- **Match** - Connects two teams, belongs to season, happens at venue
- **Venue** - Stadium/location where matches are played
- **MatchEvent** - Individual occurrences within a match (goals, cards, substitutions)

**Relationships:**
- Team ↔ Player (1:many)
- Team ↔ Match (1:many via HomeTeamId/AwayTeamId)
- Season ↔ Match (1:many)
- Match ↔ Venue (1:many)
- Match ↔ MatchEvent (1:many)

---

## 2. Architecture Decisions

### Why Layered Architecture?

```
HTTP Request
    ↓
[Controller Layer]     — Handles HTTP requests/responses, no business logic
    ↓
[Service Layer]        — Business logic, orchestration, validation
    ↓
[Repository Layer]     — Data access abstraction, database queries
    ↓
[Data Layer]           — Entity Framework Core, DbContext
    ↓
[Database]             — SQLite (development) / SQL compatible (production)
```

**Benefits:**
- **Separation of Concerns** - Each layer has a specific responsibility
- **Testability** - Can mock repositories and test services in isolation
- **Maintainability** - Code changes in one layer don't break others
- **Reusability** - Services can be called from different controllers or endpoints
- **Follows SOLID** - Single Responsibility Principle in action

### Repository Pattern

**Why use repositories?**
```csharp
// Without repository:
var player = dbContext.Players.FirstOrDefault(p => p.PlayerId == id);  // Scattered across code

// With repository:
var player = playerRepository.GetByIdAsync(id);  // Centralized, testable
```

**Benefits:**
- Abstraction - Database implementation details hidden
- Easy to swap - Could switch to SQL Server, Cosmos DB without changing services
- Mockable - Can inject fake repositories in tests
- Consistent - All data access uses same interface

---

## 3. Authentication & Security

### JWT Token Flow

```
1. User makes POST /api/auth/register with credentials
   ↓
2. UserManager creates user with hashed password (ASP.NET Identity)
   ↓
3. User makes POST /api/auth/login with email + password
   ↓
4. Password verified against hash in database
   ↓
5. JWT token generated with claims (username, email, jti)
   ↓
6. Token returned to client with expiry time
   ↓
7. Client includes token in header: Authorization: Bearer <token>
   ↓
8. Middleware validates token signature, issuer, audience, expiry
   ↓
9. If valid, request proceeds; if invalid/expired, returns 401 Unauthorized
```

### Why JWT over Sessions?

| JWT | Sessions |
|-----|----------|
| Stateless - no server memory needed | Stateful - must store session data |
| Can distribute across servers | Requires shared session store |
| Mobile/API friendly | Browser-focused |
| Self-contained claims | Must lookup claims on each request |
| Scalable to microservices | Tightly coupled to monolith |

### Password Security

```csharp
// ASP.NET Identity handles this:
var result = await _userManager.CreateAsync(user, password);
// - Hashes with PBKDF2
// - Adds salt per user
// - Never stores plaintext
// - Can configure hash iterations (cost)
```

### Other Security Measures

- **HTTPS enforced** - All traffic encrypted
- **CORS configured** - Can restrict to trusted origins
- **Input validation** - DataAnnotations on all DTOs
- **SQL injection prevention** - EF Core parameterized queries
- **Secrets management** - JWT key via environment variables (not in code)

---

## 4. Database Design & Entity Framework

### Why Entity Framework Core?

```
Option 1: Raw ADO.NET
  - Write SQL manually, error-prone
  - Vulnerable to SQL injection if not careful
  - Must map data manually
  
Option 2: Entity Framework Core (chosen)
  - LINQ queries (type-safe)
  - Automatic parameterization (prevents injection)
  - Automatic type mapping
  - Built-in relationship management
  - Migrations for version control
```

### Fluent API Configuration

```csharp
// In LeagueContext.OnModelCreating():
builder.Entity<Match>()
    .HasOne(m => m.HomeTeam)
    .WithMany(t => t.HomeMatches)
    .HasForeignKey(m => m.HomeTeamId)
    .OnDelete(DeleteBehavior.Restrict);  // Can't delete team with matches

// Why Restrict? Business logic - protects referential integrity
// Can't have matches with deleted teams
```

### Migrations

```bash
dotnet ef migrations add TeamUpdated  # Creates migration file
dotnet ef database update              # Applies to database
```

**Benefits:**
- Version control for database schema
- Reproducible across environments
- Easy rollback if needed
- Team collaboration (see what changed)

---

## 5. DTOs & Data Validation

### Why DTOs?

```csharp
// Without DTOs:
public IActionResult Create(Player player)
{
    // Client sends PlayerId, TeamId, Team object, etc.
    // Exposed unnecessary data
    // API contract tightly coupled to database model
}

// With DTOs:
public IActionResult Create(PlayerCreateDto dto)
{
    // Client only sends: FullName, ShirtNumber, Position, TeamId
    // Cleaner contract, explicit about required fields
    // Can validate independently
}
```

**Benefits:**
- **API Contract** - Clear what input/output clients expect
- **Validation** - Centralized in DTO annotations
- **Security** - Don't expose unnecessary data (IDs, internal fields)
- **Evolution** - Can change internal model without breaking API

### Input Validation

```csharp
[Required(ErrorMessage = "Email is required")]
[EmailAddress(ErrorMessage = "Please enter a valid email address")]
public string Email { get; set; }
```

**Server-side validation is critical because:**
- Client-side validation can be bypassed
- Ensures data consistency in database
- Prevents malicious input
- Shows professional API design

---

## 6. Error Handling & Logging

### Global Exception Middleware

```csharp
// Catches ALL unhandled exceptions
// Returns consistent error response:
{
  "statusCode": 500,
  "message": "An internal server error occurred",
  "timestamp": "2026-02-23T12:00:00Z"
}
```

**Why global middleware?**
- Don't repeat try-catch in every controller
- Consistent error format across API
- Centralized logging for debugging
- Prevents leaking stack traces in production

### Logging Strategy

```csharp
_logger.LogInformation("User registered: {Email}", email);  // Important events
_logger.LogWarning("Player not found: {PlayerId}", playerId);  // Worth noting
_logger.LogError(ex, "Database error");  // Failures
_logger.LogDebug("Detailed execution flow");  // Development only
```

**Why structured logging?**
- Can filter by date, level, user
- Can integrate with monitoring (Azure Application Insights)
- Helps debug production issues
- Shows professional development practices

---

## 7. Testing Strategy

### Unit Tests (Current)

```csharp
[Fact]
public async Task GetAll_ReturnsOkResult_WithPlayersList()
{
    // Arrange
    var mockService = new Mock<IPlayerService>();
    mockService.Setup(s => s.GetAllAsync())
        .ReturnsAsync(new List<Player> { ... });
    
    // Act
    var result = await controller.GetAll();
    
    // Assert
    Assert.IsType<OkObjectResult>(result);
}
```

**Why mock?**
- Test controller logic without database
- Fast execution
- Isolate what you're testing
- No side effects

### Why Xunit + Moq?

- **Xunit** - Modern .NET testing framework, supports async/await
- **Moq** - Easy mocking library with clean syntax

---

## 8. API Design (RESTful Principles)

### REST Principles Followed

```
GET    /api/players          → Retrieve all (idempotent)
GET    /api/players/{id}     → Retrieve one (idempotent)
POST   /api/players          → Create (safe to retry once)
PUT    /api/players/{id}     → Update (idempotent)
DELETE /api/players/{id}     → Delete (idempotent)
```

**Why REST?**
- Standardized interface (developers know what to expect)
- Uses HTTP verbs correctly (GET for reads, POST for creates)
- Predictable URLs (resource-based, not action-based)
- Stateless requests (scalable)

### Status Codes

```csharp
200 OK              - Success
201 Created         - Resource created
204 No Content      - Success, no body (DELETE, PUT)
400 Bad Request     - Invalid input
401 Unauthorized    - Missing/invalid auth
404 Not Found       - Resource doesn't exist
500 Server Error    - Unexpected error (logs details)
```

---

## 9. Deployment & DevOps

### Docker (Containerization)

**Why Docker?**
- Same environment everywhere (dev, staging, production)
- Eliminates "works on my machine" problems
- Easy to scale horizontally
- Cloud-native

**Multi-stage Dockerfile:**
```dockerfile
# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
# ... compile code ...

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
# ... copy compiled binaries only ...
```

**Benefits:**
- Final image is small (no SDK, just runtime)
- Faster deployments
- Reduced attack surface

### Azure Deployment

**CI/CD Pipeline (azure-pipelines.yml):**
1. Code pushed to GitHub
2. Azure DevOps detects change
3. Builds the project
4. Runs tests
5. If successful, deploys to Staging
6. If on main branch, deploys to Production

**Infrastructure as Code (ARM template):**
- Define resources in JSON (App Service, database, etc.)
- Reproducible deployments
- Version control for infrastructure
- Easy to scale or replicate

---

## 10. Key Design Decisions & Trade-offs

### SQLite vs. SQL Server

**Chosen: SQLite for dev, SQL compatible template for prod**

| SQLite | SQL Server |
|--------|-----------|
| File-based, easy setup | Separate server, more overhead |
| Good for development | Better for production load |
| Limited concurrency | Handles thousands of connections |
| No admin needed | Requires DBA |

**Decision:** Development with SQLite (quick iteration), ARM template supports SQL Server upgrade for production.

### Async/Await Throughout

```csharp
public async Task<Player> GetByIdAsync(int id)
{
    return await _context.Players.FindAsync(id);
}
```

**Why?**
- Non-blocking - threads freed up for other requests
- Better scalability
- Handles I/O bound operations efficiently
- .NET 8 optimized for async

---

## 11. VIVA Talking Points - Expected Questions

### "Explain your architecture"

**Answer:** "I use a layered architecture with Controllers, Services, Repositories, and Data layers. Controllers handle HTTP requests but contain no business logic. Services contain business logic and coordinate workflows. Repositories abstract database access. This separation ensures each layer has one responsibility, making code testable. For example, I can test a service by mocking its repository, without needing a real database."

### "Why JWT over session-based authentication?"

**Answer:** "JWT tokens are stateless—the server doesn't store anything. Each token is self-contained with claims and a cryptographic signature. This is better for APIs because: (1) No session storage needed, (2) Scales across multiple servers easily, (3) Mobile and SPA apps prefer tokens. The token includes an expiry, ensuring automatic invalidation."

### "How do you prevent SQL injection?"

**Answer:** "Entity Framework Core uses parameterized queries internally. When I write LINQ queries, they're automatically parameterized—user input is never concatenated into SQL. Additionally, I validate all input using DataAnnotations before it reaches the database."

### "Why use repositories if EF Core is already an abstraction?"

**Answer:** "Repositories add another abstraction layer. Without them, services directly depend on DbContext. With repositories, I can inject mock repositories in tests. Also, if we needed to switch from Entity Framework to a different ORM, I'd only change repository implementations, not all services."

### "How do you handle errors?"

**Answer:** "I use global exception handling middleware that catches all unhandled exceptions and returns a consistent error response with status code and message. The middleware logs full details for debugging but only returns safe information to clients, preventing stack trace leakage."

### "Why DTOs instead of returning entities directly?"

**Answer:** "DTOs create an explicit API contract independent of database structure. If clients depend on entities directly, any refactoring breaks the API. DTOs also provide security—I only expose necessary properties, hiding internal fields or sensitive data."

### "Explain the JWT flow"

**Answer:** "User registers with credentials. Password is hashed and stored. On login, password is verified. If correct, a JWT token is generated containing claims and signed with a secret. Client includes this token in the Authorization header. On each request, middleware validates the token's signature and expiry. Invalid or expired tokens return 401."

### "Why Entity Framework migrations?"

**Answer:** "Migrations track database schema changes over time, version-controlled in C#. This enables team collaboration—everyone sees what the schema should be. Migrations apply consistently across all environments and can be rolled back if needed."

### "What would you improve?"

**Answer:** "Rate limiting on auth endpoints to prevent brute force, pagination for list endpoints, caching with Redis for frequently accessed data, audit logging to track changes, automated E2E tests, and API versioning to support multiple API versions simultaneously."

---

## 12. Grading Rubric Coverage

| Criterion | How Addressed |
|-----------|--------------|
| **Project Setup & Architecture (10)** | Layered architecture, DI in Program.cs, proper folder structure |
| **Database Design & Relationships (15)** | 6 entities with proper EF Core Fluent API configuration |
| **API Development (15)** | RESTful endpoints with proper status codes, DTOs |
| **Authentication & Authorization (20)** | JWT + ASP.NET Identity, secure password hashing, token validation |
| **Business Logic Layer (10)** | Services layer with separated logic, repositories abstraction |
| **Unit Testing & Code Quality (10)** | Xunit + Moq tests, SOLID principles |
| **Deployment & CI/CD (10)** | Docker, Azure DevOps pipeline, ARM infrastructure template |
| **API Documentation (5)** | Swagger/OpenAPI, comprehensive README |
| **Version Control & GitHub (5)** | Regular commits with clear messages |

---

## 13. Local Development Commands

```bash
# Restore and build
dotnet restore
dotnet build

# Database setup
dotnet ef database update

# Run the API
dotnet run

# Run tests
dotnet test

# Docker build
docker build -t football-league-api .

# Azure deployment
az deployment group create --resource-group football-league-rg \
  --template-file azure-deploy-template.json \
  --parameters webAppName=my-unique-app jwtKey=secretkey
```

---

## Final Notes

This project demonstrates:
- **Professional architecture** - Clean, testable, maintainable
- **Security focus** - HTTPS, password hashing, input validation, JWT
- **DevOps knowledge** - Docker, CI/CD, infrastructure as code
- **Best practices** - SOLID principles, proper naming, testing

Ready for production with clear path to scale.

