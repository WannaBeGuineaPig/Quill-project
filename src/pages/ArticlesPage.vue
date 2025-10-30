<script setup>
import { useRouter } from 'vue-router'
import { useAppState } from '../composables/useAppState'
import ArticlesList from '../components/ArticlesList.vue'

const router = useRouter()
const { isLoggedIn, state } = useAppState()

const handleViewArticle = (id) => {
  router.push({ name: 'article', params: { id } })
}

const handleAddArticle = () => {
  router.push({ name: 'addArticle'})
}


</script>

<template>
  <ArticlesList
    :isLoggedIn="isLoggedIn"
    @view-article="handleViewArticle"
    @add-article="handleAddArticle"
  />
</template>