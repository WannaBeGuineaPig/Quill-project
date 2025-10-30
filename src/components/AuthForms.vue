<script setup>
import { ref } from 'vue'
import { useAppState } from '@/composables/useAppState'

const {register} = useAppState()

const props = defineProps({
  formType: {
    type: String,
    default: 'login', // 'login' или 'register'
    validator: (value) => ['login', 'register'].includes(value)
  }
})

const emit = defineEmits(['switch-form', 'login', 'register'])

const message = ref('')
const messageType = ref('')

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


const showLoginPassword = ref(false)

const showRegisterPassword = ref(false)
const showConfirmPassword = ref(false)

const submitLogin = async () => {
 
  const enterData ={
    email:loginData.value.email,
    password:loginData.value.password
  }
  try{
    console.log('Отправка данных:', enterData)
    // const  result = await login(enterData)
    // console.log('Получен результат:', result)
    // message.value = String(result.message)
    emit('login', enterData)
  
    
  }
  catch(error)
  {
    alert(error)
  }


}



const submitRegister = async () => {
  //  if (!passwordsMatch.value) {
  //   message.value = 'Пароли не совпадают'
  //   messageType.value = 'error'
  //   return
  // }

  // loading.value = true
   if (registerData.value.password !== registerData.value.confirmPassword) {
    message.value = 'Пароли не совпадают'
    alert("Пароли не совпадают")
    return
  }
  message.value = ''

  try {
    const userData = {
      email: registerData.value.email,
      nickname: registerData.value.nickname,
      password: registerData.value.password,
      
    }
    console.log('Отправка данных регистрации:', userData)
    console.log("props", props)
    emit("register",userData)

    // const result = await register(userData)
    // console.log('Получен результат:', result)
    // message.value = String(result.message)
    // if(result.success){
      
    //   messageType.value = 'success'
    //   registerData.value = {
    //   email: '',
    //   nickname: '',
    //   password: '',
    //   confirmPassword: ''
    // }
    //   switchForm()
    // }

    // alert(message.value)
  
  } catch (error) {
    message.value = error
    messageType.value = 'error'
    console.log(message)
  } 
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
        <div class="password-input-wrapper">
        <input :type="showLoginPassword?'text': 'password'" id="password" class="input" v-model="loginData.password" required />
         <button 
            type="button" 
            class="password-toggle"
            @click="showLoginPassword = !showLoginPassword"
          >
            <svg v-if="showLoginPassword" width="20" height="20" viewBox="0 0 24 24" fill="currentColor">
              <path d="M12 7c2.76 0 5 2.24 5 5 0 .65-.13 1.26-.36 1.83l2.92 2.92c1.51-1.26 2.7-2.89 3.43-4.75-1.73-4.39-6-7.5-11-7.5-1.4 0-2.74.25-3.98.7l2.16 2.16C10.74 7.13 11.35 7 12 7zM2 4.27l2.28 2.28.46.46C3.08 8.3 1.78 10.02 1 12c1.73 4.39 6 7.5 11 7.5 1.55 0 3.03-.3 4.38-.84l.42.42L19.73 22 21 20.73 3.27 3 2 4.27zM7.53 9.8l1.55 1.55c-.05.21-.08.43-.08.65 0 1.66 1.34 3 3 3 .22 0 .44-.03.65-.08l1.55 1.55c-.67.33-1.41.53-2.2.53-2.76 0-5-2.24-5-5 0-.79.2-1.53.53-2.2zm4.31-.78l3.15 3.15.02-.16c0-1.66-1.34-3-3-3l-.17.01z"/>
            </svg>
            <svg v-else width="20" height="20" viewBox="0 0 24 24" fill="currentColor">
              <path d="M12 4.5C7 4.5 2.73 7.61 1 12c1.73 4.39 6 7.5 11 7.5s9.27-3.11 11-7.5c-1.73-4.39-6-7.5-11-7.5zM12 17c-2.76 0-5-2.24-5-5s2.24-5 5-5 5 2.24 5 5-2.24 5-5 5zm0-8c-1.66 0-3 1.34-3 3s1.34 3 3 3 3-1.34 3-3-1.34-3-3-3z"/>
            </svg>
          </button>
        </div>
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
        <div class="password-input-wrapper">
        <input :type="showRegisterPassword?'text': 'password'"
         id="reg-password" class="input" v-model="registerData.password" required />
          <button 
            type="button" 
            class="password-toggle"
            @click="showRegisterPassword = !showRegisterPassword"
          >
            <svg v-if="showRegisterPassword" width="20" height="20" viewBox="0 0 24 24" fill="currentColor">
              <path d="M12 7c2.76 0 5 2.24 5 5 0 .65-.13 1.26-.36 1.83l2.92 2.92c1.51-1.26 2.7-2.89 3.43-4.75-1.73-4.39-6-7.5-11-7.5-1.4 0-2.74.25-3.98.7l2.16 2.16C10.74 7.13 11.35 7 12 7zM2 4.27l2.28 2.28.46.46C3.08 8.3 1.78 10.02 1 12c1.73 4.39 6 7.5 11 7.5 1.55 0 3.03-.3 4.38-.84l.42.42L19.73 22 21 20.73 3.27 3 2 4.27zM7.53 9.8l1.55 1.55c-.05.21-.08.43-.08.65 0 1.66 1.34 3 3 3 .22 0 .44-.03.65-.08l1.55 1.55c-.67.33-1.41.53-2.2.53-2.76 0-5-2.24-5-5 0-.79.2-1.53.53-2.2zm4.31-.78l3.15 3.15.02-.16c0-1.66-1.34-3-3-3l-.17.01z"/>
            </svg>
            <svg v-else width="20" height="20" viewBox="0 0 24 24" fill="currentColor">
              <path d="M12 4.5C7 4.5 2.73 7.61 1 12c1.73 4.39 6 7.5 11 7.5s9.27-3.11 11-7.5c-1.73-4.39-6-7.5-11-7.5zM12 17c-2.76 0-5-2.24-5-5s2.24-5 5-5 5 2.24 5 5-2.24 5-5 5zm0-8c-1.66 0-3 1.34-3 3s1.34 3 3 3 3-1.34 3-3-1.34-3-3-3z"/>
            </svg>
          </button>
        </div>
      </div>
      <div class="form-group">
        <label for="confirm-password">Подтвердите пароль</label>
          <div class="password-input-wrapper">
          <input 
            :type="showConfirmPassword ? 'text' : 'password'" 
            id="confirm-password" 
            class="input password-input" 
            v-model="registerData.confirmPassword" 
            required 
          />
          <button 
            type="button" 
            class="password-toggle"
            @click="showConfirmPassword = !showConfirmPassword"
          >
            <svg v-if="showConfirmPassword" width="20" height="20" viewBox="0 0 24 24" fill="currentColor">
              <path d="M12 7c2.76 0 5 2.24 5 5 0 .65-.13 1.26-.36 1.83l2.92 2.92c1.51-1.26 2.7-2.89 3.43-4.75-1.73-4.39-6-7.5-11-7.5-1.4 0-2.74.25-3.98.7l2.16 2.16C10.74 7.13 11.35 7 12 7zM2 4.27l2.28 2.28.46.46C3.08 8.3 1.78 10.02 1 12c1.73 4.39 6 7.5 11 7.5 1.55 0 3.03-.3 4.38-.84l.42.42L19.73 22 21 20.73 3.27 3 2 4.27zM7.53 9.8l1.55 1.55c-.05.21-.08.43-.08.65 0 1.66 1.34 3 3 3 .22 0 .44-.03.65-.08l1.55 1.55c-.67.33-1.41.53-2.2.53-2.76 0-5-2.24-5-5 0-.79.2-1.53.53-2.2zm4.31-.78l3.15 3.15.02-.16c0-1.66-1.34-3-3-3l-.17.01z"/>
            </svg>
            <svg v-else width="20" height="20" viewBox="0 0 24 24" fill="currentColor">
              <path d="M12 4.5C7 4.5 2.73 7.61 1 12c1.73 4.39 6 7.5 11 7.5s9.27-3.11 11-7.5c-1.73-4.39-6-7.5-11-7.5zM12 17c-2.76 0-5-2.24-5-5s2.24-5 5-5 5 2.24 5 5-2.24 5-5 5zm0-8c-1.66 0-3 1.34-3 3s1.34 3 3 3 3-1.34 3-3-1.34-3-3-3z"/>
            </svg>
          </button>
        </div>
      </div>
      <button type="submit" class="btn btn-primary">Зарегистрироваться</button>
    </form>
    <!-- <div v-if="message" :class="['message', messageType]">
      {{ message }}
    </div> -->
    <p class="switch-hint">
      {{ formType === 'login' ? 'Нет аккаунта?' : 'Уже есть аккаунт?' }}
      <a href="#" @click.prevent="switchForm">
        {{ formType === 'login' ? 'Зарегистрироваться' : 'Войти' }}
      </a>
    </p>
  </section>
</template>

<style scoped>

.password-toggle {
  position: absolute;
  right: 0.3rem;
  top: 35%;
  transform: translateY(-50%);
  background: none;
  border: none;
  cursor: pointer;
  padding: 0.25rem;
  border-radius: 4px;
  color: var(--text-secondary);
  transition: background-color 0.2s;
  display: flex;
}

.auth-form { max-width: 560px; width: 100%; margin: 0 auto; }
.auth-form h2 { margin-bottom: 1rem; text-align: center; }
.form-group { display:flex; flex-direction:column; gap:.5rem; }
.auth-form button {  padding: 0.85rem; margin-top: .5rem; font-size: 1rem;}
.switch-hint { text-align: center; margin-top: 1rem; }

.password-input-wrapper {
  position: relative;
  display: flex;
  align-items: center;
}

.password-input {
  padding-right: 3rem; 
  width: 100%;
}

@media (max-width: 576px) { .auth-form { padding: 1rem; }
.password-toggle {
    right: 0.5rem;
  }
}
</style>