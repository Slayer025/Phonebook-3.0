# SHARED.md — Vue.js Front-End Migration (Phonebook App)

> **Read this file first. Then read only your assigned PART file. Do not touch scope belonging to other parts.**

## Goal
Replace all Razor views (`.cshtml`) with a Vue 3 single-page application. The backend becomes a pure JSON API. **The data-access layer is untouched**: pure async ADO.NET, stored procedures only, `IContactRepository`/`ContactRepository` remain exactly as they are.

## Target Architecture
- **Backend**: ASP.NET Core 8 Web API. `ContactsApiController` under route `api/contacts`. Returns JSON only. No views, no `_Layout.cshtml` after cleanup.
- **Frontend**: Vue 3 (Composition API, `<script setup>`), Vite, Vue Router 4, plain `fetch` (no axios). No Pinia — app is small; shared state via composables.
- **Styling**: Bootstrap 5.3 via CDN (upgrade from 3.4.1). **jQuery is removed entirely.**
- **Dev workflow**: Vite dev server on `http://localhost:5173` proxying `/api` to Kestrel (`http://localhost:5000`).
- **Prod workflow**: `npm run build` outputs to `../wwwroot/`; ASP.NET Core serves the SPA with `MapFallbackToFile("index.html")`.

## Directory Layout (after migration)
```
D:\PhonebookApp\
├── Controllers/ContactsApiController.cs
├── Models/ (Contact.cs, PagedResult.cs — unchanged)
├── Repositories/ (unchanged)
├── client/                  # Vue project root
│   ├── vite.config.js
│   ├── package.json
│   └── src/
│       ├── main.js
│       ├── App.vue
│       ├── router/index.js
│       ├── api/contactsApi.js
│       ├── composables/useContacts.js
│       ├── components/ (PaginationBar.vue, ConfirmDialog.vue, ToastNotice.vue)
│       └── views/ (ContactListView.vue, ContactCreateView.vue, ContactEditView.vue)
├── wwwroot/                 # Vite build output (generated; don't hand-edit)
├── Program.cs
└── appsettings.json
```

## API Contract (fixed — every part codes against this)
| Method | Route | Body / Query | Returns |
|---|---|---|---|
| GET | `/api/contacts?page=1&pageSize=10&search=` | query | `200` → `PagedResult<Contact>` JSON |
| GET | `/api/contacts/{id}` | — | `200` → `Contact`, `404` if missing |
| POST | `/api/contacts` | `Contact` JSON (no Id) | `201` + `Location` header + created `Contact`; `400` → validation problem details; `409` → duplicate phone |
| PUT | `/api/contacts/{id}` | `Contact` JSON | `204`; `400` / `404` / `409` as above |
| DELETE | `/api/contacts/{id}` | — | `204`; `404` if missing |

**JSON shapes** (camelCase — ASP.NET Core default):
```json
// PagedResult<Contact>
{ "items": [ ... ], "totalCount": 42, "currentPage": 1, "pageSize": 10, "totalPages": 5 }
// Contact
{ "id": 1, "name": "Asha", "phoneNumber": "+91 98...", "email": null, "address": null, "createdAt": "2026-07-14T09:00:00" }
```

**Validation errors** use ASP.NET Core's standard `ValidationProblemDetails` (`errors` dictionary keyed by field name, PascalCase keys). The frontend maps them to per-field messages.

## Non-Negotiable Constraints (inherited from CLAUDE.md)
1. No ORMs, no inline SQL, stored procedures only, `SqlParameter` always, `using` on all ADO objects, async ADO.NET only.
2. Pagination stays in the database (`sp_GetContactsPaged`). The API never loads all rows.
3. Client-side validation mirrors the `Contact` DataAnnotations but the **server remains the source of truth** — API always re-validates.
4. Phone number uniqueness is enforced by the DB unique constraint; API translates the SQL error (2601/2627) to HTTP 409.

## Coding Conventions
- Vue: Composition API with `<script setup>`, one component per file, props/emits typed via `defineProps`/`defineEmits`.
- Errors: every fetch goes through `api/contactsApi.js`, which throws a typed `ApiError { status, fieldErrors, message }`.
- No global CSS frameworks beyond Bootstrap CDN classes; custom styles are scoped in SFCs.
- Commit per part, message prefix `vue-migration(partN):`.

## Part Index, Dependencies, and Suggested Model/Effort
| Part | File | Depends on | Model | Effort |
|---|---|---|---|---|
| 1 | PART1-api-endpoints.md | — | Sonnet 4.6 | extended thinking |
| 2 | PART2-vue-scaffold.md | — (parallel with 1) | Sonnet 4.6 | standard |
| 3 | PART3-api-client.md | 1 contract, 2 | Sonnet 4.6 | standard |
| 4 | PART4-contacts-list.md | 3 | Sonnet 4.6 | standard |
| 5 | PART5-forms.md | 3 | Sonnet 4.6 | extended thinking |
| 6 | PART6-delete-ux.md | 3, 4 | Haiku 4.5 or Sonnet 4.6 | standard |
| 7 | PART7-cleanup-integration.md | all | Opus 4.8 (review-heavy) | extended thinking |

Parts 1 and 2 can run in parallel. Parts 4, 5, 6 can run in parallel once 3 is merged.

## Definition of Done (per part)
- Code compiles (`dotnet build`) / builds (`npm run build`) with zero warnings introduced.
- Manual smoke test steps in the part file pass.
- No changes outside the files listed in the part's Scope section.
