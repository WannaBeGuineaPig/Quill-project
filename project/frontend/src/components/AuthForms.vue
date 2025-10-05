<script setup>
import { ref } from 'vue'

const props = defineProps({
  formType: {
    type: String,
    default: 'login', // 'login' или 'register'
    validator: (value) => ['login', 'register'].includes(value)
  }
})

const emit = defineEmits(['switch-form', 'login', 'register'])

const loginData = ref({
  email: '',
  password: ''
})

const registerData = ref({
  email: '',
  nickname: '',
  password: '',
  confirmPassword: ''
})

const submitLogin = () => {
  emit('login', loginData.value)
}

const submitRegister = () => {
  emit('register', registerData.value)
}

const switchForm = () => {
  emit('switch-form', props.formType === 'login' ? 'register' : 'login')
}
</script>

<template>
  <section class="auth-form card">
    <h2 class="card-title brand-gradient">{{ formType === 'login' ? 'Вход в систему' : 'Регистрация' }}</h2>
    
    <form v-if="formType === 'login'" class="stack" @submit.prevent="submitLogin">
      <div class="form-group">
        <label for="email">Email</label>
        <input type="email" id="email" class="input" v-model="loginData.email" required />
      </div>
      <div class="form-group">
        <label for="password">Пароль</label>
        <input type="password" id="password" class="input" v-model="loginData.password" required />
      </div>
      <button type="submit" class="btn btn-primary">Войти</button>
    </form>
    
    <form v-else class="stack" @submit.prevent="submitRegister">
      <div class="form-group">
        <label for="reg-email">Email</label>
        <input type="email" id="reg-email" class="input" v-model="registerData.email" required />
      </div>
      <div class="form-group">
        <label for="nickname">Никнейм</label>
        <input type="text" id="nickname" class="input" v-model="registerData.nickname" required />
      </div>
      <div class="form-group">
        <label for="reg-password">Пароль</label>
        <input type="password" id="reg-password" class="input" v-model="registerData.password" required />
      </div>
      <div class="form-group">
        <label for="confirm-password">Подтвердите пароль</label>
        <input type="password" id="confirm-password" class="input" v-model="registerData.confirmPassword" required />
      </div>
      <button type="submit" class="btn btn-primary">Зарегистрироваться</button>
    </form>
    
    <p class="switch-hint">
      {{ formType === 'login' ? 'Нет аккаунта?' : 'Уже есть аккаунт?' }}
      <a href="#" @click.prevent="switchForm">
        {{ formType === 'login' ? 'Зарегистрироваться' : 'Войти' }}
      </a>
    </p>
  </section>
</template>

<style scoped>
.auth-form { max-width: 560px; width: 100%; margin: 0 auto; }
.auth-form h2 { margin-bottom: 1rem; text-align: center; }
.form-group { display:flex; flex-direction:column; gap:.5rem; }
.auth-form button { width: 100%; padding: 0.85rem; margin-top: .5rem; }
.switch-hint { text-align: center; margin-top: 1rem; }
@media (max-width: 576px) { .auth-form { padding: 1rem; } }
</style>