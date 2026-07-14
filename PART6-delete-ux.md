# PART 6 — Delete Confirmation & Notifications

**Read SHARED.md first.** You replace the old jQuery confirm-and-AJAX-delete flow with Vue-native UX, and add app-wide toasts.

## Scope
- `client/src/components/ConfirmDialog.vue` (new)
- `client/src/components/ToastNotice.vue` (new)
- `client/src/composables/useToast.js` (new)
- `client/src/views/ContactListView.vue` (wire dialog into existing delete button — minimal diff)
- `client/src/views/ContactCreateView.vue` / `ContactEditView.vue` (add success toast on save — minimal diff)
- `client/src/App.vue` (mount toast container)

## Tasks
1. `ConfirmDialog.vue`: promise-based or v-model modal (Bootstrap modal markup, but controlled by Vue — do not use Bootstrap's jQuery-era JS API; the vanilla `bootstrap.Modal` from the CDN bundle is fine, or pure Vue with a backdrop div). Props: title, message, confirm/cancel labels. Emits `confirm` / `cancel`.
2. `useToast.js`: module-level reactive queue; `toast.success(msg)`, `toast.error(msg)`; auto-dismiss 4 s.
3. `ToastNotice.vue`: renders the queue top-right, Bootstrap toast styling.
4. List view: Delete → dialog ("Delete {name}? This cannot be undone.") → on confirm `removeContact(id)` → success toast; `ApiError` → error toast.
5. Create/Edit: success toast ("Contact saved") after redirect.

## Constraints
- No jQuery. No new npm dependencies.
- Don't restructure the views — additive changes only.

## Smoke Test
Delete flow shows dialog, cancel does nothing, confirm removes row and toasts; killing the API mid-delete shows an error toast.

## Model / Effort
Haiku 4.5 is sufficient (well-bounded, pattern-heavy); use Sonnet 4.6 if the promise-based dialog wiring gives trouble. Standard effort.
