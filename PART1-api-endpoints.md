# PART 1 — Backend JSON API

**Read SHARED.md first.** You are converting the server to a JSON API. You do NOT touch anything under `client/`, `Views/`, or `wwwroot/`.

## Scope (files you may create/edit)
- `Controllers/ContactsApiController.cs` (new)
- `Program.cs` (add `AddControllers()` + `MapControllers()`; keep existing MVC pipeline working for now — Razor removal is Part 7)
- `Models/Contact.cs` (only if annotations need adjusting for JSON binding — prefer no changes)

## Tasks
1. Implement `ContactsApiController` (`[ApiController]`, `[Route("api/contacts")]`) with the five actions in the SHARED.md contract, delegating to `IContactRepository` via constructor injection.
2. GET list: bind `page` (default 1, min 1), `pageSize` (default 10, clamp 1–100), `search` (nullable, trim, empty → null). Return `PagedResult<Contact>`.
3. POST: on `ModelState` invalid return `ValidationProblem()`. On success return `CreatedAtAction` pointing at the GET-by-id action with the new id from `sp_InsertContact`'s `@NewId` output.
4. PUT: 404 if the repository reports no row affected; 204 on success.
5. Duplicate phone handling: catch `SqlException` where `Number` is 2601 or 2627 and return `Conflict(new { message = "Phone number already exists." })`. Do this in the controller (or a small shared helper in the same file) — do not modify the repository.
6. DELETE: 204 on success, 404 if no row affected. If `sp_DeleteContact` / repository doesn't report rows affected, extend the repository method's return type to `Task<bool>` — this is the only permitted repository change, and only if needed.

## Constraints
- Repository stays pure async ADO.NET / stored procedures. No EF, no Dapper, no inline SQL.
- No `.Result` / `.Wait()`; all actions `async Task<IActionResult>`.

## Smoke Test
```
dotnet run
curl http://localhost:5000/api/contacts?page=1&pageSize=5
curl -X POST http://localhost:5000/api/contacts -H "Content-Type: application/json" -d "{\"name\":\"Test\",\"phoneNumber\":\"9999912345\"}"
# repeat the POST → expect 409
curl -X DELETE http://localhost:5000/api/contacts/{newId} → 204
```

## Model / Effort
Sonnet 4.6, extended thinking (error-mapping and status-code edge cases benefit from it).
