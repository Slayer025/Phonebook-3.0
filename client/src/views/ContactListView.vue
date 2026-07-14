<script setup>
import { onMounted, ref } from 'vue'
import { useContacts } from '../composables/useContacts'
import { useToast } from '../composables/useToast'
import PaginationBar from '../components/PaginationBar.vue'
import ConfirmDialog from '../components/ConfirmDialog.vue'

const { items, totalPages, currentPage, loading, error, load, setSearch, removeContact } = useContacts()
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

onMounted(() => {
  load(1)
})
</script>

<template>
  <h2 class="mb-3">Contacts</h2>

  <div class="input-group mb-3" style="max-width: 420px;">
    <span class="input-group-text">Search</span>
    <input
      v-model="searchInput"
      type="text"
      class="form-control"
      placeholder="Search by name or phone..."
      @input="onSearchInput"
    />
  </div>

  <div v-if="error" class="alert alert-danger alert-dismissible fade show" role="alert">
    {{ error.message ?? 'Failed to load contacts.' }}
    <button type="button" class="btn-close" aria-label="Close" @click="error = null"></button>
  </div>

  <div class="table-responsive">
    <table class="table table-striped align-middle">
      <thead>
        <tr>
          <th>Name</th>
          <th>Phone</th>
          <th>Email</th>
          <th>Address</th>
          <th>Created</th>
          <th>Actions</th>
        </tr>
      </thead>
      <tbody>
        <tr v-if="loading">
          <td colspan="6" class="text-center py-4">
            <span class="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span>
            Loading...
          </td>
        </tr>
        <tr v-else-if="!items.length">
          <td colspan="6" class="text-center py-4 text-muted">No contacts found.</td>
        </tr>
        <tr v-for="contact in items" v-else :key="contact.id">
          <td>{{ contact.name }}</td>
          <td>{{ contact.phoneNumber }}</td>
          <td>{{ contact.email }}</td>
          <td>{{ contact.address }}</td>
          <td>{{ formatDate(contact.createdAt) }}</td>
          <td>
            <div class="d-flex gap-2">
              <router-link :to="`/edit/${contact.id}`" class="btn btn-sm btn-outline-primary">Edit</router-link>
              <button type="button" class="btn btn-sm btn-outline-danger" @click="askDelete(contact)">
                Delete
              </button>
            </div>
          </td>
        </tr>
      </tbody>
    </table>
  </div>

  <PaginationBar :current-page="currentPage" :total-pages="totalPages" @page-change="onPageChange" />

  <ConfirmDialog
    v-model="confirmVisible"
    title="Delete contact"
    :message="pendingContact ? `Delete ${pendingContact.name}? This cannot be undone.` : ''"
    confirm-label="Delete"
    @confirm="confirmDelete"
    @cancel="cancelDelete"
  />
</template>
