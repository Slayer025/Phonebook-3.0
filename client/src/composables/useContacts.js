import { ref } from 'vue'
import { getPaged, remove as removeApi } from '../api/contactsApi'

const DEBOUNCE_MS = 300

export function useContacts() {
  const items = ref([])
  const totalCount = ref(0)
  const totalPages = ref(0)
  const currentPage = ref(1)
  const pageSize = ref(10)
  const search = ref('')
  const loading = ref(false)
  const error = ref(null)

  let debounceTimer = null

  async function load(page = currentPage.value) {
    loading.value = true
    error.value = null

    try {
      let requestedPage = Math.max(page, 1)
      let result = await getPaged({ page: requestedPage, pageSize: pageSize.value, search: search.value })

      // If the requested page no longer exists (e.g. the last row on the
      // last page was just deleted), fall back to the new last page.
      if (result.totalPages > 0 && requestedPage > result.totalPages) {
        requestedPage = result.totalPages
        result = await getPaged({ page: requestedPage, pageSize: pageSize.value, search: search.value })
      }

      items.value = result.items
      totalCount.value = result.totalCount
      totalPages.value = result.totalPages
      currentPage.value = result.currentPage
    } catch (err) {
      error.value = err
    } finally {
      loading.value = false
    }
  }

  function setSearch(term) {
    search.value = term

    if (debounceTimer) {
      clearTimeout(debounceTimer)
    }

    debounceTimer = setTimeout(() => {
      load(1)
    }, DEBOUNCE_MS)
  }

  async function removeContact(id) {
    // Intentionally no try/catch: delete failures propagate to the caller
    // (list-load failures instead set `error` for an inline alert).
    await removeApi(id)
    await load(currentPage.value)
  }

  return {
    items,
    totalCount,
    totalPages,
    currentPage,
    pageSize,
    search,
    loading,
    error,
    load,
    setSearch,
    removeContact,
  }
}
