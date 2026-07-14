<script setup>
import ToastNotice from './components/ToastNotice.vue'
</script>

<template>
  <nav class="navbar navbar-expand navbar-dark sticky-top shadow-sm pb-navbar">
    <div class="container flex-wrap gap-2 py-2">
      <router-link class="navbar-brand d-flex align-items-center gap-2 mb-0" to="/">
        <i class="bi bi-journal-bookmark-fill"></i>
        Phonebook
      </router-link>
      <div class="d-flex gap-2">
        <router-link to="/" class="btn btn-outline-light btn-sm">
          <i class="bi bi-people-fill me-1"></i>Contacts
        </router-link>
        <router-link to="/create" class="btn btn-primary btn-sm">
          <i class="bi bi-person-plus-fill me-1"></i>Add Contact
        </router-link>
      </div>
    </div>
  </nav>

  <div class="container py-4 pb-5">
    <router-view />
  </div>

  <ToastNotice />
</template>
