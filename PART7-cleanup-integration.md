# PART 7 — Razor Removal, Cleanup & Final Integration

**Read SHARED.md first.** All other parts are merged. You delete the legacy front-end, tighten `Program.cs`, and verify the whole system end-to-end. This is the only part allowed to delete files.

## Scope
- Delete: `Views/` (all `.cshtml`), `Controllers/HomeController.cs`
- `Program.cs`, `PhonebookApp.csproj`, `CLAUDE.md`, `.vscode/launch.json`
- `wwwroot/site.css` (delete if unused, or port needed rules into a Vue scoped style / `client/src/assets`)

## Tasks
1. Remove Razor: delete `Views/` and `HomeController.cs`. In `Program.cs` drop `AddControllersWithViews()` → `AddControllers()`; remove `MapControllerRoute` default MVC route; keep `MapControllers()` + `UseStaticFiles()` + `MapFallbackToFile("index.html")` in that order.
2. Sweep the csproj for now-unneeded Razor-related items; ensure `dotnet publish` still includes `wwwroot/`.
3. Add a `client/README` note + root build note: prod build = `npm run build` (in `client/`) then `dotnet publish`.
4. Update `CLAUDE.md`: frontend section now describes Vue 3 SPA + JSON API; remove Razor/jQuery/Bootstrap 3 guidance; add the API contract table from SHARED.md; keep all data-access prohibitions verbatim.
5. Full regression pass (checklist below). Fix integration bugs found; if a fix belongs to another part's files, keep the diff minimal and note it in the commit message.

## Regression Checklist
- [ ] Fresh clone flow: `npm ci && npm run build` in `client/`, `dotnet run` at root, app fully works at :5000 with no Vite dev server.
- [ ] Deep links (`/create`, `/edit/3`) load directly (SPA fallback).
- [ ] CRUD round-trip; duplicate phone 409 surfaces on the form; delete confirmation + toast.
- [ ] Pagination + search against 25+ rows; DB-level paging confirmed (check SQL Profiler or just trust the untouched repository).
- [ ] `grep -ri jquery` in the repo returns nothing outside `PhonebookApp_NET45_Backup`.
- [ ] `dotnet build` zero warnings introduced; `npm run build` clean.

## Model / Effort
Opus 4.8 (or Fable 5), extended thinking — this part is judgment-heavy: safe deletion, pipeline ordering, doc rewrite, and cross-part bug triage.
