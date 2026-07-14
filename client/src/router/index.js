import { createRouter, createWebHistory } from 'vue-router'
import ContactListView from '../views/ContactListView.vue'
import ContactCreateView from '../views/ContactCreateView.vue'
import ContactEditView from '../views/ContactEditView.vue'

const router = createRouter({
  history: createWebHistory(),
  routes: [
    { path: '/', name: 'ContactList', component: ContactListView },
    { path: '/create', name: 'CreateContact', component: ContactCreateView },
    { path: '/edit/:id', name: 'EditContact', component: ContactEditView, props: true },
  ],
})

export default router
