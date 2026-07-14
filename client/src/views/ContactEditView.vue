<script setup>
import { onMounted, ref } from 'vue'
import { useRouter } from 'vue-router'
import ContactForm from '../components/ContactForm.vue'
import { getById, update, ApiError } from '../api/contactsApi'
import { useToast } from '../composables/useToast'

const props = defineProps({ id: { type: [String, Number], required: true } })

const router = useRouter()
const toast = useToast()

const contact = ref(null)
const loading = ref(true)
const notFound = ref(false)
const submitting = ref(false)
const serverErrors = ref({})

onMounted(async () => {
  try {
    contact.value = await getById(props.id)
  } catch (err) {
    if (err instanceof ApiError && err.status === 404) {
      notFound.value = true
    } else {
      toast.error(err.message ?? 'Failed to load contact.')
    }
  } finally {
    loading.value = false
  }
})

async function handleSubmit(updatedContact) {
  submitting.value = true
  serverErrors.value = {}

  try {
    await update(props.id, updatedContact)
    toast.success('Contact saved.')
    router.push('/')
  } catch (err) {
    const hasFieldErrors = err instanceof ApiError && Object.keys(err.fieldErrors ?? {}).length > 0
    if (hasFieldErrors) {
      serverErrors.value = err.fieldErrors
    } else {
      toast.error(err.message ?? 'Failed to save contact.')
    }
  } finally {
    submitting.value = false
  }
}
</script>

<template>
  <router-link to="/" class="text-decoration-none small pb-muted d-inline-block mb-2">
    <i class="bi bi-arrow-left"></i> Back to contacts
  </router-link>
  <h2 class="pb-page-title mb-3"><i class="bi bi-pencil-fill me-2 text-primary"></i>Edit Contact</h2>

  <div v-if="loading" class="text-center py-5">
    <span class="spinner-border spinner-border-sm text-primary me-2" role="status" aria-hidden="true"></span>
    <span class="pb-muted">Loading...</span>
  </div>

  <div v-else-if="notFound" class="alert alert-warning d-flex align-items-center gap-2">
    <i class="bi bi-exclamation-triangle-fill"></i>
    <span>Contact not found. <router-link to="/">Back to list</router-link></span>
  </div>

  <div v-else class="pb-card p-4 pb-form-card">
    <ContactForm
      :model-value="contact"
      :submitting="submitting"
      :server-errors="serverErrors"
      @submit="handleSubmit"
    />
  </div>
</template>
