<script setup>
import { computed } from 'vue'
import { useRouter } from 'vue-router'
import Header from './components/header.vue'
import Footer from './components/Footer.vue'
import { useAppState } from './composables/useAppState'

const router = useRouter()
const { isLoggedIn, isAdmin, logout, state } = useAppState()
const userNickname = computed(() => state.currentUser?.nickname || 'Гость')

const navigate = (view) => {
  const map = {
    articles: { name: 'home' },
    profile: { name: 'profile' },
    admin: { name: 'admin' },
  }
  router.push(map[view] || { name: 'home' })
}

const goLogin = () => router.push({ name: 'login' })
const goRegister = () => router.push({ name: 'register' })
const doLogout = () => { logout(); router.push({ name: 'home' }) }
</script>

<template>
  <div class="app-container">
    <Header 
      :isLoggedIn="isLoggedIn"
      :isAdmin="isAdmin"
      :userNickname="userNickname"
      @navigate="navigate"
      @login="goLogin"
      @register="goRegister"
      @logout="doLogout"
    />
    <main class="main-content container">
      <router-view />
    </main>
    <Footer />
  </div>
</template>

<style>
.app-container { display: flex; flex-direction: column; min-height: 100vh; width: 100%; }
.main-content { flex: 1; }
</style>