# Retail Procurement System

A comprehensive .NET 8 + Angular retail procurement system with real-time updates via SignalR, JWT authentication, PostgreSQL, and Docker support.

## Architecture

```
src/
├── RetailProcurement.API           # ASP.NET Core Web API
│   ├── Controllers/                # StoreItems, Suppliers, Statistics, SupplierStoreItems, Auth
│   ├── Hubs/ProcurementHub.cs      # SignalR hub
│   └── Middleware/ExceptionMiddleware.cs
├── RetailProcurement.Core          # Domain layer
│   ├── Entities/                   # BaseEntity, StoreItem, Supplier, SupplierStoreItem, SalesRecord, QuarterlyPlanEntry, User
│   ├── DTOs/                       # Request/response DTOs
│   └── Interfaces/                 # IRepository, IUnitOfWork, IService contracts
├── RetailProcurement.Infrastructure # Data layer
│   ├── Data/                       # EF Core AppDbContext, entity configurations, migrations
│   ├── Data/Seeding/DataSeeder.cs  # Bogus-powered fake data seeder
│   ├── Repositories/               # Generic Repository + Unit of Work
│   ├── Services/                   # Business logic services
│   └── Patterns/                   # Strategy (BestOffer), Builder (QuarterlyPlan)
tests/
└── RetailProcurement.Tests         # xUnit unit + integration tests (33 tests)
frontend/
└── retail-procurement-ui/          # Angular 19 SPA
```

## Software Patterns Used

| Pattern | Location | Purpose |
|---|---|---|
| **Generic Repository** | `Infrastructure/Repositories/Repository.cs` | Reusable data-access abstraction |
| **Unit of Work** | `Infrastructure/Repositories/UnitOfWork.cs` | Transaction coordination |
| **Strategy** | `Infrastructure/Patterns/BestOffer/` | Pluggable best-offer selection (lowest price / fastest delivery) |
| **Builder** | `Infrastructure/Patterns/QuarterlyPlan/QuarterlyPlanBuilder.cs` | Fluent quarterly plan construction |

## Prerequisites

- Docker & Docker Compose
- OR: .NET 8 SDK, Node 18+, PostgreSQL 14+

## Running with Docker (recommended)

```bash
docker compose up --build
```

Services:
- API: http://localhost:8080 (Swagger at /swagger)
- Frontend: http://localhost:4200
- PostgreSQL: localhost:5432

The database is auto-migrated and seeded on first startup.

## Running locally

### API
```bash
# Start PostgreSQL first, then:
cd src/RetailProcurement.API
dotnet run
# API at https://localhost:5001, Swagger at https://localhost:5001/swagger
```

### Frontend
```bash
cd frontend/retail-procurement-ui
npm install
ng serve
# App at http://localhost:4200
```

## Running Tests

Due to Windows Application Control policy, output binaries from the Desktop must be redirected:

```bash
dotnet test tests/RetailProcurement.Tests -o C:\Temp\TestOutput
```

Or from a non-restricted path:
```bash
dotnet test tests/RetailProcurement.Tests
```

**33 tests** — unit tests for all controllers, service integration tests with in-memory DB.

## API Endpoints

| Method | Route | Auth Required | Description |
|---|---|---|---|
| GET | /api/store-items | No | List all store items |
| GET | /api/store-items/{id} | No | Get store item by ID |
| POST | /api/store-items | Yes | Create store item |
| PUT | /api/store-items/{id} | Yes | Update store item |
| DELETE | /api/store-items/{id} | Yes | Delete store item |
| GET | /api/suppliers | No | List all suppliers |
| GET | /api/suppliers/{id} | No | Get supplier by ID |
| POST | /api/suppliers | Yes | Create supplier |
| PUT | /api/suppliers/{id} | Yes | Update supplier |
| DELETE | /api/suppliers/{id} | Yes | Delete supplier |
| GET | /api/supplier-store-items | No | List all relationships |
| POST | /api/supplier-store-items | Yes | Create relationship |
| DELETE | /api/supplier-store-items/{supplierId}/{storeItemId} | Yes | Delete relationship |
| GET | /api/statistics/supplier/{id} | No | Supplier sales statistics |
| GET | /api/statistics/best-offer/{productId} | No | Best offer for a product |
| POST | /api/statistics/quarterly-plan | Yes | Create quarterly plan entry |
| GET | /api/statistics/quarterly-plan | No | Get current quarter's plan |
| POST | /api/auth/register | No | Register new user |
| POST | /api/auth/login | No | Login, get JWT token |

## Authentication

1. Register: `POST /api/auth/register`
2. Login: `POST /api/auth/login` → receive `token`
3. Add header: `Authorization: Bearer {token}` for protected endpoints
4. In Swagger UI: click **Authorize**, enter `Bearer {token}`

## SignalR

Connect to `/hubs/procurement`. Events broadcast on store item mutations:
- `StoreItemCreated` — new item created
- `StoreItemUpdated` — item updated
- `StoreItemDeleted` — item deleted (receives item ID)

## Optional Features Implemented

- ✅ **SignalR** — real-time store item events
- ✅ **JWT Authorization** — register/login, protected write endpoints
- ✅ **Angular Frontend** — full SPA with store items, suppliers, statistics, login/register

## Assumptions & Design Decisions

- Best offer strategy defaults to **lowest price**; can be swapped to `FastestDeliveryStrategy` via DI
- Database seeded with 10 suppliers, 30 store items, ~100 sales records using **Bogus**
- Passwords hashed with PBKDF2 (100k iterations, SHA-256)
- JWT tokens expire in 24 hours
- Tests run against an **in-memory database** for speed and isolation
- The `appsettings.json` JWT secret is safe for development; override via environment variables in production
