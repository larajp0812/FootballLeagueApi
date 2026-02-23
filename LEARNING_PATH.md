# Football League API - Learning Path & Tutorial

A structured guide to understand this fully-featured ASP.NET Core backend API. 
Complete phases 1-3 to understand the core architecture (layered + dependency injection), then explore specialized topics in phases 4-6.

**Estimated Time to Complete:** ~2.5 hours
**Best for:** Pre-viva exam study, code review, and future maintenance

---

## 🎯 PHASE 1: UNDERSTAND THE ARCHITECTURE (20 mins)

### Step 1: Start with the Overview
**File:** [README.md](README.md)
- Understand what the project does
- See the high-level features (Teams, Players, Matches, Seasons, Venues, Auth)

### Step 2: Understand Dependency Injection & Configuration
**File:** [Program.cs](Program.cs)
- Read the XML comments explaining each section
- This shows how all layers are wired together
- Notice how services and repositories are registered
- Understand: Controllers depend on Services, Services depend on Repositories

**Key concepts:**
```
Program.cs sets up:
├─ Database connection (DbContext: LeagueContext)
├─ Authentication (Identity)
├─ Dependency Injection (ISeasonService → SeasonService, etc.)
└─ API Middleware (CORS, Swagger, HTTPS)
```

---

## 🎯 PHASE 2: UNDERSTAND THE DATA MODEL (15 mins)

### Step 3: Read the Domain Models
Read these in order to see the data structure:

1. **[Models/Team.cs](Models/Team.cs)**
   - Core entity
   - Has relationships to Players, HomeMatches, AwayMatches
   - Notice the navigation properties (TeamId references in other tables)

2. **[Models/Player.cs](Models/Player.cs)**
   - Belongs to a Team (TeamId foreign key)
   - [JsonIgnore] on Team property = prevents circular serialization
   - Learn: Why we ignore navigation properties in JSON

3. **[Models/Season.cs](Models/Season.cs)**
   - Container for all matches in a time period
   - [JsonIgnore] on Matches = prevents massive nested data in responses

4. **[Models/Match.cs](Models/Match.cs)**
   - Links HomeTeam and AwayTeam (both are Teams)
   - Belongs to a Season and Venue
   - Has MatchEvents (goals, cards, etc.)
   - Complex relationships - pay attention to foreign keys

5. **[Models/Venue.cs](Models/Venue.cs)**
   - Simple: stadiums where matches are played
   - Can have many Matches

6. **[Models/MatchEvent.cs](Models/MatchEvent.cs)**
   - Represents events during a match (goals, cards)
   - Optional Player (some events don't involve a specific player)
   - Minute-level timing

### Step 4: Understand Database Configuration
**File:** [Data/LeagueContext.cs](Data/LeagueContext.cs)
- See how models map to database tables (DbSet<Team>, DbSet<Player>, etc.)
- Learn about the Fluent API configuration in OnModelCreating()
- Understand why HomeTeam and AwayTeam relationships have DeleteBehavior.Restrict
  - **Why?** You can't delete a team if it has scheduled matches

---

## 🎯 PHASE 3: UNDERSTAND THE FULL CRUD FLOW (40 mins)

### This is the game-changer phase. After this, you'll understand the entire architecture!

### Step 5: Follow a Complete CRUD Example - TEAMS

Read these files in order. Each builds on the previous:

#### 5a. **Controller** - Where HTTP requests enter
**File:** [Controllers/TeamsController.cs](Controllers/TeamsController.cs)
- Receives HTTP requests (GET, POST, PUT, DELETE)
- Calls the service to process requests
- Returns HTTP responses (200 OK, 201 Created, 204 No Content, 404 Not Found)
- All methods are async (non-blocking)

**Key endpoints:**
```
GET    /api/teams           → GetAll() → returns 200 OK
GET    /api/teams/{id}      → GetById() → returns 200 OK or 404
POST   /api/teams           → Create() → returns 201 Created
PUT    /api/teams/{id}      → Update() → returns 204 No Content
DELETE /api/teams/{id}      → Delete() → returns 204 No Content
```

#### 5b. **DTOs** - Data Transfer Objects
**Files:** 
- [DTOs/TeamCreateDto.cs](DTOs/TeamCreateDto.cs) - for POST requests
- [DTOs/TeamReadDto.cs](DTOs/TeamReadDto.cs) - for GET responses
- [DTOs/TeamUpdateDto.cs](DTOs/TeamUpdateDto.cs) - for PUT requests

**Why separate DTOs?**
- Security: Don't expose internal IDs unnecessarily
- Flexibility: API contract separate from database model
- Validation: Different validation rules for Create vs Update

#### 5c. **Mapper** - Convert between DTOs and Models
**File:** [DTOs/TeamMapper.cs](DTOs/TeamMapper.cs)
- `ToDto(team)` - Convert Team model → TeamReadDto (for API response)
- `ToModel(dto)` - Convert TeamCreateDto → Team model (for database)
- Keeps conversion logic centralized and reusable

#### 5d. **Service** - Business Logic Layer
**File:** [Services/ITeamService.cs](Services/ITeamService.cs) (interface)
**File:** [Services/TeamService.cs](Services/TeamService.cs) (implementation)

**What happens in the service:**
```
CreateAsync(team):
  1. Add team to repository (in-memory change)
  2. Call SaveChangesAsync() to persist to database
  3. Return the created team
  
UpdateAsync(id, team):
  1. Validate that URL id matches object TeamId
  2. Call repository to update
  3. Save changes and return success bool
  
DeleteAsync(id):
  1. Retrieve team by ID
  2. Delete it and save
  3. Return whether it succeeded
```

**Key insight:** Service coordinates repository calls and handles transactions

#### 5e. **Repository** - Data Access Layer
**File:** [Repositories/ITeamRepository.cs](Repositories/ITeamRepository.cs) (interface)
**File:** [Repositories/TeamRepository.cs](Repositories/TeamRepository.cs) (implementation)

**What the repository does:**
```
GetAllAsync()     → _context.Teams.ToListAsync()
GetByIdAsync(id)  → _context.Teams.FindAsync(id)
AddAsync(team)    → _context.Teams.AddAsync(team)
UpdateAsync(team) → _context.Teams.Update(team)
DeleteAsync(team) → _context.Teams.Remove(team)
SaveChangesAsync()→ _context.SaveChangesAsync() > 0
```

**Key insight:** Repository is just CRUD wrapper around EF Core. It doesn't have business logic.

### Step 6: Understand the Full Flow

**Request Flow (e.g., POST /api/teams):**
```
1. Client sends JSON: { "Name": "Manchester", "Coach": "Ten Hag", "FoundedYear": 1878 }

2. Controller.Create(TeamCreateDto dto)
   └─ Parses JSON to TeamCreateDto

3. Service.CreateAsync(Team team)
   └─ Validates business rules
   └─ Calls repository.AddAsync(team)
   └─ Calls repository.SaveChangesAsync()

4. Repository.SaveChangesAsync()
   └─ Sends INSERT statement to database
   └─ Database generates TeamId = 5 (auto-increment)
   └─ Returns true

5. Service returns created Team with TeamId = 5

6. Controller converts Team → TeamReadDto
   └─ { "TeamId": 5, "Name": "Manchester", ... }

7. Controller returns 201 Created with Location header
   └─ HTTP/1.1 201 Created
   └─ Location: /api/teams/5
   └─ Body: { "TeamId": 5, "Name": "Manchester", ... }
```

---

## 🎯 PHASE 4: UNDERSTAND OTHER ENTITIES (20 mins)

Now that you understand Teams, the other entities follow the EXACT SAME PATTERN.

### Step 7: Quickly Review Other Controllers
Read these in this order (they all follow the Teams pattern):

1. **[Controllers/PlayersController.cs](Controllers/PlayersController.cs)**
   - Same pattern as Teams
   - Players belong to Teams (TeamId foreign key)

2. **[Controllers/SeasonsController.cs](Controllers/SeasonsController.cs)**
   - Same pattern
   - Seasons contain Matches

3. **[Controllers/MatchesController.cs](Controllers/MatchesController.cs)**
   - Same pattern but more complex
   - Homestead validation (home team ≠ away team)
   - Notice the try/catch for business logic errors

4. **[Controllers/VenuesController.cs](Controllers/VenuesController.cs)**
   - Same pattern
   - Simple entity

5. **[Controllers/MatchEventController.cs](Controllers/MatchEventController.cs)**
   - Same pattern
   - Events within matches (goals, cards, etc.)

**Time-saving tip:** Don't read the full service/repository for each. They all follow the same pattern as Teams.

---

## 🎯 PHASE 4: UNDERSTAND AUTHENTICATION & JWT (15 mins)

### Step 8: User Registration & JWT Login

Authentication in this project uses **two-part security**:

#### Part 1: User Registration with ASP.NET Identity
**File:** [Controllers/AuthController.cs](Controllers/AuthController.cs) - Register endpoint

```csharp
// POST /api/auth/register
// Body: { "UserName": "john", "Email": "john@example.com", "Password": "SecurePass123!" }
```

**What happens:**
1. UserManager (from ASP.NET Core Identity) creates a new IdentityUser
2. Password is automatically hashed using PBKDF2 with salt (cryptographically secure)
3. User account saved to database table `AspNetUsers`
4. Password is never stored in plain text

**Related DTOs:**
- [DTOs/Auth/RegisterDto.cs](DTOs/Auth/RegisterDto.cs) - Validation: email format, password complexity (uppercase, lowercase, digit, symbol)
- Input validation prevents weak passwords

#### Part 2: JWT Token-Based Login
**File:** [Controllers/AuthController.cs](Controllers/AuthController.cs) - Login endpoint

```csharp
// POST /api/auth/login
// Body: { "Email": "john@example.com", "Password": "SecurePass123!" }
// Response: { "token": "eyJhbGc...", "expires": "2026-02-24T12:00:00Z" }
```

**What happens:**
1. Find user by email using UserManager
2. Verify password (compare hash, not plaintext)
3. If valid, create JWT token:
   - **Header:** Algorithm (HS256) + token type
   - **Payload:** Claims (Subject = user ID, Email, Jti = unique token ID), Expiry = 60 minutes
   - **Signature:** Signed with secret key, prevents tampering
4. Return token to client
5. Client stores token (localStorage, sessionStorage, or cookie)

**JWT Flow Diagram:**
```
Client                          API Server
  │                              │
  ├─ POST /api/auth/login ──────>│
  │  { email, password }         │ Verify password
  │                              │ Generate JWT
  │<───── 200 OK ────────────────┤
  │  { token, expires }          │
  │                              │
  ├─ GET /api/teams ────────────>│
  │  Header: Authorization: Bearer eyJhbGc...
  │                              │ Validate token
  │                              │ Check signature + expiry
  │<───── 200 OK ────────────────┤
  │  [Array of teams]            │
```

#### Configuration
**File:** [appsettings.json](appsettings.json) - `Jwt` section

```json
{
  "Jwt": {
    "Key": "your-secret-key-min-32-chars-long-for-security",
    "Issuer": "FootballLeagueApi",
    "Audience": "FootballLeagueApiUsers",
    "ExpiresMinutes": 60
  }
}
```

**File:** [Program.cs](Program.cs) - Search for `AddAuthentication` and `JwtBearerDefaults`

JWT is configured in Program.cs with:
- Token validation: Checks issuer, audience, signature, and expiry
- All endpoints require `[Authorize]` attribute for protection
- Unauthenticated requests receive 401 Unauthorized

**Key insight:** JWT tokens are stateless (no session database needed) and can be passed to mobile/SPA clients.

---

## 🎯 PHASE 5: UNDERSTAND TESTING (15 mins)

### Step 9: Unit Testing with xUnit + Moq

Test files validate business logic in isolation. 

**File:** [Tests/Controllers/PlayersControllerTests.cs](Tests/Controllers/PlayersControllerTests.cs)

**What testing pattern is used:**
1. **Mock the Service** - Create a fake IPlayerService
2. **Test the Controller** - Call controller methods with the mock
3. **Assert the Result** - Verify correct response

**Example test:**
```csharp
[Fact]
public async Task GetById_WithValidId_ReturnsOkResult()
{
    // Arrange (setup)
    var playerId = 1;
    var mockService = new Mock<IPlayerService>();
    mockService.Setup(s => s.GetByIdAsync(playerId))
        .ReturnsAsync(new Player { PlayerId = 1, FullName = "John Doe" });
    var controller = new PlayersController(mockService.Object, logger);

    // Act (execute)
    var result = await controller.GetById(playerId);

    // Assert (verify)
    var okResult = Assert.IsType<OkObjectResult>(result);
    Assert.NotNull(okResult.Value);
}
```

**Why test this way?**
- **Unit isolated:** Only testing controller, not database
- **Fast:** Mocked service returns instantly
- **Reliable:** Doesn't depend on database state
- **Clear:** Arrange-Act-Assert pattern is readable

**Run tests:**
```bash
dotnet test
```

**Key insight:** Testing should validate behavior, not implementation details.

---

## 🎯 PHASE 6: UNDERSTAND DEPLOYMENT (20 mins)

### Step 10: Containerization with Docker

Docker packages your app + dependencies into a container so it runs the same everywhere.

**File:** [Dockerfile](Dockerfile) - Container build instructions

**Build stages:**
1. **Stage 1 - Build:** Uses `dotnet sdk:8.0` to compile C# code
2. **Stage 2 - Runtime:** Uses `dotnet aspnet:8.0` (smaller, no SDK)
3. **Result:** ~200 MB image (much smaller than docker with full SDK)

**Key settings:**
- Port: 8080 (HTTP)
- Environment: Production
- No debugging symbols (release mode)

**Build and run locally:**
```bash
docker build -t footballapi:latest .
docker run -p 8080:8080 footballapi:latest
```

### Step 11: Azure Cloud Deployment

**Files:**
- [azure-deploy-template.json](azure-deploy-template.json) - Infrastructure as Code (ARM template)
- [azure-pipelines.yml](azure-pipelines.yml) - CI/CD pipeline definition
- [AZURE_DEPLOYMENT.md](AZURE_DEPLOYMENT.md) - Step-by-step deployment guide

**Deployment flow:**
```
1. Push code to GitHub
   ↓
2. Azure Pipelines triggered automatically
   ├─ dotnet build  (compile)
   ├─ dotnet test   (run tests)
   └─ docker build & push  (upload image to Azure Container Registry)
   ↓
3. Container deployed to Azure App Service
   ├─ Staging slot (automatic)
   └─ Production slot (on main branch only)
   ↓
4. API accessible at: https://footballapi-prod.azurewebsites.net
```

**ARM Template provisions:**
- App Service Plan (compute + OS)
- Web App (running container)
- Configuration:
  - JWT key, issuer, audience
  - Database connection string
  - Environment variables

**Key insight:** Infrastructure as Code means your server setup is versioned alongside your code.

### Step 12: GitHub Source Control

**Read:** [README.md](README.md) - Version Control section

Git tracks all code changes with clear commit messages. Recommended commit structure:

```bash
# Phase 1: Setup
git commit -m "feat: add project structure and dependencies"

# Phase 2: Models
git commit -m "feat: add Team, Player, Season models"

# Phase 3: API Endpoints  
git commit -m "feat: add Teams and Players CRUD endpoints"
git commit -m "feat: add database migrations"

# Phase 4: Authentication
git commit -m "feat: add JWT authentication"
git commit -m "feat: add user registration endpoint"

# Phase 5: Testing
git commit -m "test: add unit tests for PlayersController"

# Phase 6: Deployment
git commit -m "build: add Dockerfile for containerization"
git commit -m "ci: add Azure DevOps pipeline"

# Fixes
git commit -m "fix: correct async method signatures in repositories"

# Docs
git commit -m "docs: update LEARNING_PATH and VIVA_NOTES"

git push origin main
```

---

## 🎯 PHASE 7: VIVA EXAM PREPARATION (10 mins)

### Step 13: Know Your Architecture for the Viva

**File:** [VIVA_NOTES.md](VIVA_NOTES.md) - Complete viva preparation guide with 10 expected Q&A pairs

Your viva examiner will ask about design decisions. Be ready to explain:

**Expected Questions:**

1. **Architecture Decision:** "Why did you use a layered architecture (Controllers → Services → Repositories)?"
   - *Answer:* Separation of concerns, testability (mock services), scalability, maintainability

2. **Authentication:** "How does JWT work in your API?"
   - *Answer:* User logs in with credentials, receives token with signed claims, token validates stateless requests

3. **Database Design:** "Why does Match have DeleteBehavior.Restrict for Team references?"
   - *Answer:* Prevents deleting a team if it has scheduled matches, maintains referential integrity

4. **DTOs:** "Why do you use separate Create/Read/Update DTOs instead of one Model DTO?"
   - *Answer:* API contracts decouple from database; different operations need different data shapes

5. **Testing:** "How are your unit tests isolated from the database?"
   - *Answer:* Mock the IPlayerService, test controller logic independently, no database dependencies

6. **Async/Await:** "Why are all methods async?"
   - *Answer:* Database I/O is slow; async frees threads for other requests; massive performance under load

7. **Deployment:** "How does your Dockerfile ensure consistency from dev to production?"
   - *Answer:* Multi-stage build: compile in SDK, run in smaller runtime image; same container everywhere

8. **Error Handling:** "How do you handle exceptions across the API?"
   - *Answer:* GlobalExceptionHandlingMiddleware catches all exceptions, logs details, returns consistent error response

9. **API Design:** "Why is your API RESTful (GET, POST, PUT, DELETE)?"
   - *Answer:* Standard convention; intuitive for clients; scales well; clear HTTP semantics

10. **Git Workflow:** "How do you track changes and collaborate?"
    - *Answer:* Semantic commit messages by feature; version control of code, migrations, and infrastructure

**See [VIVA_NOTES.md](VIVA_NOTES.md) for:**
- Complete 10 Q&A pairs with detailed answers
- Rubric coverage checklist (marks: architecture, database, API, auth, logic, testing, deployment, docs, git)
- Design trade-offs explained
- Common pitfalls to avoid

---

## 🎯 PHASE 8: DATABASE MIGRATIONS (OPTIONAL Deep Dive - 10 mins)

### Step 14: Understanding Schema Evolution

Database schema changes are tracked with migrations:

**Folder:** [Migrations/](Migrations/)

1. **[Migrations/20260217112920_InitialCreate.cs](Migrations/20260217112920_InitialCreate.cs)**
   - Creates initial schema (Teams, Players, Matches, etc.)
   - Sets up columns, data types, relationships

2. **[Migrations/20260222170754_TeamUpdated.cs](Migrations/20260222170754_TeamUpdated.cs)**
   - Subsequent changes to schema
   - Example: Adding Coach column, changing data type, etc.

**How migrations work:**
```csharp
public override void Up(MigrationBuilder migrationBuilder)
{
    // Create table with columns, foreign keys, indexes
    migrationBuilder.CreateTable(name: "Teams",
        columns: table => new
        {
            TeamId = table.Column<int>(),
            Name = table.Column<string>(),
            Coach = table.Column<string>(),
            // ... etc
        });
}

public override void Down(MigrationBuilder migrationBuilder)
{
    // Rollback - drop table
    migrationBuilder.DropTable(name: "Teams");
}
```

**Why this matters:**
- Schema versioning like git for databases
- Can rollback to previous version if needed
- Other developers see exact schema evolution
- Running `dotnet ef database update` applies all pending migrations

**Apply migrations:**
```bash
dotnet ef migrations add AddNewColumn  # Create new migration
dotnet ef database update              # Apply all pending migrations
```

**Key insight:** Migrations with code guarantee reproducible schema across all environments.

---

## 🎯 VISUAL ARCHITECTURE OVERVIEW

```
┌─────────────────────────────────────┐
│           HTTP CLIENTS              │
│    (Postman, Browser, Mobile)       │
└──────────────┬──────────────────────┘
               │
               ↓ HTTP requests (with JWT Token)
┌─────────────────────────────────────┐
│      MIDDLEWARE / AUTHENTICATION    │
│  - JWT Bearer validation            │
│  - Exception handling               │
│  - CORS, Logging                    │
└──────────────┬──────────────────────┘
               │
               ↓
┌─────────────────────────────────────┐
│         CONTROLLERS                 │
│  (TeamsController.cs, etc.)         │
│  - Parse HTTP and DTOs              │
│  - ModelState validation            │
│  - Call Services                    │
│  - Return HTTP responses            │
└────────────┬────────────────────────┘
             │
             ↓ Call business logic
┌──────────────────────────────────────┐
│          SERVICES                    │  ← Business Logic
│  (TeamService.cs, etc.)              │
│  - Validate business rules           │
│  - Coordinate repository calls       │
│  - Handle transactions               │
└────────────┬─────────────────────────┘
             │
             ↓ CRUD operations
┌──────────────────────────────────────┐
│        REPOSITORIES                  │  ← Data Layer
│  (TeamRepository.cs, etc.)           │
│  - AddAsync(), UpdateAsync(), etc.   │
│  - SaveChangesAsync() to database    │
└────────────┬─────────────────────────┘
             │
             ↓ Entity Framework Core
┌──────────────────────────────────────┐
│       ENTITYFRAMEWORK CORE            │
│  (LeagueContext.cs)                  │
│  - Maps models to database tables    │
│  - Generates SQL from LINQ            │
│  - Manages DbContext (unit of work)  │
└────────────┬─────────────────────────┘
             │
             ↓ SQL queries / Migrations
┌──────────────────────────────────────┐
│      DATABASE (SQLite / SQL Server)  │
│  Teams, Players, Matches, Seasons... │
│  AspNetUsers, AspNetRoles (Identity) │
└──────────────────────────────────────┘
```

---

## 🎯 QUICK REFERENCE: THE LAYERS

### Layer 1: PRESENTATION (Controllers + DTOs)
- **What:** Handle HTTP requests/responses, validate input
- **Where:** Controllers/ and DTOs/ folders
- **File pattern:** *Controller.cs, *Dto.cs, *Mapper.cs
- **Depends on:** Services
- **Example:** TeamsController → TeamCreateDto → TeamMapper → ITeamService

### Layer 2: BUSINESS LOGIC (Services)
- **What:** Business rules, validation, orchestration, transactions
- **Where:** Services/ folder
- **File pattern:** I*Service.cs (interface) and *Service.cs (implementation)
- **Depends on:** Repositories
- **Example:** TeamService implements ITeamService

### Layer 3: DATA ACCESS (Repositories)
- **What:** Database CRUD operations, abstract EF Core
- **Where:** Repositories/ folder
- **File pattern:** I*Repository.cs (interface) and *Repository.cs (implementation)
- **Depends on:** Entity Framework Core, LeagueContext
- **Example:** TeamRepository implements ITeamRepository

### Layer 4: DATA (EF Core + Database)
- **What:** Object-relational mapping, schema definition
- **Where:** Data/ folder (LeagueContext.cs)
- **Depends on:** SQLite/SQL Server
- **Example:** LeagueContext inherits IdentityDbContext, defines all DbSets and relationships
```

---

## 📚 RECOMMENDED READING ORDER SUMMARY

### Phase 1-3 (Understand Core Architecture - 1 hour)
```
1️⃣  README.md                          (What the project does)
2️⃣  Program.cs                         (Application setup + DI)
3️⃣  Models/*.cs                        (All 6 domain entities)
4️⃣  Data/LeagueContext.cs              (Database configuration)
5️⃣  Controllers/TeamsController.cs      (HTTP layer)
6️⃣  DTOs/Team*.cs                      (Data contracts)
7️⃣  DTOs/TeamMapper.cs                 (DTO conversion)
8️⃣  Services/ITeamService.cs           (Service interface)
9️⃣  Services/TeamService.cs            (Business logic)
🔟 Repositories/ITeamRepository.cs     (Repository interface)
1️⃣1️⃣ Repositories/TeamRepository.cs     (Data access)
1️⃣2️⃣ Controllers/PlayersController.cs   (Same pattern as Teams)
1️⃣3️⃣ Tests/Controllers/PlayersControllerTests.cs (Unit test example)
```

### Phase 4-6 (Specialized Features - 1 hour)
```
1️⃣4️⃣ Controllers/AuthController.cs      (Authentication + JWT)
1️⃣5️⃣ DTOs/Auth/*.cs                     (Auth data contracts)
1️⃣6️⃣ appsettings.json                   (JWT configuration)
1️⃣7️⃣ Dockerfile                         (Containerization)
1️⃣8️⃣ azure-pipelines.yml                (CI/CD pipeline)
1️⃣9️⃣ azure-deploy-template.json         (Infrastructure as Code)
2️⃣0️⃣ AZURE_DEPLOYMENT.md                (Deployment guide)
```

### Phase 7-8 (Viva Prep & Advanced - 30 mins)
```
2️⃣1️⃣ VIVA_NOTES.md                      (Exam preparation)
2️⃣2️⃣ Migrations/                        (Schema evolution)
2️⃣3️⃣ Middleware/GlobalExceptionHandlingMiddleware.cs (Cross-cutting concerns)
```

---

## 💡 TIPS FOR UNDERSTANDING THE CODE

### Tip 1: Follow One Request (Full Lifecycle)
Pick a simple operation like "Create a Team" and trace it through all 4 layers:

```
CLIENT REQUEST:
POST /api/teams
Authorization: Bearer eyJhbGc...  ← JWT token
{
  "Name": "Manchester",
  "Coach": "Ten Hag",
  "FoundedYear": 1878
}

↓ MIDDLEWARE
Validates JWT signature, issuer, expiry
Checks authorization (user is authenticated)

↓ CONTROLLER (TeamsController.Create)
Receives HTTP request
Parses JSON to TeamCreateDto
Validates ModelState
LogInformation("Creating team...")
Calls service.CreateAsync(team)

↓ SERVICE (TeamService.CreateAsync)
Performs business logic:
  - Validate team name is not empty
  - Validate FoundedYear is reasonable
Calls repository.AddAsync(team)
Calls repository.SaveChangesAsync()
Returns created team object

↓ REPOSITORY (TeamRepository)
Adds to EF Core DbContext
SaveChangesAsync() generates SQL INSERT

↓ DATABASE
Inserts record into Teams table
Auto-generates TeamId = 5
Commits transaction

↓ RESPONSE
{ "TeamId": 5, "Name": "Manchester", "Coach": "Ten Hag", "FoundedYear": 1878 }
HTTP/1.1 201 Created
Location: /api/teams/5
```

### Tip 2: Understand Async/Await
All methods are `async Task<...>` (non-blocking I/O):

**Why?** Database calls are slow (~100ms per query)
- Without async: Thread waits, blocks other requests
- With async: Thread freed, handles other requests
- Result: 200 concurrent requests on 20 threads instead of 200 threads

```csharp
public async Task<Team> CreateAsync(Team team)
{
    await repository.AddAsync(team);      // Execute, don't wait
    return await repository.SaveChangesAsync();  // Wait for DB commit
}
```

### Tip 3: Why Interfaces?
All services and repositories have interfaces:

```csharp
ITeamService service = new TeamService(repository);  // Easy to swap
Mock<ITeamService> mockService = new Mock<ITeamService>();  // Easy to mock

// Without interface:
TeamService service = new TeamService(repository);  // Tightly coupled
// Can't mock, can't swap implementation
```

**Benefits:**
- **Testability:** Mock in unit tests
- **Flexibility:** Swap implementations (SQL → Redis cache)
- **Loose coupling:** Controller depends on contract, not implementation

### Tip 4: [JsonIgnore] Attribute (Prevent Circular References)
Relationships create circular references. Example: Team has Players, Player belongs to Team.

```csharp
public class Player
{
    public int PlayerId { get; set; }
    public int TeamId { get; set; }
    
    [JsonIgnore]  // NO SERIALIZE - prevent infinite loop
    public Team? Team { get; set; }
}

// Without [JsonIgnore] (ERROR):
GET /api/players/1
{
  "PlayerId": 1,
  "TeamId": 5,
  "Team": {
    "TeamId": 5,
    "Players": [
      {
        "PlayerId": 1,
        "Team": {
          "Players": [ ... ]  // ← INFINITE LOOP
        }
      }
    ]
  }
}

// With [JsonIgnore] (CORRECT):
GET /api/players/1
{
  "PlayerId": 1,
  "TeamId": 5
}
// Client knows Team ID = 5, can fetch /api/teams/5 separately
```

### Tip 5: Why Multiple DTOs?
Different operations need different data contracts:

```csharp
// CREATE request: No ID (auto-generated), no computed fields
POST /api/teams
{
  "Name": "Manchester",  ← Required
  "Coach": "Ten Hag",     ← Required
  "FoundedYear": 1878     ← Required
}

// RESPONSE: Include ID and read-only fields
{
  "TeamId": 5,           ← Generated by server, not in request
  "Name": "Manchester",
  "Coach": "Ten Hag",
  "FoundedYear": 1878,
  "PlayerCount": 11      ← Computed field, not in create DTO
}

// UPDATE request: ID in URL path, not in body
PUT /api/teams/5
{
  "Name": "Man City",    ← Updated
  "Coach": "Guardiola",  ← Updated
  "FoundedYear": 1878    ← Unchanged
}
// Don't include "TeamId" - it's in the URL path!
```

**DTO naming convention:**
- `*CreateDto` → POST request body
- `*ReadDto` → GET response body
- `*UpdateDto` → PUT request body
- `*` Model → Database entity (private)

### Tip 6: Understand Dependency Injection (DI)
Services depend on repositories, controllers depend on services. DI container wires them together:

```csharp
// Program.cs (startup configuration)
builder.Services.AddScoped<ITeamRepository, TeamRepository>();
builder.Services.AddScoped<ITeamService, TeamService>();

// Result:
// TeamsController(ITeamService service) <- DI container injects
// TeamService(ITeamRepository repo) <- DI container injects
// TeamRepository(LeagueContext context) <- DI container injects
```

**Scopes:**
- `AddTransient` - New instance every time (stateless)
- `AddScoped` - One instance per HTTP request (recommended for services/repos)
- `AddSingleton` - One instance for lifetime of app (config, cache)

### Tip 7: JWT Tokens Contain User Information
JWT is Base64-encoded JSON with three parts:

```
Header.Payload.Signature

Example:
eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.
eyJzdWIiOiIxMjM0NTY3ODkwIiwiZW1haWwiOiJqb2huQGV4YW1wbGUuY29tIiwiaWF0IjoxNTE2MjM5MDIyfQ.
SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c

Decoded Payload:
{
  "sub": "1234567890",           // Subject (user ID)
  "email": "john@example.com",   // User email
  "exp": 1516239022,             // Expires (timestamp)
  "iat": 1516239022              // Issued at (timestamp)
}

Signature validates that:
- Token wasn't modified
- Token came from server (secret key only on server)
```

Server validates token on every protected request (no database lookup needed!)

### Tip 8: Exception Handling is Centralized
Instead of try/catch in every controller:

```csharp
// GlobalExceptionHandlingMiddleware catches ALL exceptions
try
{
    await next(context);
}
catch (Exception ex)
{
    logger.LogError(ex, "Unexpected error");
    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
    await context.Response.WriteAsJsonAsync(new ErrorResponse
    {
        StatusCode = 500,
        Message = "An error occurred",
        Details = ex.Message,  // Only in Development!
        Timestamp = DateTime.UtcNow
    });
}
```

**Benefit:** Consistent error format across entire API

---

## 🎯 COMMON QUESTIONS ANSWERED

### "How does a request get routed to the right controller?"
1. HTTP request arrives at `/api/teams`
2. ASP.NET Core middleware processes it
3. Routing finds `TeamsController` with `[Route("api/[controller]")]`
4. Matches HTTP method (POST) to `[HttpPost]` method
5. Parameter binding converts JSON body to `TeamCreateDto
6. Method executes, returns response

### "Why do services exist if repositories can do CRUD?"
- **Repository:** Pure data access (Add, Update, Delete, Get from DB)
- **Service:** Business logic (validation, error handling, orchestration, transactions)
- **Separation:** Testable, reusable, maintainable
- Without services, business logic becomes scattered in controllers

**Example:** Create a match validation
```csharp
// Bad: Business logic in Controller
[HttpPost]
public async Task<IActionResult> Create(MatchCreateDto dto)
{
    if (dto.HomeTeamId == dto.AwayTeamId)  // ← Business logic!
        return BadRequest("Home and away teams must be different");
    var match = _mapper.ToModel(dto);
    await _repository.AddAsync(match);
    return Ok();
}

// Good: Business logic in Service
[HttpPost]
public async Task<IActionResult> Create(MatchCreateDto dto)
{
    var match = await _service.CreateAsync(dto);  // ← Service handles validation
    return Ok(match);
}

// MatchService.CreateAsync
public async Task<Match> CreateAsync(MatchCreateDto dto)
{
    if (dto.HomeTeamId == dto.AwayTeamId)  // ← Business logic centralized
        throw new ArgumentException("Teams must be different");
    var match = _mapper.ToModel(dto);
    await _repository.AddAsync(match);
    await _repository.SaveChangesAsync();
    return match;
}
```

### "Where does SaveChangesAsync get called?"
In the **service**, not the repository. Service owns the transaction:

```csharp
public async Task<Match> CreateAsync(Match match)
{
    await _repository.AddAsync(match);      // Stages in EF Core memory
    await _repository.SaveChangesAsync();    // COMMITS to database
    return match;
}
```

**Why?** Service might need multiple repository calls in one transaction:
```csharp
public async Task<Match> CreateWithEventsAsync(MatchCreateDto dto, List<EventDto> events)
{
    var match = _mapper.ToModel(dto);
    await _matchRepository.AddAsync(match);    // Not saved yet!
    
    foreach (var eventDto in events)
    {
        eventDto.MatchId = match.MatchId;
        var matchEvent = _eventMapper.ToModel(eventDto);
        await _eventRepository.AddAsync(matchEvent);  // Not saved yet!
    }
    
    await _repository.SaveChangesAsync();  // All changes commit together
    // If error occurs, all changes rollback automatically
}
```

### "How does EF Core know the database relationships?"
Three ways EF knows how to map:

1. **Foreign Key Properties**
   ```csharp
   public class Player
   {
       public int TeamId { get; set; }  // ← Name convention: EntityId
   }
   // EF assumes TeamId is foreign key to Teams.TeamId
   ```

2. **Navigation Properties**
   ```csharp
   public class Player
   {
       public Team? Team { get; set; }  // ← Navigation property
   }
   // EF can load related Team when accessing player.Team
   ```

3. **Fluent API Configuration**
   ```csharp
   // Data/LeagueContext.cs
   modelBuilder.Entity<Match>()
       .HasOne(m => m.HomeTeam)
       .WithMany(t => t.HomeMatches)
       .OnDelete(DeleteBehavior.Restrict);  // ← Can't delete team with matches
   ```

### "What does [Authorize] do on a controller?"
Requires JWT bearer token to be present and valid:

```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize]  // ← ALL endpoints in this controller require JWT
public class TeamsController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()  // ← Requires JWT
    {
        // ...
    }
    
    [AllowAnonymous]  // ← Exception: allows without JWT
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto model)
    {
        // ...
    }
}
```

**How it works:**
```
Request → Middleware checks Authorization header
          Authorization: Bearer eyJhbGc...
          │
          ├─ Valid token → Allow request to continue
          └─ Invalid/missing token → Return 401 Unauthorized
```

### "When should I use appsettings.Development vs appsettings.json?"
- **appsettings.json** - Shared defaults for all environments
- **appsettings.Development.json** - Override for development only
  - Detailed logging (Debug level)
  - Relaxed CORS
  - Local database connection

```json
// appsettings.json (all environments)
{
  "Jwt": {
    "Key": "your-secret-key-min-32-chars",
    "Issuer": "FootballLeagueApi",
    "Audience": "FootballLeagueApiUsers"
  }
}

// appsettings.Development.json (development override)
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug"  // More verbose in dev
    }
  }
}
```

Program.cs loads them:
```csharp
builder.Configuration
    .AddJsonFile("appsettings.json")  // Base config
    .AddJsonFile($"appsettings.{environment}.json");  // Env override
```

---

## 📚 NEXT STEPS FOR DEEPER LEARNING

1. **Modify a service** - Add new validation to TeamService.CreateAsync()
2. **Add a property** - Add "Website" to Team, create migration
3. **Write unit tests** - Mock ITeamRepository and test TeamService
4. **Create new entity** - Add "Coach" entity with relationship to Team
5. **Learn EF Core docs** - https://learn.microsoft.com/ef/core/
6. **Watch JWT tutorials** - Understand token structure deeper
7. **Deploy to Azure** - Run azure-pipelines.yml and watch logs
8. **Performance tune** - Add database indexes, caching with Redis

---

## 🎓 YOU'RE NOW READY FOR YOUR VIVA!

You understand:

✅ **Architecture** - Layered design with separation of concerns
✅ **Data Flow** - Request → Controller → Service → Repository → Database
✅ **Authentication** - JWT tokens, claims-based authorization
✅ **Database** - EF Core, migrations, relationships, constraints
✅ **Testing** - Unit tests with mocking
✅ **Deployment** - Docker containers, Azure App Service, CI/CD pipelines
✅ **Best Practices** - Async/await, dependency injection, interfaces
✅ **Error Handling** - Centralized middleware, consistent responses
✅ **API Design** - RESTful endpoints, DTOs, status codes
✅ **Git Workflow** - Semantic commits, version control

---

## 💬 VIVA TIPS

**Before Viva:**
- Read VIVA_NOTES.md (10 common questions + answers)
- Trace one complete request through all layers
- Understand why each design decision was made
- Practice explaining concepts without looking at code

**During Viva:**
- Listen carefully to the question
- Take 5 seconds to organize your thoughts
- Use the architecture diagram to explain flow
- Be honest if you don't know something ("I'd need to investigate")
- Show enthusiasm about the project

**Common Viva Questions Covered:**
1. Architecture and layering
2. Database design and migrations
3. Authentication and authorization
4. DTOs and model mapping
5. Testing strategy and mocking
6. Deployment and DevOps
7. Error handling
8. Performance considerations
9. Security best practices
10. Design trade-offs

See **[VIVA_NOTES.md](VIVA_NOTES.md)** for complete answers to all 10 questions.
