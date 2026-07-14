<script setup>
defineProps({
  modelValue: { type: Boolean, default: false },
  title: { type: String, default: 'Confirm' },
  message: { type: String, default: 'Are you sure?' },
  confirmLabel: { type: String, default: 'Confirm' },
  cancelLabel: { type: String, default: 'Cancel' },
})

const emit = defineEmits(['update:modelValue', 'confirm', 'cancel'])

function onConfirm() {
  emit('update:modelValue', false)
  emit('confirm')
}

function onCancel() {
  emit('update:modelValue', false)
  emit('cancel')
}
</script>

<template>
  <Teleport to="body">
    <div v-if="modelValue" class="modal-backdrop fade show"></div>
    <div v-if="modelValue" class="modal fade show d-block" tabindex="-1" role="dialog" aria-modal="true">
      <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title d-flex align-items-center gap-2">
              <i class="bi bi-exclamation-triangle-fill text-danger"></i>{{ title }}
            </h5>
            <button type="button" class="btn-close" aria-label="Close" @click="onCancel"></button>
          </div>
          <div class="modal-body">
            <p class="mb-0">{{ message }}</p>
          </div>
          <div class="modal-footer">
            <button type="button" class="btn btn-outline-secondary" @click="onCancel">{{ cancelLabel }}</button>
            <button type="button" class="btn btn-danger" @click="onConfirm">
              <i class="bi bi-trash-fill me-1"></i>{{ confirmLabel }}
            </button>
          </div>
        </div>
      </div>
    </div>
  </Teleport>
</template>
