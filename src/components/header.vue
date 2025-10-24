<script setup>
import { defineProps, defineEmits } from 'vue'

const props = defineProps({
  isLoggedIn: {
    type: Boolean,
    default: false
  },
  isAdmin: {
    type: Boolean,
    default: false
  },
  userNickname: {
    type: String,
    default: ''
  }
})

const emit = defineEmits(['navigate', 'login', 'register', 'logout'])

const navigate = (view) => {
  emit('navigate', view)
}

const login = () => {
  emit('login')
}

const register = () => {
  emit('register')
}

const logout = () => {
  emit('logout')
}
</script>

<template>
  <header class="app-header glass">
    <div class="logo" @click="navigate('articles')">
      <svg class="logo-mark" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg" aria-hidden="true">
        <path d="M5 4h14a1 1 0 011 1v14a1 1 0 01-1 1H5a1 1 0 01-1-1V5a1 1 0 011-1z" stroke="currentColor" stroke-width="1.5" opacity=".5"/>
        <path d="M8 8h8M8 12h8M8 16h5" stroke="currentColor" stroke-width="1.5"/>
      </svg>
      <span class="logo-text brand-gradient">Quill</span>
    </div>
    <nav class="main-nav">
      <a href="#" class="nav-pill" @click.prevent="navigate('articles')">Главная</a>
      <a v-if="isLoggedIn" href="#" class="nav-pill" @click.prevent="navigate('profile')">Профиль</a>
      <a v-if="isAdmin" href="#" class="nav-pill" @click.prevent="navigate('admin')">Админ</a>
    </nav>
    <div class="auth-controls">
      <template v-if="!isLoggedIn">
        <button class="btn btn-secondary" @click="login">Войти</button>
        <button class="btn btn-primary" @click="register">Регистрация</button>
      </template>
      <template v-else>
        <span class="user-nickname pill">{{ userNickname }}</span>
        <button class="btn btn-ghost" @click="logout">Выйти</button>
      </template>
    </div>
  </header>
</template>

<style scoped>
.app-header {
  position: sticky;
  top: 0;
  z-index: 50;
  display: grid;
  grid-template-columns: 1fr auto auto;
  align-items: center;
  gap: 1rem;
  padding: 0.75rem 1.25rem;
}

.logo { display:flex; align-items:center; gap:.6rem; cursor:pointer; }
.logo-mark { width: 28px; height: 28px; color: var(--accent); }
.logo-text { font-size: 1.25rem; font-weight: 800; letter-spacing: .2px; }

.main-nav { display:flex; gap:.5rem; align-items:center; }
.nav-pill { padding:.45rem .8rem; border-radius: 999px; border:1px solid var(--glass-border); background: var(--surface); font-weight:600; }
.nav-pill:hover { transform: translateY(-1px); transition: transform .2s ease; }

.auth-controls { display:flex; align-items:center; gap:.6rem; }
.user-nickname { font-weight:600; }

@media (max-width: 768px) {
  .app-header { grid-template-columns: 1fr auto; }
  .main-nav { display:none; }
}
</style>