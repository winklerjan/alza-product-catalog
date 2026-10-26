# Architectural decisions

## Product availability

- Date: 2026-10-26
- Decision: `Product` gets an `IsAvailable` flag. The list endpoints (v1 and v2) return only available products; `GET /products/{id}` and the description PATCH are unaffected by availability. `IsAvailable` is exposed in the API response.
- Alternatives rejected: treating "available" as every product in the catalog (no filtering); a full stock-quantity model with a computed availability flag.
- Rationale: the assignment asks for an endpoint to "list all available products," which implies some products can be unavailable. A plain boolean is the simplest model that satisfies this without the added complexity of stock tracking, which remains a possible stretch goal.

## Runtime

- Date: 2026-10-26
- Decision: .NET 10 (LTS), SDK pinned in `global.json` with `rollForward: latestFeature`.
- Alternatives rejected: an unpinned SDK version, which risks a different SDK being picked up on another machine.
- Rationale: .NET 10 is the current LTS; pinning keeps builds reproducible across machines.

## Solution file

- Date: 2026-10-26
- Decision: `ProductCatalog.slnx`, the XML solution format.
- Alternatives rejected: the classic `.sln` format.
- Rationale: `dotnet new sln` defaults to `.slnx` on the installed SDK; it's simpler to read and diff, and is supported by VS 2022 17.13+/VS 2026, VS Code with the C# Dev Kit, and Rider.

## Architecture

- Date: 2026-10-26
- Decision: Layered architecture: `Domain` ← `Application` ← `Infrastructure`, with `Api` as the composition root. Manual mapping between entities and DTOs.
- Alternatives rejected: MediatR/CQRS for request handling, AutoMapper for mapping.
- Rationale: the application has 4 endpoints; a mediator pipeline and reflection-based mapping would add indirection without a matching benefit. Manual mapping is explicit and easy to follow.

## API style

- Date: 2026-10-26
- Decision: Controllers with `[ApiController]`.
- Alternatives rejected: minimal APIs.
- Rationale: conventional, and versioning plus Swagger integration is straightforward with controllers.

## Versioning

- Date: 2026-10-26
- Decision: `Asp.Versioning.Mvc` + `Asp.Versioning.Mvc.ApiExplorer`, URL segment versioning (`/api/v{version}/products`).
- Alternatives rejected: header or query-string versioning.
- Rationale: URL segment versioning is the most discoverable and explicit option, and shows up clearly in Swagger.

## Swagger

- Date: 2026-10-26
- Decision: `Swashbuckle.AspNetCore`, one Swagger document per API version, UI at `/swagger` with XML comments enabled.
- Rationale: per-version documents keep each version's contract isolated and readable.

## Partial update

- Date: 2026-10-26
- Decision: `PATCH /api/v{version}/products/{id}` with a plain JSON body (`{ "description": "..." }`), not JSON Patch.
- Alternatives rejected: RFC 6902 JSON Patch.
- Rationale: the only partial update supported is the description field; a JSON Patch document would be unnecessary ceremony for a single-field update.

## Pagination

- Date: 2026-10-26
- Decision: v2 list takes `page` (default 1, min 1) and `pageSize` (default 10, range 1-100), ordered by `Id`. Out-of-range values return 400; a page past the end returns 200 with an empty `items` array.
- Rationale: stable, predictable pagination with explicit bounds.

## Errors

- Date: 2026-10-26
- Decision: RFC 9457 Problem Details for all error responses.
- Rationale: a standard, machine-readable error format.

## Validation

- Date: 2026-10-26
- Decision: DataAnnotations on request models, enforced automatically by `[ApiController]`. Domain entities also guard their own invariants.
- Alternatives rejected: FluentValidation.
- Rationale: DataAnnotations cover the validation needs of this API without an extra dependency; the domain layer is the second line of defense regardless of what validates at the boundary.

## ORM and database

- Date: 2026-10-26
- Decision: EF Core with the SQL Server provider, migrations in `Infrastructure`, SQL Server running in Docker via `docker-compose.yml`.
- Rationale: EF Core is the standard ORM for .NET; SQL Server was the preferred database for this project.

## Mock vs. database

- Date: 2026-10-26
- Decision: One `IProductRepository` abstraction with two implementations (`SqlProductRepository`, `InMemoryProductRepository`), selected by the `Persistence:Provider` configuration value. Both share the same seed data.
- Rationale: lets the API run without Docker for quick iteration and testing, while keeping production code paths identical to the database-backed one.

## Tests

- Date: 2026-10-26
- Decision: xUnit v3, NSubstitute for mocking, Shouldly for assertions.
- Rationale: a mainstream combination with no licensing friction.

## Quality

- Date: 2026-10-26
- Decision: `.editorconfig`, nullable reference types enabled, warnings treated as errors, latest recommended analysis level, Central Package Management for NuGet versions.
- Rationale: catches correctness and style issues at build time instead of in review, and keeps dependency versions consistent across projects.
