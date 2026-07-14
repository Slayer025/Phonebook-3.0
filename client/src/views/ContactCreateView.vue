<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import ContactForm from '../components/ContactForm.vue'
import { create, ApiError } from '../api/contactsApi'
import { useToast } from '../composables/useToast'

const router = useRouter()
const toast = useToast()

const emptyContact = { name: '', phoneNumber: '', email: '', address: '' }
const submitting = ref(false)
const serverErrors = ref({})

async function handleSubmit(contact) {
  submitting.value = true
  serverErrors.value = {}

  try {
    await create(contact)
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
  <h2 class="mb-3">Create Contact</h2>
  <ContactForm
    :model-value="emptyContact"
    :submitting="submitting"
    :server-errors="serverErrors"
    @submit="handleSubmit"
  />
</template>
