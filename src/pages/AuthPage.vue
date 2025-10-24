<script setup>
import { useRouter } from 'vue-router'
import { useAppState } from '../composables/useAppState'
import { defineProps } from 'vue'
import AuthForms from '../components/AuthForms.vue'

const props = defineProps({ formType: { type: String, default: 'login' } })
const router = useRouter()
const { login, register } = useAppState()

const handleSwitch = (type) => {
  router.push({ name: type })
}

const handleLogin = async(data) => {
  const ok = await login(data)

  if(ok.success){
    console.log(ok)
    alert("Успешная авторизация")
    router.push({ name: 'home' })
  }
  
}

const handleRegister = async(data) => {
  
  const ok = await register(data)
    if(ok.success){
      alert("Успешная регистрация!")
      router.push({ name: 'login' })
    }
    else{
      alert(ok.message)
    }
}
</script>

<template>
  <AuthForms
    :formType="props.formType"
    @switch-form="handleSwitch"
    @login="handleLogin"
    @register="handleRegister"
  />
  
</template>