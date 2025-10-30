<script setup>
import { useRoute, useRouter } from 'vue-router'
import { computed, ref, onMounted } from 'vue'
import { useAppState } from '../composables/useAppState'
import ArticleDetail from '../components/ArticleDetail.vue'
import { 
  getLikesCount, 
  getDislikesCount 
} from '../userApi'
import { watch } from 'vue'

// Следим за изменением авторизации



const route = useRoute()
const router = useRouter()
const { 
  state, 
  isLoggedIn, 
  isAdmin, 
  voteArticle, 
  addCommentToArticle, 
  deleteComment, 
  loadComments,
  getArticleId,
  getUserArticleRating, 
  deleteArticleStat 
} = useAppState()

const articleId = Number(route.params.id)
const article = ref(null)
const comments = ref([])
const loading = ref(true)
const userRating = ref(0)
const articleStats = ref({ 
  likes: 0,
  dislikes: 0
})

const isAuthor = computed(() => article.value?.authorId === state.currentUser?.id)

// Функция для загрузки статистики статьи
const loadArticleStats = async (id) => {
  try {
    // Используем методы из API для получения актуальных счетчиков
    const likes = await getLikesCount(id);
    const dislikes = await getDislikesCount(id);
    
    articleStats.value = {
      likes: likes || 0,
      dislikes: dislikes || 0
    }
  } catch (error) {
    console.error('Ошибка загрузки статистики:', error)
    // Fallback на данные из статьи
    if (article.value) {
      articleStats.value = {
        likes: article.value.likes || 0,
        dislikes: article.value.dislikes || 0
      }
    }
  }
}

// Загружаем статью, комментарии и рейтинг
onMounted(async () => {
  try {
    console.log('Загрузка статьи с ID:', articleId)
    
    // Загружаем статью
    article.value = await getArticleId(articleId)
    console.log('Загруженная статья:', article.value)
    
    // Загружаем статистику статьи
    await loadArticleStats(articleId)

    // Загружаем комментарии
    if (article.value) {
      comments.value = await loadComments(articleId)
      console.log('Загруженные комментарии:', comments.value)
    }

    // Загружаем рейтинг пользователя
    if (isLoggedIn.value) {
      userRating.value = await getUserArticleRating(articleId)
      console.log('Рейтинг пользователя:', userRating.value)
    }
  } catch (error) {
    console.error('Ошибка загрузки:', error)
  } finally {
    loading.value = false
  }
})

watch(isLoggedIn, async (newVal) => {
  if (newVal && article.value) {
    // Перезагружаем рейтинг пользователя при входе
    userRating.value = await getUserArticleRating(articleId)
    console.log('Рейтинг перезагружен после входа:', userRating.value)
  } else if (!newVal) {
    // Сбрасываем рейтинг при выходе
    userRating.value = 0
    console.log('Рейтинг сброшен после выхода')
  }
})

const handleEdit = (id) => {
  router.push({ name: 'editArticle',params: { id }  })
}

const handleDelete = async() => {
  const ok = await deleteArticleStat(articleId)
  if(ok.success){
    alert("Удалено")
    router.push({ name: 'home' })
  }
  else{
    alert(ok.message)
  }
  
}

const handleBack = () => {
  router.push({ name: 'home' })
}

const handleVote = async (value) => {
  const success = await voteArticle(articleId, value)
  if (success) {
    // Сразу обновляем локальное состояние рейтинга
    const previousRating = userRating.value
    
    if (previousRating === value) {
      userRating.value = 0 // снимаем оценку
    } else {
      userRating.value = value // устанавливаем новую оценку
    }
    
    // ПЕРЕЗАГРУЖАЕМ актуальную статистику с сервера
    await loadArticleStats(articleId)
    
    console.log('Обновленная статистика:', articleStats.value)
    console.log('Текущий рейтинг пользователя:', userRating.value)
  }
}


const handleAddComment = async (text) => {
  const success = await addCommentToArticle(articleId, text)
  if (success) {
    // Перезагружаем комментарии после добавления
    comments.value = await loadComments(articleId)
    console.log('Комментарии обновлены после добавления')
  }
}

const handleDeleteComment = async (commentId) => {
  const success = await deleteComment(commentId)
  if (success) {
    // Удаляем комментарий из локального состояния
    comments.value = comments.value.filter(comment => comment.id !== commentId)
    console.log('Комментарий удален из локального состояния')
  }
}
</script>

<template>
  <div v-if="loading" class="loading glass">
    Загрузка статьи...
  </div>
  <ArticleDetail
    v-else-if="article"
    :isLoggedIn="isLoggedIn"
    :isAdmin="isAdmin"
    :isAuthor="isAuthor"
    :currentArticle="article"
    :comments="comments"
    :userRating="userRating"
    :articleStats="articleStats"
    @edit-article="handleEdit"
    @delete-article="handleDelete"
    @back-to-list="handleBack"
    @vote="handleVote"
    @add-comment="handleAddComment"
    @delete-comment="handleDeleteComment"
  />
  <div v-else class="not-found glass">
    Статья не найдена
    <button class="btn btn-secondary" @click="handleBack">Вернуться к списку</button>
  </div>
</template>

<style scoped>
.loading, .not-found {
  border-radius: 8px;
  border: 1px solid var(--glass-border);
  background: var(--glass-bg);
  backdrop-filter: saturate(140%) blur(12px);
  -webkit-backdrop-filter: saturate(140%) blur(12px);
  padding: 2rem;
  text-align: center;
}

.not-found {
  display: flex;
  flex-direction: column;
  gap: 1rem;
  align-items: center;
}
</style>
