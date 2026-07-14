# PART 4 — Contacts List View & Pagination

**Read SHARED.md first.** You replace the Razor `Index.cshtml` experience. Delete UX (dialogs/toasts) belongs to Part 6 — here the delete button only emits/calls the composable's stub.

## Scope
- `client/src/views/ContactListView.vue` (replace placeholder)
- `client/src/components/PaginationBar.vue` (new)

## Tasks
1. `ContactListView.vue`, driven entirely by `useContacts()`:
   - Search input (Bootstrap input group) wired to `setSearch` — the composable already debounces.
   - Responsive Bootstrap table: Name, Phone, Email, Address, Created (format via `Intl.DateTimeFormat`), Actions (Edit → router-link to `/edit/:id`; Delete button calls `removeContact(id)` directly for now — Part 6 adds confirmation).
   - Loading state: spinner row. Error state: dismissible alert.
   - Empty state: `No contacts found.` (must render when `items` is empty — parity with the old Razor rule).
   - "Add Contact" button → `/create`.
2. `PaginationBar.vue`:
   - Props: `currentPage`, `totalPages`. Emit: `page-change`.
   - Previous / page numbers / Next, Bootstrap `pagination` markup; `disabled` on Prev at page 1 and Next at last page; `active` on current page.
   - Window the numbers (max 7 buttons with ellipsis) so 500 pages don't render 500 buttons.
   - Hide entirely when `totalPages <= 1`.

## Constraints
- All pagination is server-driven — never slice `items` client-side.
- No jQuery, no direct fetch calls (composable only).

## Smoke Test
Seed 25+ rows. Verify: page navigation, search resets to page 1, deleting the last row of the final page navigates to the new last page, empty search result shows the empty state.

## Model / Effort
Sonnet 4.6, standard effort.
