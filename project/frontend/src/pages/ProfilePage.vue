<script setup>
import { useRouter } from 'vue-router'
import { useAppState } from '../composables/useAppState'
import UserProfile from '../components/UserProfile.vue'

const router = useRouter()
const { state } = useAppState()

const handleSave = (data) => {
  if (state.currentUser) {
    state.currentUser.email = data.email
    state.currentUser.nickname = data.nickname
  }
}

const handleViewArticle = (id) => {
  router.push({ name: 'article', params: { id } })
}
</script>

<template>
  <UserProfile @save-profile="handleSave" @view-article="handleViewArticle" />
</template>