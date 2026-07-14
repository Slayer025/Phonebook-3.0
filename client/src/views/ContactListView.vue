<script setup>
import { onMounted, ref } from 'vue'
import { useContacts } from '../composables/useContacts'
import { useToast } from '../composables/useToast'
import PaginationBar from '../components/PaginationBar.vue'
import ConfirmDialog from '../components/ConfirmDialog.vue'

const { items, totalCount, totalPages, currentPage, loading, error, load, setSearch, removeContact } =
  useContacts()
const toast = useToast()

const searchInput = ref('')
const confirmVisible = ref(false)
const pendingContact = ref(null)

function onSearchInput() {
  setSearch(searchInput.value)
}

function onPageChange(page) {
  load(page)
}

function askDelete(contact) {
  pendingContact.value = contact
  confirmVisible.value = true
}

async function confirmDelete() {
  const contact = pendingContact.value
  pendingContact.value = null
  if (!contact) return

  try {
    await removeContact(contact.id)
    toast.success(`${contact.name} was deleted.`)
  } catch (err) {
    toast.error(err.message ?? 'Failed to delete contact.')
  }
}

function cancelDelete() {
  pendingContact.value = null
}

function formatDate(value) {
  if (!value) return ''
  return new Intl.DateTimeFormat('en-US', { dateStyle: 'medium' }).format(new Date(value))
}

function initials(name) {
  if (!name) return '?'
  const parts = name.trim().split(/\s+/)
  const first = parts[0]?.[0] ?? ''
  const last = parts.length > 1 ? parts[parts.length - 1][0] : ''
  return (first + last).toUpperCase()
}

function avatarStyle(name) {
  let hash = 0
  for (const char of name ?? '') {
    hash = (hash * 31 + char.charCodeAt(0)) & 0xffffffff
  }
  const hue = Math.abs(hash) % 360
  const hue2 = (hue + 45) % 360
  return {
    background: `linear-gradient(135deg, hsl(${hue}, 75%, 55%), hsl(${hue2}, 75%, 45%))`,
  }
}

onMounted(() => {
  load(1)
})
</script>

<template>
  <div class="d-flex justify-content-between align-items-center flex-wrap gap-2 mb-3">
    <h2 class="pb-page-title mb-0">
      Contacts
      <span v-if="totalCount" class="badge rounded-pill pb-count-badge ms-2 align-middle fs-6">
        {{ totalCount }}
      </span>
    </h2>
    <router-link to="/create" class="btn btn-primary">
      <i class="bi bi-person-plus-fill me-1"></i>Add Contact
    </router-link>
  </div>

  <div class="pb-card p-3 p-md-4">
    <div class="input-group mb-3" style="max-width: 420px;">
      <span class="input-group-text border-end-0"><i class="bi bi-search"></i></span>
      <input
        v-model="searchInput"
        type="text"
        class="form-control border-start-0"
        placeholder="Search by name or phone..."
        @input="onSearchInput"
      />
    </div>

    <div v-if="error" class="alert alert-danger alert-dismissible fade show" role="alert">
      {{ error.message ?? 'Failed to load contacts.' }}
      <button type="button" class="btn-close" aria-label="Close" @click="error = null"></button>
    </div>

    <div class="table-responsive">
      <table class="pb-table table align-middle mb-0">
        <thead>
          <tr>
            <th>Contact</th>
            <th>Phone</th>
            <th>Email</th>
            <th>Address</th>
            <th>Created</th>
            <th class="text-end">Actions</th>
          </tr>
        </thead>
        <tbody>
          <tr v-if="loading">
            <td colspan="6" class="text-center py-5">
              <span class="spinner-border spinner-border-sm text-primary me-2" role="status" aria-hidden="true"></span>
              <span class="pb-muted">Loading contacts...</span>
            </td>
          </tr>
          <tr v-else-if="!items.length">
            <td colspan="6">
              <div class="pb-empty">
                <i class="bi bi-person-lines-fill d-block mb-2"></i>
                <div class="fw-semibold">No contacts found.</div>
                <div class="small">Try a different search, or add a new contact.</div>
              </div>
            </td>
          </tr>
          <tr v-for="contact in items" v-else :key="contact.id">
            <td>
              <div class="d-flex align-items-center gap-2">
                <div class="pb-avatar" :style="avatarStyle(contact.name)">{{ initials(contact.name) }}</div>
                <span class="fw-semibold">{{ contact.name }}</span>
              </div>
            </td>
            <td>{{ contact.phoneNumber }}</td>
            <td class="pb-muted">{{ contact.email || '—' }}</td>
            <td class="pb-muted">{{ contact.address || '—' }}</td>
            <td class="pb-muted">{{ formatDate(contact.createdAt) }}</td>
            <td>
              <div class="d-flex gap-2 justify-content-end">
                <router-link
                  :to="`/edit/${contact.id}`"
                  class="btn btn-sm btn-outline-primary pb-icon-btn"
                  title="Edit"
                >
                  <i class="bi bi-pencil-fill"></i>
                </router-link>
                <button
                  type="button"
                  class="btn btn-sm btn-outline-danger pb-icon-btn"
                  title="Delete"
                  @click="askDelete(contact)"
                >
                  <i class="bi bi-trash-fill"></i>
                </button>
              </div>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <div class="mt-3">
      <PaginationBar :current-page="currentPage" :total-pages="totalPages" @page-change="onPageChange" />
    </div>
  </div>

  <ConfirmDialog
    v-model="confirmVisible"
    title="Delete contact"
    :message="pendingContact ? `Delete ${pendingContact.name}? This cannot be undone.` : ''"
    confirm-label="Delete"
    @confirm="confirmDelete"
    @cancel="cancelDelete"
  />
</template>
