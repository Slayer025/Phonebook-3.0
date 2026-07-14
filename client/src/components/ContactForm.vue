<script setup>
import { reactive, watch, computed } from 'vue'

const props = defineProps({
  modelValue: { type: Object, required: true },
  submitting: { type: Boolean, default: false },
  serverErrors: { type: Object, default: () => ({}) },
})

const emit = defineEmits(['submit'])

const PHONE_PATTERN = /^[0-9+\-() ]{7,50}$/
const EMAIL_PATTERN = /^[^\s@]+@[^\s@]+\.[^\s@]+$/

const form = reactive({
  name: '',
  phoneNumber: '',
  email: '',
  address: '',
})

const touched = reactive({ name: false, phoneNumber: false, email: false })
const clientErrors = reactive({ name: '', phoneNumber: '', email: '' })

watch(
  () => props.modelValue,
  (value) => {
    form.name = value?.name ?? ''
    form.phoneNumber = value?.phoneNumber ?? ''
    form.email = value?.email ?? ''
    form.address = value?.address ?? ''
  },
  { immediate: true, deep: true },
)

function validateField(field) {
  if (field === 'name') {
    if (!form.name.trim()) clientErrors.name = 'Name is required.'
    else if (form.name.length > 255) clientErrors.name = 'Name cannot exceed 255 characters.'
    else clientErrors.name = ''
  } else if (field === 'phoneNumber') {
    if (!form.phoneNumber.trim()) clientErrors.phoneNumber = 'Phone number is required.'
    else if (form.phoneNumber.length > 50) clientErrors.phoneNumber = 'Phone number cannot exceed 50 characters.'
    else if (!PHONE_PATTERN.test(form.phoneNumber)) clientErrors.phoneNumber = 'Enter a valid phone number.'
    else clientErrors.phoneNumber = ''
  } else if (field === 'email') {
    if (form.email && form.email.length > 255) clientErrors.email = 'Email cannot exceed 255 characters.'
    else if (form.email && !EMAIL_PATTERN.test(form.email)) clientErrors.email = 'Invalid email address.'
    else clientErrors.email = ''
  }
}

function onBlur(field) {
  touched[field] = true
  validateField(field)
}

function validateAll() {
  touched.name = true
  touched.phoneNumber = true
  touched.email = true
  validateField('name')
  validateField('phoneNumber')
  validateField('email')
  return !clientErrors.name && !clientErrors.phoneNumber && !clientErrors.email
}

const displayedErrors = computed(() => ({
  name: props.serverErrors.name || clientErrors.name,
  phoneNumber: props.serverErrors.phoneNumber || clientErrors.phoneNumber,
  email: props.serverErrors.email || clientErrors.email,
}))

function onSubmit() {
  if (!validateAll()) return
  emit('submit', { ...props.modelValue, ...form })
}
</script>

<template>
  <form novalidate @submit.prevent="onSubmit">
    <div class="mb-3">
      <label class="form-label" for="contact-name">Name</label>
      <input
        id="contact-name"
        v-model="form.name"
        type="text"
        class="form-control"
        :class="{ 'is-invalid': displayedErrors.name }"
        maxlength="255"
        required
        @blur="onBlur('name')"
      />
      <div class="invalid-feedback">{{ displayedErrors.name }}</div>
    </div>

    <div class="mb-3">
      <label class="form-label" for="contact-phone">Phone Number</label>
      <input
        id="contact-phone"
        v-model="form.phoneNumber"
        type="text"
        class="form-control"
        :class="{ 'is-invalid': displayedErrors.phoneNumber }"
        maxlength="50"
        required
        @blur="onBlur('phoneNumber')"
      />
      <div class="invalid-feedback">{{ displayedErrors.phoneNumber }}</div>
    </div>

    <div class="mb-3">
      <label class="form-label" for="contact-email">Email</label>
      <input
        id="contact-email"
        v-model="form.email"
        type="email"
        class="form-control"
        :class="{ 'is-invalid': displayedErrors.email }"
        maxlength="255"
        @blur="onBlur('email')"
      />
      <div class="invalid-feedback">{{ displayedErrors.email }}</div>
    </div>

    <div class="mb-3">
      <label class="form-label" for="contact-address">Address</label>
      <textarea id="contact-address" v-model="form.address" class="form-control" rows="4"></textarea>
    </div>

    <div class="d-flex gap-2">
      <button type="submit" class="btn btn-primary" :disabled="submitting">
        <span v-if="submitting" class="spinner-border spinner-border-sm me-1" role="status" aria-hidden="true"></span>
        Save
      </button>
      <router-link to="/" class="btn btn-outline-secondary">Cancel</router-link>
    </div>
  </form>
</template>
