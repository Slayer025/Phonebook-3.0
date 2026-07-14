# PART 2 — Vue Project Scaffold & Build Integration

**Read SHARED.md first.** You set up the Vue project shell and the dev/prod build pipeline. You do NOT implement any real views, API calls, or components beyond placeholders.

## Scope
- `client/` (entire folder — new)
- `Program.cs` (static files + SPA fallback only)
- `.vscode/tasks.json` (optional: add `npm run dev` / `npm run build` tasks)

## Tasks
1. Scaffold with Vite: Vue 3, JavaScript (no TypeScript), no ESLint/Prettier prompts needed. Project root: `client/`.
2. `vite.config.js`:
   - `build.outDir: '../wwwroot'`, `emptyOutDir: true`
   - dev server proxy: `/api` → `http://localhost:5000`
3. `index.html`: add Bootstrap 5.3 CSS + JS bundle via CDN (no jQuery).
4. `src/router/index.js` with routes:
   - `/` → `ContactListView`
   - `/create` → `ContactCreateView`
   - `/edit/:id` → `ContactEditView` (props: true)
   Use `createWebHistory()`.
5. Placeholder view components: each renders just its name in an `<h2>`.
6. `App.vue`: Bootstrap navbar ("Phonebook", link to Home and Add Contact) + `<router-view/>`.
7. `Program.cs`: add `app.UseStaticFiles()` (if absent) and `app.MapFallbackToFile("index.html")` **after** API routes. Do not remove MVC/Razor registration — that is Part 7.
8. Verify prod path: `npm run build`, then `dotnet run` serves the SPA at `http://localhost:5000/`.

## Constraints
- No axios, no Pinia, no UI kit beyond Bootstrap CDN.
- Do not create `api/contactsApi.js` or composables (Part 3).

## Smoke Test
- `npm run dev` → app loads at :5173, navbar routes switch views without full reload.
- `npm run build` → files land in `wwwroot/`; `dotnet run` serves SPA; deep link `http://localhost:5000/create` works (fallback).

## Model / Effort
Sonnet 4.6, standard effort.
