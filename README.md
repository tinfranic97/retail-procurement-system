# Retail Procurement System

A full-stack retail procurement application built with .NET 8 Web API and Angular 19, containerized with Docker and served via nginx.

---

## Tech Stack

| Layer | Technology |
|---|---|
| API | ASP.NET Core 8, Entity Framework Core 8 |
| Database | PostgreSQL 16 |
| Frontend | Angular 19 (standalone components) |
| Real-time | SignalR |
| Auth | JWT Bearer tokens, PBKDF2 password hashing |
| Testing | xUnit, Moq, FluentAssertions, EF Core In-Memory |
| Containerization | Docker, Docker Compose, nginx |
| Data seeding | Bogus |

---

## Project Structure

```
├── src/
│   ├── RetailProcurement.API/              # ASP.NET Core Web API
│   │   ├── Controllers/                    # StoreItems, Suppliers, Statistics, SupplierStoreItems, Auth
│   │   ├── Hubs/ProcurementHub.cs          # SignalR hub
│   │   └── Middleware/ExceptionMiddleware.cs
│   ├── RetailProcurement.Core/             # Domain layer
│   │   ├── Entities/                       # BaseEntity, StoreItem, Supplier, SupplierStoreItem, SalesRecord, QuarterlyPlanEntry, User
│   │   ├── DTOs/                           # Request/response DTOs
│   │   └── Interfaces/                     # IRepository, IUnitOfWork, IService contracts
│   └── RetailProcurement.Infrastructure/   # Data & business logic layer
│       ├── Data/                           # EF Core AppDbContext, migrations
│       ├── Data/Seeding/DataSeeder.cs      # Bogus-powered fake data seeder
│       ├── Repositories/                   # Generic Repository + Unit of Work
│       ├── Services/                       # Business logic services
│       └── Patterns/                       # Strategy (BestOffer), Builder (QuarterlyPlan)
├── tests/
│   └── RetailProcurement.Tests/            # xUnit unit + integration tests
├── frontend/
│   └── retail-procurement-ui/             # Angular 19 SPA
├── docker-compose.yml
└── README.md
```

---

## Running the Application

### With Docker (recommended)

```bash
docker compose up --build -d
```

Once all containers are healthy, the application is available at:

| Service | URL |
|---|---|
| Frontend | http://localhost:4200 |
| API (direct) | http://localhost:8080/api |
| Swagger UI | http://localhost:8080/swagger |
| SignalR Hub | http://localhost:4200/hubs/procurement |

> On first startup the API automatically runs EF Core migrations and seeds the database with 10 suppliers, 30 store items, 58 supplier–item relationships, and 100 sales records generated with Bogus.

To stop all containers:

```bash
docker compose down
```

To stop and remove all data (including the PostgreSQL volume):

```bash
docker compose down -v
```

---

### Running Locally (without Docker)

**API** — requires a running PostgreSQL instance. Update the connection string in `src/RetailProcurement.API/appsettings.json` if needed.

```bash
cd src/RetailProcurement.API
dotnet run
# API at http://localhost:8080, Swagger at http://localhost:8080/swagger
```

**Frontend** — proxies API and SignalR calls to the local API:

```bash
cd frontend/retail-procurement-ui
npm install
ng serve --proxy-config proxy.conf.json
# App at http://localhost:4200
```

---

## Running Tests

```bash
dotnet test tests/RetailProcurement.Tests
```

On Windows with Application Control policies that block Desktop execution, redirect the output:

```bash
mkdir C:\Temp\TestOutput
dotnet test tests/RetailProcurement.Tests -o C:\Temp\TestOutput
```

### Test coverage

| File | Type | What is tested |
|---|---|---|
| `Unit/Controllers/StoreItemsControllerTests.cs` | Unit | Controller request handling and HTTP responses |
| `Unit/Controllers/SuppliersControllerTests.cs` | Unit | Controller request handling and HTTP responses |
| `Unit/Controllers/StatisticsControllerTests.cs` | Unit | Controller request handling and HTTP responses |
| `Unit/Services/StoreItemServiceTests.cs` | Unit | Service business logic with mocked repository |
| `Unit/Services/SupplierServiceTests.cs` | Unit | Service business logic with mocked repository |
| `Integration/StatisticsServiceIntegrationTests.cs` | Integration | Full service layer against EF Core In-Memory DB |

---

## API Endpoints

All write endpoints (`POST`, `PUT`, `DELETE`) require a JWT Bearer token. Obtain one via `/api/auth/register` or `/api/auth/login`.

### Store Items — `/api/store-items`

| Method | Endpoint | Auth | Description |
|---|---|---|---|
| GET | `/api/store-items` | — | List all store items |
| GET | `/api/store-items/{id}` | — | Get a store item by ID |
| POST | `/api/store-items` | ✅ | Create a store item |
| PUT | `/api/store-items/{id}` | ✅ | Update a store item |
| DELETE | `/api/store-items/{id}` | ✅ | Delete a store item |

### Suppliers — `/api/suppliers`

| Method | Endpoint | Auth | Description |
|---|---|---|---|
| GET | `/api/suppliers` | — | List all suppliers |
| GET | `/api/suppliers/{id}` | — | Get a supplier by ID |
| POST | `/api/suppliers` | ✅ | Create a supplier |
| PUT | `/api/suppliers/{id}` | ✅ | Update a supplier |
| DELETE | `/api/suppliers/{id}` | ✅ | Delete a supplier |

### Statistics — `/api/statistics`

| Method | Endpoint | Auth | Description |
|---|---|---|---|
| GET | `/api/statistics/supplier/{id}` | — | Sales statistics for a supplier |
| GET | `/api/statistics/best-offer/{productId}` | — | Best supplier offer for a product |
| POST | `/api/statistics/quarterly-plan` | ✅ | Create a quarterly procurement plan |
| GET | `/api/statistics/quarterly-plan` | — | Retrieve the current quarter's plan |

### Supplier–Store Item Relationships — `/api/supplier-store-items`

| Method | Endpoint | Auth | Description |
|---|---|---|---|
| GET | `/api/supplier-store-items` | — | List all supplier–item relationships |
| POST | `/api/supplier-store-items` | ✅ | Link a supplier to a store item |
| DELETE | `/api/supplier-store-items/{supplierId}/{storeItemId}` | ✅ | Remove a supplier–item link |

### Auth — `/api/auth`

| Method | Endpoint | Description |
|---|---|---|
| POST | `/api/auth/register` | Register a new user, returns a JWT token |
| POST | `/api/auth/login` | Login with existing credentials, returns a JWT token |

---

## Authentication

Register a new user:

```http
POST /api/auth/register
Content-Type: application/json

{
  "username": "yourname",
  "email": "you@example.com",
  "password": "YourPassword1"
}
```

Or log in with an existing account:

```http
POST /api/auth/login
Content-Type: application/json

{
  "username": "yourname",
  "password": "YourPassword1"
}
```

Both return an `AuthResponse` containing a `token`. Use it on protected endpoints:

```
Authorization: Bearer <token>
```

In Swagger UI, click **Authorize** and enter `Bearer <token>`.

**Validation rules:**
- Username: 3–100 characters, must be unique
- Email: valid email format, must be unique
- Password: minimum 8 characters

---

## Software Patterns

| Pattern | Location | Purpose |
|---|---|---|
| **Generic Repository** | `Infrastructure/Repositories/Repository.cs` | Reusable, type-safe data access for all entities |
| **Unit of Work** | `Infrastructure/Repositories/UnitOfWork.cs` | Coordinates all repositories under a single `SaveChangesAsync` |
| **Strategy** | `Infrastructure/Patterns/BestOffer/` | Pluggable best-offer algorithm (`LowestPriceStrategy`, `FastestDeliveryStrategy`) |
| **Builder** | `Infrastructure/Patterns/QuarterlyPlan/QuarterlyPlanBuilder.cs` | Fluent, validated construction of quarterly procurement plans |

---

## Optional Features (all implemented)

- **SignalR** — `ProcurementHub` broadcasts `StoreItemCreated`, `StoreItemUpdated`, and `StoreItemDeleted` events in real time to all connected clients.
- **Angular frontend** — full SPA with store items, suppliers, statistics, and login/register. Connects to SignalR for live updates without page refresh.
- **JWT Authorization** — register/login flow with PBKDF2-hashed passwords and 24-hour JWT tokens. All mutating API endpoints are protected.

---

## Assumptions & Design Decisions

- The best-offer strategy defaults to **lowest price**. The `FastestDeliveryStrategy` is implemented and can be swapped via DI in `Program.cs`.
- Creating a quarterly plan for a quarter that already has a plan replaces the existing entries.
- JWT tokens are stateless; logout is handled client-side by discarding the stored token.
- The JWT secret in `appsettings.json` is suitable for development only. Override it via environment variables in production.
- Tests use an **EF Core In-Memory database** for speed and isolation; no external database is required to run them.
