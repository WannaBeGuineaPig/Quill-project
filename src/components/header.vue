<script setup>
import { defineProps, defineEmits, ref } from 'vue'

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

const showTooltip = ref(false)

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
        <div class="user-nickname-container">
          <span 
            class="user-nickname" 
            @mouseenter="showTooltip = true"
            @mouseleave="showTooltip = false"
          >
            {{ userNickname }}
          </span>
          <div v-if="showTooltip" class="tooltip">
            Ваш никнейм
          </div>
        </div>
        <button class="btn btn-ghost" @click="logout">
          <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none">
          <path d="M13.34 8.17C12.41 8.17 11.65 7.4 11.65 6.47C11.65 6.02178 11.8281 5.59193 12.145 5.27499C12.4619 4.95805 12.8918 4.78 13.34 4.78C14.28 4.78 15.04 5.54 15.04 6.47C15.04 7.4 14.28 8.17 13.34 8.17ZM10.3 19.93L4.37 18.75L4.71 17.05L8.86 17.9L10.21 11.04L8.69 11.64V14.5H7V10.54L11.4 8.67L12.07 8.59C12.67 8.59 13.17 8.93 13.5 9.44L14.36 10.79C15.04 12 16.39 12.82 18 12.82V14.5C16.14 14.5 14.44 13.67 13.34 12.4L12.84 14.94L14.61 16.63V23H12.92V17.9L11.14 16.21L10.3 19.93ZM21 23H19V3H6V16.11L4 15.69V1H21V23ZM6 23H4V19.78L6 20.2V23Z" fill="#9EA5A7"/>
          </svg>
        </button>
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
.nav-pill { padding:.35rem .9rem; border-radius: 999px; border:1px solid var(--glass-border); background: var(--glass-bg); font-weight:600; box-shadow: 0 5px 5px rgba(0, 0, 0, 0.100); }

.nav-pill:hover { transform: translateY(-1px); transition: transform .2s ease;}

.auth-controls { display:flex; align-items:center; gap:.6rem; }
.btn.btn-ghost { border: none; margin-left: -0.4rem; padding: 0.5rem 0.5rem; }
.btn.btn-ghost:hover { background: none; transform: translateY(-2px);}

.user-nickname-container { position: relative;}

.user-nickname { font-size: .9rem; cursor: pointer;}

.tooltip {
  position: absolute;
  top: 100%;
  left: 80%;
  transform: translateX(-50%);
  background: rgba(0, 0, 0, 0.299);
  color: var(--color-text);
  padding: 0.3rem 0.75rem;
  border-radius: 60px;
  font-size: 0.8rem;
  border: 5px solid transparent;
  white-space: nowrap;
  box-shadow: 0 5px 12px rgba(0, 0, 0, 0.500);
}

@media (max-width: 768px) {
  .app-header { grid-template-columns: 1fr auto; }
  .main-nav { display:none; }
}
</style>