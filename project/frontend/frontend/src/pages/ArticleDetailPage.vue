<script setup>
import { useRoute, useRouter } from 'vue-router'
import { computed } from 'vue'
import { useAppState } from '../composables/useAppState'
import ArticleDetail from '../components/ArticleDetail.vue'

const route = useRoute()
const router = useRouter()
const { state, isLoggedIn, isAdmin, voteArticle, addComment, deleteArticle } = useAppState()

const articleId = Number(route.params.id)
const article = computed(() => state.articles.find(a => a.id === articleId))
const isAuthor = computed(() => article.value?.authorId === state.currentUser?.id)

const handleEdit = () => {
  router.push({ name: 'profile' })
}

const handleDelete = () => {
  deleteArticle(articleId)
  router.push({ name: 'home' })
}

const handleBack = () => {
  router.push({ name: 'home' })
}
</script>

<template>
  <ArticleDetail
    v-if="article"
    :isLoggedIn="isLoggedIn"
    :isAdmin="isAdmin"
    :isAuthor="isAuthor"
    @edit-article="handleEdit"
    @delete-article="handleDelete"
    @back-to-list="handleBack"
    @vote="(v) => voteArticle(articleId, v)"
  />
  <div v-else class="not-found glass">Статья не найдена</div>
</template>

<style scoped>
.not-found {
  border-radius: 8px;
  border: 1px solid var(--glass-border);
  background: var(--glass-bg);
  backdrop-filter: saturate(140%) blur(12px);
  -webkit-backdrop-filter: saturate(140%) blur(12px);
  padding: 2rem;
}
</style>