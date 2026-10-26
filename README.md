# ProductCatalog

A REST API for an e-shop product catalog: listing products, retrieving a single product, and
updating a product's description.

## Tech stack

- .NET 10 (LTS), ASP.NET Core with controllers
- `Asp.Versioning.Mvc` for URL-segment API versioning (`/api/v1/...`, `/api/v2/...`)
- `Swashbuckle.AspNetCore` for Swagger/OpenAPI documentation
- EF Core with the SQL Server provider, and an in-memory provider for running without a database
- xUnit v3, NSubstitute and Shouldly for tests

## Architecture

```
src/
  ProductCatalog.Domain/          entities and invariants, no dependencies
  ProductCatalog.Application/     services, DTOs, repository abstraction
  ProductCatalog.Infrastructure/  EF Core, SQL repository, in-memory repository, seed data
  ProductCatalog.Api/             controllers, request models, versioning, Swagger, DI
tests/
  ProductCatalog.UnitTests/       mirrors the src/ layer structure
  ProductCatalog.IntegrationTests/ end-to-end tests over real HTTP, via WebApplicationFactory
```

Dependencies point inward: `Domain` has none, `Application` depends only on `Domain`,
`Infrastructure` depends on `Application` and `Domain`, and `Api` is the composition root.
Controllers only ever depend on `Application` abstractions, never on EF Core or Infrastructure
types directly. See [`docs/DECISIONS.md`](docs/DECISIONS.md) for the reasoning behind these and
other choices.

## Prerequisites

- .NET 10 SDK
- Docker (for running against a real SQL Server), **or** SQL Server LocalDB on Windows
- `jq` is only needed if you're using the Claude Code agent tooling in this repo, not to build or run the app

## Running with SQL Server

```bash
dotnet tool restore
docker compose up -d
dotnet run --project src/ProductCatalog.Api --launch-profile https
```

This applies pending EF Core migrations and seeds the database automatically on startup
(`Persistence:MigrateOnStartup` is `true` in Development). Swagger opens automatically at
`https://localhost:7165/swagger`.

The SA password in `docker-compose.yml` and `appsettings.json` (`Local_dev_only_123!`) is for
the local, throwaway Docker container only — it is not a secret worth protecting and should
never be reused anywhere real.

On Windows without Docker, point the `ProductCatalog` connection string in `appsettings.json` at
LocalDB instead:

```
Server=(localdb)\mssqllocaldb;Database=ProductCatalog;Trusted_Connection=True;TrustServerCertificate=True
```

## Running with mock data (no database needed)

```bash
dotnet run --project src/ProductCatalog.Api --launch-profile Mock
```

Uses an in-memory repository seeded with the same data as the SQL Server path. Swagger opens
automatically at `https://localhost:7165/swagger`.

## Switching data source

Set `Persistence:Provider` to `SqlServer` or `Mock` in `appsettings.json`, or override it per
run with the `Persistence__Provider` environment variable (this is how the `Mock` launch profile
works). No code changes are needed either way.

## API documentation

Swagger UI is available at `/swagger` when running in Development, with a dropdown to switch
between the v1 and v2 documents.

## Endpoints

| Method | Route | Versions | Example |
| --- | --- | --- | --- |
| GET | `/api/v1/products` | 1.0 | `curl https://localhost:7165/api/v1/products` |
| GET | `/api/v2/products?page=&pageSize=` | 2.0 | `curl "https://localhost:7165/api/v2/products?page=1&pageSize=10"` |
| GET | `/api/v{1,2}/products/{id}` | 1.0, 2.0 | `curl https://localhost:7165/api/v1/products/1` |
| PATCH | `/api/v{1,2}/products/{id}` | 1.0, 2.0 | `curl -X PATCH https://localhost:7165/api/v1/products/1 -H "Content-Type: application/json" -d "{\"description\":\"New description\"}"` |

All error responses are RFC 9457 Problem Details (`application/problem+json`).

## Running tests

```bash
dotnet test
```

Runs both the unit and integration test projects. No Docker is needed — the integration tests
run against the in-memory provider via `WebApplicationFactory`.

## Project structure

See [Architecture](#architecture) above. Architectural decisions and their reasoning are in
[`docs/DECISIONS.md`](docs/DECISIONS.md).

## Development process

<!-- Written by Jan. -->
