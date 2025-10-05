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

const handleLogin = (data) => {
  const ok = login(data)
  if (ok) router.push({ name: 'home' })
}

const handleRegister = (data) => {
  const ok = register(data)
  if (ok) router.push({ name: 'home' })
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