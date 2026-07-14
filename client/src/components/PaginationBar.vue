<script setup>
import { computed } from 'vue'

const props = defineProps({
  currentPage: { type: Number, required: true },
  totalPages: { type: Number, required: true },
})

const emit = defineEmits(['page-change'])

const pageItems = computed(() => {
  const total = props.totalPages
  const current = props.currentPage
  const delta = 1

  const range = []
  for (let i = 1; i <= total; i++) {
    if (i === 1 || i === total || (i >= current - delta && i <= current + delta)) {
      range.push(i)
    }
  }

  const withEllipsis = []
  let previous = 0
  for (const page of range) {
    if (previous && page - previous > 1) {
      withEllipsis.push('...')
    }
    withEllipsis.push(page)
    previous = page
  }

  return withEllipsis
})

function go(page) {
  if (page < 1 || page > props.totalPages || page === props.currentPage) {
    return
  }
  emit('page-change', page)
}
</script>

<template>
  <nav v-if="totalPages > 1" aria-label="Contacts pagination">
    <ul class="pagination justify-content-center flex-wrap">
      <li class="page-item" :class="{ disabled: currentPage === 1 }">
        <button type="button" class="page-link" :disabled="currentPage === 1" @click="go(currentPage - 1)">
          Previous
        </button>
      </li>

      <li
        v-for="(item, index) in pageItems"
        :key="index"
        class="page-item"
        :class="{ active: item === currentPage, disabled: item === '...' }"
      >
        <span v-if="item === '...'" class="page-link">&hellip;</span>
        <button v-else type="button" class="page-link" @click="go(item)">{{ item }}</button>
      </li>

      <li class="page-item" :class="{ disabled: currentPage === totalPages }">
        <button type="button" class="page-link" :disabled="currentPage === totalPages" @click="go(currentPage + 1)">
          Next
        </button>
      </li>
    </ul>
  </nav>
</template>
