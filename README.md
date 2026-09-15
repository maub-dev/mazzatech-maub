# Order Management API

A .NET 10 order-management API built with Clean Architecture and CQRS. It uses
Controllers because the API benefits from a conventional, discoverable HTTP layer
with route/action metadata while the application layer retains all use-case
orchestration and business logic.

## Architecture

| Project | Responsibility |
|---|---|
| `OrderManagement.Domain` | Order aggregate, order-item invariants, status, and domain total calculation |
| `OrderManagement.Application` | MediatR commands/queries, handlers, validation pipeline, and ports |
| `OrderManagement.Infrastructure` | EF Core SQLite persistence, migrations, repository, and JWT token service |
| `OrderManagement.Api` | Controller endpoints, JWT configuration, and HTTP error mapping |

`TotalAmount` is calculated by `Order` from its items. An order cannot have no
items, zero/negative quantities or prices, and only pending orders can be cancelled.
The MediatR pipeline uses Serilog to log every command/query request and response
with elapsed execution time; failed requests include the associated exception.

## Run locally

Install the .NET 10 SDK, then:

```powershell
dotnet restore
dotnet run --project src\OrderManagement.Api
```

The API applies pending EF Core migrations automatically during startup and stores
the SQLite database in `orders.db` in the working directory.

Swagger UI is available at `http://localhost:5070/swagger` when using the HTTP
launch profile. Use **Authorize** to provide the JWT returned by `/auth/login`.

Authenticate first:

```powershell
$login = Invoke-RestMethod -Method Post http://localhost:5070/auth/login `
  -ContentType application/json `
  -Body '{"email":"dev@martech.com","password":"Senha@123"}'
```

Use `$login.accessToken` as a Bearer token for:

| Method | Route |
|---|---|
| `POST` | `/auth/login` |
| `POST` | `/api/orders` |
| `GET` | `/api/orders?page=1&pageSize=10` |
| `GET` | `/api/orders/{id}` |
| `PATCH` | `/api/orders/{id}/cancel` |

## Run with Docker

```powershell
docker compose up --build
```

The service is available on `http://localhost:8080`. SQLite data persists in the
named `orders-data` volume.

## Tests

```powershell
dotnet test
```