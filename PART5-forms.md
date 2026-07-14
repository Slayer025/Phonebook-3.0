# PART 5 — Create & Edit Forms with Validation

**Read SHARED.md first.** You replace `Create.cshtml` / `Edit.cshtml` and the old jQuery unobtrusive validation.

## Scope
- `client/src/views/ContactCreateView.vue` (replace placeholder)
- `client/src/views/ContactEditView.vue` (replace placeholder)
- `client/src/components/ContactForm.vue` (new — shared by both views)

## Tasks
1. `ContactForm.vue`:
   - Props: `modelValue` (contact object), `submitting`, `serverErrors` (object). Emits: `submit`.
   - Fields: Name (required, max 255), PhoneNumber (required, max 50, pattern `^[0-9+\-() ]{7,50}$`), Email (optional, email format, max 255), Address (optional, textarea).
   - Client-side validation runs on blur + on submit; rules mirror the `Contact` DataAnnotations. Show Bootstrap `is-invalid` + `invalid-feedback`.
   - Merge `serverErrors` (from `ApiError.fieldErrors`) into the displayed errors — server messages win.
2. `ContactCreateView.vue`: empty model → `contactsApi.create` → on success route to `/` (Part 6 adds the success toast; for now a plain redirect is fine). On `ApiError` pass `fieldErrors` down; 409 must appear under the PhoneNumber field.
3. `ContactEditView.vue`: load via `getById(route id)`; 404 → alert + link back to list. Submit via `update`; same error handling.
4. Disable the submit button and show a spinner while a request is in flight; prevent double submit.

## Constraints
- No jQuery / unobtrusive validation. No validation libraries (VeeValidate etc.) — hand-rolled rules keep the dependency surface zero.
- Server remains the source of truth; never suppress server errors in favor of client rules.

## Smoke Test
- Submit empty form → inline required errors, no network call.
- Duplicate phone → 409 message under the phone field.
- Edit non-existent id (`/edit/99999`) → not-found state.

## Model / Effort
Sonnet 4.6, extended thinking (two-source validation merging is the fiddly bit).
