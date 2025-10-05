<script setup>
import AdminPanel from '../components/AdminPanel.vue'
import { useAppState } from '../composables/useAppState'

const { blockArticle, blockUser, blockComment, state } = useAppState()

// For demo, admin actions are wired directly; the AdminPanel can emit events in future.
</script>

<template>
  <AdminPanel />
</template>