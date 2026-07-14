# PART 3 — API Client & Contacts Composable

**Read SHARED.md first.** You build the single fetch layer every view will use. No UI work.

## Scope
- `client/src/api/contactsApi.js` (new)
- `client/src/composables/useContacts.js` (new)

## Tasks
1. `contactsApi.js` — thin wrapper over `fetch`, base path `/api/contacts`:
   - `getPaged({ page, pageSize, search })` → `PagedResult`
   - `getById(id)` → `Contact`
   - `create(contact)` → created `Contact`
   - `update(id, contact)` → `void`
   - `remove(id)` → `void`
2. Central response handler:
   - `2xx` → parse JSON (or return undefined for 204).
   - `400` with `errors` dict → throw `ApiError` with `fieldErrors` mapped to camelCase keys (`Name` → `name`).
   - `404` → `ApiError { status: 404, message: 'Contact not found.' }`
   - `409` → `ApiError { status: 409, fieldErrors: { phoneNumber: 'Phone number already exists.' } }`
   - anything else → generic `ApiError`.
3. Define and export `class ApiError extends Error { status; fieldErrors; }`.
4. `useContacts.js` — composable holding list state:
   - refs: `items`, `totalCount`, `totalPages`, `currentPage`, `pageSize` (default 10), `search`, `loading`, `error`
   - `load(page)` — calls `getPaged`, guards against out-of-range page (clamp to `totalPages` after deletes).
   - `setSearch(term)` — resets to page 1 then loads. Debounce (300 ms) lives here, not in components.
   - `removeContact(id)` — calls `remove`, then reloads current page (clamping if the page became empty).

## Constraints
- No component code, no router imports, no direct DOM access.
- No external HTTP libs.

## Smoke Test
With Part 1's API running via the Vite proxy, exercise from the browser console or a scratch component:
`getPaged({page:1,pageSize:5})` resolves; `create({})` rejects with `ApiError.fieldErrors.name` populated.

## Model / Effort
Sonnet 4.6, standard effort.
