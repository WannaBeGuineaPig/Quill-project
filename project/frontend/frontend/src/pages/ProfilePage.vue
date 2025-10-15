<script setup>
import { useRouter } from 'vue-router'
import { useAppState } from '../composables/useAppState'
import UserProfile from '../components/UserProfile.vue'

const router = useRouter()
const { state, editUser } = useAppState()

const currentUser =  state.currentUser


const handleSave = (data) => {
  if (state.currentUser) {
    console.log(data)
    const ok = editUser(data)
    console.log(ok)
  }
}

const handleViewArticle = (id) => {
  router.push({ name: 'article', params: { id } })
}
</script>

<template>
  <UserProfile :user ="currentUser" @editUser="handleSave" @view-article="handleViewArticle" />
</template>