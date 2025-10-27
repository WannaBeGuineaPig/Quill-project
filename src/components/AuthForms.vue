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
            {{ showLoginPassword ? '🙈' : '👁️' }}
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
            {{ showRegisterPassword ? '🙈' : '👁️' }}
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
            {{ showConfirmPassword ? '📘' : '📖' }}
          </button>
        </div>
      </div>
      <button type="submit" class="btn btn-primary">Зарегистрироваться</button>
    </form>
      <div v-if="message" :class="['message', messageType]">
      {{ message }}
    </div>
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
  width:10%;
  position: absolute;
  right: 0.75rem;
  background: none;
  border: none;
  cursor: pointer;
  font-size: 1.2rem;
  padding: 0.25rem;
  border-radius: 4px;
  transition: background-color 0.2s;
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


.password-toggle:hover {
  background-color: rgba(0, 0, 0, 0.1);
}

.password-toggle:focus {
  outline: 2px solid var(--primary-color);
  outline-offset: 2px;
}

@media (max-width: 576px) { .auth-form { padding: 1rem; }
.password-toggle {
    right: 0.5rem;
  }
}
</style>