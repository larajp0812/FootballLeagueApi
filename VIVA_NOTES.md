# Football League API - Viva Script

## INTRODUCTION

Good [morning/afternoon]. I'm here to present my Football League API project. This is a REST API built with .NET 8.0 and ASP.NET Core that manages all aspects of a football league system. 

The project allows users to manage teams, players, matches across different seasons, match events like goals and cards, venues, and also includes user authentication with login and registration. What I'm particularly proud of is that I've implemented this following clean architecture principles with a layered design pattern, which I'll explain in detail.



---

## THE ARCHITECTURE

Let me walk you through the architecture I chose. I decided on a layered architecture with four main layers, and there's a clear reason for this approach.

At the top, we have the **Controllers** layer. This is where HTTP requests come in. The Teams Controller, for example, handles GET, POST, PUT, and DELETE requests for teams. Controllers are intentionally thin—they just parse the request, call the service layer, and send back a response. They're not where business logic lives.

Below that is the **Services** layer. This is where the business logic actually happens. The TeamService, for instance, handles operations like creating a team, updating it, or deleting it. It takes data from the repository, applies any business rules, and coordinates the overall workflow. I use async/await here for non-blocking I/O, which is important for performance in a web API.

Then we have the **Repositories** layer. This abstracts all direct database access. Each entity—Team, Player, Match, etc.—has its own repository interface and implementation. The repository is responsible for fetching data, saving data, and querying. By having this abstraction layer, I can easily swap out the database implementation later if needed, and it makes testing much simpler because I can mock the repositories.

Finally, at the bottom is the **Data Access** layer, which is Entity Framework Core. EF Core is an ORM that handles all the low-level database operations. I'm using a code-first approach where my C# models define the database schema, and migrations track how the schema evolves over time.

The key benefit of this architecture is **separation of concerns**. Each layer has a single responsibility, the code is testable, and it's scalable. If I need to add new features, I can do so without changing existing code.




---

## THE DATA MODEL

Now let me explain the domain model. The core entity is **Team**. Each team has a TeamId, Name, Coach, and FoundedYear. Teams have relationships—they can have many Players associated with them, and they play many Matches in both home and away roles. I separated HomeMatches and AwayMatches because a team plays from both perspectives, and it's useful to track form separately.

**Players** belong to a team. They have their position and other attributes, and the relationship is many-to-one.

**Matches** are the events. A match has a HomeTeam and AwayTeam, both referenced from the Team entity. It belongs to a Season, which indicates when it's played. And crucially, a match can have multiple **MatchEvents**—these represent things like goals being scored, cards being given, or substitutions happening.

**Seasons** are containers for matches. They have a name like "2025/26", a start date, and an end date. All matches belong to a season.

**Venues** are the stadiums where matches are played. They have a location and capacity.

I also have an **Auth system** with Login and Register functionality, handled through the AuthController with DTOs for those requests.

The relationships are all navigational, which means Entity Framework Core automatically manages the foreign keys for me.



---

## CRUD OPERATIONS AND THE DTO PATTERN

Let me explain how CRUD operations work using Teams as an example. When someone makes a POST request to /api/teams to create a team, they send a **TeamCreateDto**. This DTO contains Name, Coach, and FoundedYear—notice it doesn't include the TeamId because that's auto-generated.

The controller receives this DTO, the service validates it, converts it to a Team model, and saves it. The response comes back as a **TeamReadDto**, which includes the TeamId plus all the other fields.

For updating with a PUT request to /api/teams/{id}, we use a **TeamUpdateDto**. Again, no TeamId in the body because it's in the URL. The service validates, updates the team, and returns 204 No Content.

For DELETE to /api/teams/{id}, the service deletes the team and returns 204 No Content or 404 if not found.

And for GET requests, we return the TeamReadDto with all the information.

**Why separate DTOs?** This is important for several reasons. First, **security**—we don't expose internal IDs or sensitive relationships unnecessarily. Second, **flexibility**—we can change the API contract without changing the domain model. Third, **validation**—we can validate the input DTO separately from the domain model. And fourth, **performance**—we only expose the fields that clients need.

So a CreateDto has minimal fields, a ReadDto has everything, and an UpdateDto might have different fields than Create. This is a best practice in API design.



---

## SERVICES AND REPOSITORIES

The **Service layer** is where orchestration happens. When we call CreateAsync on the TeamService, for example, it takes the Team model, calls the repository to add it, then calls SaveChangesAsync to commit the transaction. The service is also where you'd put any validation logic or business rules—like checking that a team name is unique, or that a player hasn't exceeded the maximum salary.

By keeping business logic in the service and not in the controller, we maintain a clean separation. The controller just coordinates the request/response, while the service knows how to create a team properly.

The **Repository layer** is an abstraction for data access. Each entity has an interface like ITeamRepository and a concrete implementation. The repository handles all queries like GetAllAsync, GetByIdAsync, AddAsync, UpdateAsync, DeleteAsync. By programming to an interface rather than a concrete class, we make the code testable. In unit tests, we can mock the repository and test the service logic without hitting a real database.

This also gives us flexibility. If tomorrow we decide to switch from SQL Server to MongoDB, we only need to change the repository implementation. The services and controllers don't need to change at all.



---

## ENTITY FRAMEWORK CORE AND MIGRATIONS

I'm using Entity Framework Core with a code-first approach. This means I define my C# models first, and EF Core generates the database schema from those models. The relationships are defined through navigation properties—so when a Team has a collection of Players, EF Core knows to create a foreign key constraint.

I have two migrations visible in the project. The first is InitialCreate, which sets up the basic schema. The second is TeamUpdated, which shows how the schema evolved—maybe we added the Coach field or FoundedYear. With migrations, every database change is tracked and versioned. If I need to roll back, I can. If I need to apply the schema to a different environment, I just run the migrations. This is much better than running manual SQL scripts.



---

## DESIGN DECISIONS AND JUSTIFICATION

I made several deliberate design choices in this project. Let me explain why.

First, **Dependency Injection**. Every service receives its dependencies through the constructor. This creates loose coupling—the controller doesn't care whether the TeamService uses a TeamRepository or a mock. It just depends on the abstraction. This makes testing easy and the code more flexible.

Second, **Async/Await throughout**. All database operations are asynchronous. This means while we're waiting for the database to return a result, the thread can handle another request. This significantly improves performance under load.

Third, **SOLID principles**. The code follows Single Responsibility—each class has one reason to change. It follows the Dependency Inversion Principle by depending on abstractions, not concrete classes. Open/Closed Principle—we can extend functionality without modifying existing code. Liskov Substitution—repositories are interchangeable. Interface Segregation—interfaces are focused and not bloated.

Fourth, **Error handling through HTTP status codes**. A missing team returns 404 Not Found. A creation returns 201 Created. No content updates return 204 No Content. This is RESTful and clients can handle them appropriately.

All of these decisions were made with testability, maintainability, and scalability in mind.



---

## ANSWERING POTENTIAL QUESTIONS

If you ask me about concurrent updates, that's a great question. Entity Framework Core supports optimistic concurrency with a RowVersion field. If two clients try to update the same team simultaneously, the second update will detect the conflict and we can handle it gracefully.

If you ask what happens when we delete a team with players—that's something I should strengthen. Currently, there's no cascade delete constraint defined. Ideally, we'd either prevent deletion if there are related players, or cascade delete them. It's a consideration for data integrity.

On handling large datasets, right now the GetAll endpoints return everything. If we had thousands of teams, that would be slow. We'd need pagination—returning maybe 50 teams at a time with a page number parameter.

For team identification, I have both HomeMatches and AwayMatches to track a team's home and away form separately. A team plays matches from both perspectives, so this separation is meaningful.

And for match events, the relationship is straightforward—when a goal is scored, we create a MatchEvent with type "Goal" associated with that match. This allows us to build a complete record of what happened in the match.



---

## TESTING AND QUALITY ASSURANCE

For testing, the architecture I've built makes it easy. With mocked repositories, I can unit test each service in isolation. For example, I can test that UpdateTeam returns false if the ID doesn't match, or that CreateTeam saves correctly.

Integration tests would use a real database—maybe a test database—and verify that the full flow works from controller to database.

API tests would call the endpoints directly and verify the responses, error codes, and data.

The code is structured so that testing is straightforward because of dependency injection and the repository abstraction.

---

## PERFORMANCE CONSIDERATIONS

Currently, the API is structurally sound but could be optimized. Pagination would be crucial—GetAll endpoints should return limited results, maybe 50 at a time, to avoid transferring massive datasets.

Caching could help. Seasons rarely change, for instance. We could cache them in memory to avoid database hits.

Indexing the database on frequently queried columns—like TeamId in the Player table—would speed up queries.

We could use projections in LINQ to query only the fields we need rather than entire entities.

And lazy loading vs eager loading—if a team has 100 players and we only want the team name, we shouldn't fetch all the players.



---

## SECURITY

Security is important. The API has authentication with login and registration, which prevents unauthenized access. Entity Framework Core prevents SQL injection automatically through parameterized queries—we never interpolate user input into SQL.

We use DTOs to avoid exposing internal IDs unnecessarily. We validate input data before processing.

For a production system, we'd add CORS configuration to control which domains can call the API, HTTPS to encrypt data in transit, password hashing for authentication, and authorization to control what authenticated users can do.

We'd also add rate limiting to prevent abuse, and logging to audit who did what and when.

---

## FUTURE ENHANCEMENTS

There are several exciting features we could add. League standings—automatically calculated from match results. Player statistics—track goals, assists, cards per player. Email notifications when matches are coming up. File uploads for team logos or player photos. A mobile app on top of this API. Real-time updates using SignalR so clients see match events as they happen.

The architecture I've built makes it easy to add these features without disrupting existing code.

---

## SUMMARY OF STRENGTHS

This project demonstrates solid software engineering principles. The layered architecture is clean and professional. The code follows SOLID principles. It uses established patterns like Repository and Dependency Injection. It's testable, maintainable, and scalable. The separation between controllers, services, and repositories is crisp. The DTO pattern provides good API design. And the migrations system gives us version control over the database.

---

## AREAS FOR IMPROVEMENT

Being honest, there are gaps. There's no comprehensive error handling—I haven't added a global exception handler. There's no logging, so if something goes wrong in production, we wouldn't have a clear audit trail. There are no unit tests written yet, which is important for quality assurance. Validation could be stronger with FluentValidation. Authorization isn't implemented—anyone can do anything. Pagination isn't implemented, so large datasets could be slow. There are no soft deletes, so deleting a team is permanent. And lacking timestamps like CreatedAt and UpdatedAt makes it hard to track when things happened.

These are improvements I'd make in a second iteration or production environment.

