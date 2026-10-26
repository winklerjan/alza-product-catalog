# CLAUDE.md

Guidance for AI coding agents working in this repository. Humans may find it useful too.

## Project

**ProductCatalog** is a REST API for an e-shop product catalog: listing products (v1 full list, v2 paginated), retrieving one product by id, and updating a product's description.

Stack: .NET 10 (LTS), ASP.NET Core controllers, API versioning (URL segment), Swashbuckle (Swagger UI at `/swagger`), EF Core with SQL Server, and an in-memory provider with mock data. Tests use xUnit v3, NSubstitute and Shouldly.

Architectural decisions and their reasoning are in [`docs/DECISIONS.md`](docs/DECISIONS.md). Read it before proposing structural changes, and add an entry when you make one.

## Commands

```bash
dotnet tool restore                      # restores the pinned dotnet-ef tool
dotnet build                             # warnings are errors
dotnet test                              # all tests; no Docker needed

# Run with mock data (no database needed)
dotnet run --project src/ProductCatalog.Api --launch-profile Mock

# Run with SQL Server
docker compose up -d
dotnet run --project src/ProductCatalog.Api --launch-profile https

# Migrations
dotnet ef migrations add <Name> --project src/ProductCatalog.Infrastructure --startup-project src/ProductCatalog.Api
```

## Architecture

```
src/
  ProductCatalog.Domain/          entities and invariants, no dependencies
  ProductCatalog.Application/     services, DTOs, repository abstraction
  ProductCatalog.Infrastructure/  EF Core, SQL repository, in-memory repository, seed data
  ProductCatalog.Api/             controllers, request models, versioning, Swagger, DI
tests/
  ProductCatalog.UnitTests/
  ProductCatalog.IntegrationTests/
```

Dependency rules (strict):

- `Domain` references nothing.
- `Application` references only `Domain`.
- `Infrastructure` references `Application` and `Domain`.
- `Api` references `Application`, and `Infrastructure` only to call `AddInfrastructure(...)` in `Program.cs`. Controllers depend on Application abstractions only, never on Infrastructure types or EF Core.
- Each layer exposes one DI extension (`AddApplication()`, `AddInfrastructure(IConfiguration)`). `Program.cs` stays short.

Data source is chosen by configuration, `Persistence:Provider` = `SqlServer` or `Mock`. Both repository implementations share one seed data source (`ProductSeedData`).

## API conventions

- Routes: `/api/v{version}/products`, plural nouns, no verbs.
- Errors are RFC 9457 Problem Details: 404 for unknown resources, 400 `ValidationProblemDetails` for invalid input, 500 without leaking internals.
- Input validation uses DataAnnotations on request models. Domain entities guard their own invariants as well.
- Every action declares `[ProducesResponseType]` for each status code it can return and has XML doc comments (they feed Swagger).
- A new API version must appear as its own Swagger document.

## Code conventions

- File-scoped namespaces, one public type per file, folders match namespaces.
- Classes `sealed` unless designed for inheritance. Records for DTOs and results.
- `async` all the way with `CancellationToken` as the last parameter, always passed down. No `.Result` or `.Wait()`.
- Nullable reference types on. No `!` suppression without a clear reason.
- Manual mapping (`product.ToDto()`). No AutoMapper, MediatR or similar.
- Structured logging with message templates, never string interpolation.
- Comments explain why, not what. No `TODO`s, commented-out code, or dead code.
- Package versions live in `Directory.Packages.props` (Central Package Management).

## Data layer

- EF Core reads use `AsNoTracking()`. Paging always orders by `Id` first.
- Never hand-edit files in `Migrations/`. Change the model and regenerate.
- Seed data must be static and deterministic (no `DateTime.Now`, no randomness).
- The SA password in `docker-compose.yml` and `appsettings.json` is for the local throwaway container only.

## Testing

- Unit tests mirror the `src` folder structure. Integration tests use `WebApplicationFactory` with the `Mock` provider.
- Names follow `MethodName_Scenario_ExpectedResult`. Arrange, Act, Assert. No logic in tests; use `[Theory]`.
- Tests are deterministic and isolated: no real time, randomness, network, or shared mutable state between tests.
- `dotnet test` from the repo root must pass without Docker.

## Working rules for agents

- For C# symbol navigation (find references, go to definition, checking the impact of a rename or signature change), use the LSP tool, not grep. Use grep for text search and non-C# files.
- Keep changes small and focused on the task. Mention unrelated improvements instead of making them.
- Before finishing: `dotnet build` with zero warnings and `dotnet test` green.
- Do not add NuGet packages without stating why. Use the latest stable version compatible with `net10.0`, checked rather than guessed.
- Never weaken guardrails to get green: no disabling analyzers or warnings, skipping tests, or bypassing hooks.
- Commits follow Conventional Commits, imperative mood, lowercase subject up to 72 characters.

## Guardrails

`.claude/settings.json` defines tool permissions and hooks:

- `guard-bash.sh` blocks destructive shell and git commands (push, history rewrites, hook bypass, recursive deletes, dropping the database).
- `protect-files.sh` blocks edits to generated migrations, build output, `.git`, and secret files.
- `format-cs.sh` formats edited C# files.
- `build-on-stop.sh` makes sure the solution still builds before the agent hands back.

A `code-reviewer` subagent and the `/review` and `/verify` commands are available for reviews and fresh-clone verification.
