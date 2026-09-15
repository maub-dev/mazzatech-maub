# Order Management API

A .NET 10 order-management API built with Clean Architecture and CQRS. It uses
Controllers because the API benefits from a conventional, discoverable HTTP layer
with route/action metadata while the application layer retains all use-case
orchestration and business logic.

## Architecture

| Project                          | Responsibility                                                               |
| -------------------------------- | ---------------------------------------------------------------------------- |
| `OrderManagement.Domain`         | Order aggregate, status transitions, and domain total calculation             |
| `OrderManagement.Application`    | MediatR commands/queries, handlers, request validation pipeline, and ports     |
| `OrderManagement.Infrastructure` | EF Core SQLite persistence, migrations, repository, and JWT token service    |
| `OrderManagement.Api`            | Controller endpoints, JWT configuration, and HTTP error mapping              |

`TotalAmount` is calculated by `Order` from its items. Create-order requests are
validated before handling and require at least one item, non-empty product names,
and positive quantities and prices. Only pending orders can be cancelled.
`UnitPrice` is persisted as invariant-culture decimal text in SQLite to avoid
floating-point precision loss. The MediatR pipeline uses Serilog to log every
command/query request and response with elapsed execution time; failed requests
include the associated exception.

## Run locally

Install the .NET 10 SDK, then:

```shell
dotnet restore
dotnet run --project src/OrderManagement.Api
```

The API applies pending EF Core migrations automatically during startup and stores
data in the SQLite database file `orders.db` in the working directory.

Swagger UI is available at `http://localhost:5070/swagger` when using the HTTP
launch profile. Use **Authorize** to provide the JWT returned by `/auth/login`.

For Swagger authentication, use these local/demo credentials:

- **Email:** `dev@mazzatech.com`
- **Password:** `Senha@123`

After signing in through `/auth/login`, copy the returned access token into the
**Authorize** dialog. The fixed credentials are intended for local/demo use only.

Available endpoints:

| Method  | Route                            |
| ------- | -------------------------------- |
| `POST`  | `/auth/login`                    |
| `POST`  | `/api/orders`                    |
| `GET`   | `/api/orders?page=1&pageSize=10` |
| `GET`   | `/api/orders/{id}`               |
| `PATCH` | `/api/orders/{id}/cancel`        |

## Run with Docker

```shell
docker compose up --build
```

The service is available on `http://localhost:8080`. SQLite data persists in the
named `orders-data` volume.

## Tests

```shell
dotnet test
```
