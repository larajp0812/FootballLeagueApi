# Football League API - Learning Path & Tutorial

A structured guide to understand the project by reading files in the optimal order.

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

## 🎯 PHASE 5: UNDERSTAND AUTHENTICATION (10 mins)

### Step 8: Authentication
**File:** [Controllers/AuthController.cs](Controllers/AuthController.cs)

**What does it do:**
```
POST /api/auth/register
- Accepts: { "UserName": "john", "Email": "john@example.com", "Password": "..." }
- Uses UserManager (from ASP.NET Identity)
- Identity automatically hashes password
- Creates user account in the database
```

**Related DTOs:**
- [DTOs/Auth/RegisterDto.cs](DTOs/Auth/RegisterDto.cs)
- [DTOs/Auth/LoginDto.cs](DTOs/Auth/LoginDto.cs) - Not yet implemented

**Key insight:** Auth is handled by ASP.NET Core Identity, which is different from business logic services.

---

## 🎯 PHASE 6: UNDERSTAND MIGRATIONS (Optional, but important)

### Step 9: Database Schema Evolution
**Folder:** [Migrations/](Migrations/)

Read the migration files to see how the database schema has evolved:

1. **[Migrations/20260217112920_InitialCreate.cs](Migrations/20260217112920_InitialCreate.cs)**
   - Creates all tables for the first time
   - Sets up columns, data types, and relationships
   - Migration names: timestamp + description
   - "Up()" method creates tables
   - "Down()" method drops tables (for rollback)

2. **[Migrations/20260222170754_TeamUpdated.cs](Migrations/20260222170754_TeamUpdated.cs)**
   - Later change to the schema
   - Shows how the database evolved after initial creation
   - Example: Maybe added Coach field to Team

**Key insight:** Migrations let you version control your database schema alongside your code.

---

## 🎯 VISUAL ARCHITECTURE OVERVIEW

```
┌─────────────────────────────────────┐
│           HTTP CLIENTS              │
│    (Postman, Browser, Mobile)       │
└──────────────┬──────────────────────┘
               │
               ↓ HTTP requests
┌─────────────────────────────────────┐
│         CONTROLLERS                 │
│  (TeamsController.cs, etc.)         │
│  - Parse HTTP and DTOs              │
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
│  - Maps models to database           │
│  - Generates SQL                     │
└────────────┬─────────────────────────┘
             │
             ↓ SQL queries
┌──────────────────────────────────────┐
│        DATABASE (SQLite)             │
│  Teams, Players, Matches, Seasons... │
└──────────────────────────────────────┘
```

---

## 🎯 QUICK REFERENCE: THE THREE LAYERS

### Layer 1: PRESENTATION (Controllers)
- **What:** Handle HTTP requests/responses
- **Where:** Controllers/ folder
- **File pattern:** *Controller.cs
- **Depends on:** Services
- **Example:** TeamsController.cs

### Layer 2: BUSINESS LOGIC (Services)
- **What:** Business rules, data validation, orchestration
- **Where:** Services/ folder
- **File pattern:** I*Service.cs (interface) and *Service.cs (implementation)
- **Depends on:** Repositories
- **Example:** TeamService.cs

### Layer 3: DATA ACCESS (Repositories + EF Core)
- **What:** Direct database operations
- **Where:** Repositories/ folder
- **File pattern:** I*Repository.cs (interface) and *Repository.cs (implementation)
- **Depends on:** Entity Framework Core, LeagueContext
- **Example:** TeamRepository.cs

---

## 🎯 RECOMMENDED READING ORDER SUMMARY

```
1️⃣  README.md                         (What the project does)
2️⃣  Program.cs                        (Application setup)
3️⃣  Models/*.cs                       (All model files)
4️⃣  Data/LeagueContext.cs             (Database configuration)
5️⃣  Controllers/TeamsController.cs     (HTTP layer)
6️⃣  DTOs/Team*.cs                     (Data contracts)
7️⃣  DTOs/TeamMapper.cs                (DTO conversion)
8️⃣  Services/ITeamService.cs          (Service interface)
9️⃣  Services/TeamService.cs           (Business logic)
🔟 Repositories/ITeamRepository.cs    (Repository interface)
1️⃣1️⃣ Repositories/TeamRepository.cs    (Data access)
1️⃣2️⃣ Controllers/PlayersController.cs  (Another entity - same pattern)
1️⃣3️⃣ Controllers/AuthController.cs     (Authentication)
1️⃣4️⃣ Migrations/                       (Database schema evolution)
```

---

## 💡 TIPS FOR UNDERSTANDING THE CODE

### Tip 1: Follow One Request
Pick a simple operation like "Create a Team" and trace it through:
```
1. What does the HTTP request look like?
   POST /api/teams with JSON body
   
2. What does the controller do?
   Parse DTO, call service
   
3. What does the service do?
   Add to repo, save changes
   
4. What does the repo do?
   Add to DbContext, execute SaveChangesAsync
   
5. What does the database do?
   Insert record, return generated ID
   
6. What's the HTTP response?
   201 Created with URL to new resource
```

### Tip 2: Understand Async/Await
All methods are `async Task<...>` because:
- Database I/O is slow
- `async`/`await` frees up the thread while waiting
- Multiple requests can share threads
- Massive performance improvement under load

### Tip 3: Why Interfaces?
All services and repositories have interfaces (ITeamService, ITeamRepository):
- **Testability:** Mock implementations for unit tests
- **Flexibility:** Can swap implementations (SQL → NoSQL)
- **Loose coupling:** Controller depends on interface, not concrete class

### Tip 4: [JsonIgnore] Attribute
Why is Team [JsonIgnore] on Player?
```csharp
[JsonIgnore]
public Team? Team { get; set; }  // Don't serialize this in responses

// Without [JsonIgnore], a player response would be:
{
  "PlayerId": 1,
  "FullName": "John",
  "Team": {
    "TeamId": 5,
    "Name": "Manchester",
    "Players": [ ... ]  // ← Circular reference!
  }
}

// With [JsonIgnore], response is just:
{
  "PlayerId": 1,
  "FullName": "John",
  "TeamId": 5
}
```

### Tip 5: Why Multiple DTOs?
- **TeamCreateDto:** POST body, no ID (auto-generated)
- **TeamReadDto:** GET response, includes ID
- **TeamUpdateDto:** PUT body, no ID (in URL path)

Different operations need different data shapes!

---

## 🎯 COMMON QUESTIONS ANSWERED

### "How does a request get routed?"
1. HTTP request arrives at `/api/teams`
2. ASP.NET Core sees `TeamsController` with `[Route("api/[controller]")]`
3. Matches POST to `[HttpPost] Create()` method
4. Parameter binding converts JSON to DTO
5. Method executes

### "Why do we need both services and repositories?"
- **Repository:** Pure data access (Add, Update, Delete, Get)
- **Service:** Business logic (validation, error handling, coordination)
- Without services, business logic would be scattered in controllers

### "Why is SaveChangesAsync in the service?"
- Service owns the transaction
- Multiple repository calls might need to succeed together
- Only service knows if operation completed successfully

### "How does the database know the relationships?"
- Foreign key properties (TeamId in Player)
- Navigation properties (Team property in Player)
- Fluent API configuration in LeagueContext.OnModelCreating()

---

## 📚 NEXT STEPS FOR DEEPER UNDERSTANDING

1. **Try modifying a service** - Add validation to TeamService.CreateAsync()
2. **Add a new property** - Add "Stadium" to Team, run migration
3. **Write unit tests** - Mock ITeamRepository and test TeamService logic
4. **Try adding a new entity** - Create Sponsor model following the Clubs pattern
5. **Explore Entity Framework Core docs** - Learn advanced queries

---

## 🎓 YOU'RE NOW READY TO:

✅ Understand how a request flows through the app
✅ Add new endpoints (follow the Teams pattern)
✅ Modify business logic in services
✅ Update the database schema with migrations
✅ Write tests by mocking repositories
✅ Explain the architecture in your viva!

Good luck! 🚀
