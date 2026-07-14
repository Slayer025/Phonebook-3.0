<script setup>
import ToastNotice from './components/ToastNotice.vue'
</script>

<template>
  <nav class="navbar navbar-expand navbar-dark bg-dark mb-4">
    <div class="container flex-wrap gap-2">
      <router-link class="navbar-brand mb-0" to="/">Phonebook</router-link>
      <div class="d-flex gap-2">
        <router-link to="/" class="btn btn-outline-light btn-sm">Home</router-link>
        <router-link to="/create" class="btn btn-primary btn-sm">Add Contact</router-link>
      </div>
    </div>
  </nav>

  <div class="container pb-5">
    <router-view />
  </div>

  <ToastNotice />
</template>
