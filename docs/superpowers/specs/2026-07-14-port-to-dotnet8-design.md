# Port PhonebookApp from .NET Framework 4.5 to .NET 8.0

**Date:** 2026-07-14
**Status:** Approved

## Context

PhonebookApp is an ASP.NET MVC 5.2.7 app targeting .NET Framework 4.5, using pure ADO.NET
against SQL Server Express via 5 stored procedures (`sp_GetContactsPaged`, `sp_GetContactById`,
`sp_InsertContact`, `sp_UpdateContact`, `sp_DeleteContact`). The existing `CLAUDE.md` hard-pins
this stack and states deviation is a failure. This spec documents an intentional, user-approved
migration to .NET 8, after which `CLAUDE.md` will be rewritten to describe the new stack while
keeping the ADO.NET / no-ORM / no-inline-SQL / stored-procedure / server-side-pagination rules.

## Decisions

- **Data access:** Keep pure ADO.NET + the same 5 stored procedures, same schema, same DB
  (`PhonebookDb` on SQL Server Express). Switch `System.Data.SqlClient` → `Microsoft.Data.SqlClient`.
  All repository methods become `async Task<T>` (`ExecuteReaderAsync`, `ExecuteNonQueryAsync`,
  `ExecuteScalarAsync`), all still `SqlParameter`-bound and `using`-wrapped.
- **Web framework:** ASP.NET Core MVC with Razor views (not Razor Pages, not Minimal API) —
  closest structural match to the existing Controllers/Views split.
- **DI:** `IContactRepository` → `ContactRepository` registered as a scoped service in
  `Program.cs`, injected into `HomeController` via constructor (replacing `new ContactRepository()`).
- **Static assets:** Bootstrap 3.4.1 / jQuery 3.6.0 / jQuery Validate / Unobtrusive Validation
  stay on CDN links in `_Layout.cshtml` (already mostly CDN-based); no `wwwroot` asset copying
  needed except `Content/site.css` → `wwwroot/site.css`.
- **Config:** `Web.config` connection string → `appsettings.json` `ConnectionStrings:PhonebookDbConnection`.
- **Validation:** `DataAnnotations` on `Contact.cs`/`PagedResult.cs` unchanged — fully compatible
  with ASP.NET Core.

## Project structure changes

| Old (.NET Framework 4.5) | New (.NET 8) |
|---|---|
| `PhonebookApp.csproj` (non-SDK) | SDK-style `.csproj`, `TargetFramework=net8.0`, `PackageReference` for `Microsoft.Data.SqlClient` |
| `Global.asax` / `Global.asax.cs` | `Program.cs` (minimal hosting model) |
| `Web.config` | `appsettings.json` |
| `packages.config` / `packages/` | `PackageReference` items, NuGet global cache |
| `Views/Web.config` | removed (not used in ASP.NET Core) |
| `bin/`, `obj/` | removed, regenerated on build |
| `App_Data/` | removed (confirmed empty) |
| `Content/site.css` | `wwwroot/site.css` |
| (none) | `Views/_ViewImports.cshtml` (tag helpers) |

`Controllers/`, `Models/`, `Repositories/`, `Views/*.cshtml` are ported in place, not replaced.

## Order of operations

1. Backup entire project to `D:\PhonebookApp_NET45_Backup`.
2. Scaffold new `.csproj`, `appsettings.json`, `Program.cs` (before deleting anything).
3. Delete old .NET Framework-specific files/folders once replacements exist.
4. Port `Models/`, `Repositories/`, `Controllers/`, `Views/` incrementally, building after each.
5. Rewrite `CLAUDE.md`'s Architecture & Tech Stack section for .NET 8, keeping ADO.NET/SP/security rules.
6. Verify: `dotnet restore`, `dotnet build`, `dotnet run`, exercise CRUD + pagination + search against `PhonebookDb`.

## Out of scope

- No EF Core, no ORM.
- No change to the database schema or stored procedures.
- No new features — behavior parity with the .NET Framework 4.5 version.
