---
description: Verify the repo works from a fresh clone
---
1. Clone this repo into a fresh temporary directory (uncommitted work is excluded on purpose).
2. In the clone: `dotnet tool restore`, `dotnet build`, `dotnet test`.
3. Start the API with the Mock profile and curl `/swagger/v1/swagger.json`, `/swagger/v2/swagger.json`,
   `/api/v1/products`, `/api/v2/products`. Stop it.
4. If Docker is available: start SQL Server with a fresh volume, run the SQL Server profile,
   confirm seeded data, then tear down.
5. Confirm no `.idea`, `bin` or `obj` files are tracked.
6. Follow the README steps literally and report any step that is wrong or missing.
Report results as a checklist. Fix nothing.
