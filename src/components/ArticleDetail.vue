<script setup>
import { ref, computed , onUnmounted} from 'vue'
import { useAppState } from '@/composables/useAppState'

const { formatDate, } = useAppState()
const props = defineProps({
  isLoggedIn: {
    type: Boolean,
    default: false
  },
  isAdmin: {
    type: Boolean,
    default: false
  },
  isAuthor: {
    type: Boolean,
    default: false
  },
  currentArticle: {
    type: Object,
    required: true
  },
  comments: {
    type: Array,
    default: () => []
  }, 
  userRating: { 
    type: Number,
    default: 0
  },
  articleStats: { 
    type: Object,
    default: () => ({ likes: 0, dislikes: 0 })
  }
})

const emit = defineEmits(['edit-article', 'delete-article', 'back-to-list', 'vote', 'add-comment', 'delete-comment'])

const commentText = ref('')

const isLiked = computed(() => props.userRating === 1)
const isDisliked = computed(() => props.userRating === -1)

const likesCount = computed(() => props.articleStats.likes)
const dislikesCount = computed(() => props.articleStats.dislikes)

const submitComment = () => {
  if (commentText.value.trim()) {
    emit('add-comment', commentText.value.trim())
    commentText.value = ''
  }
}

const vote = (value) => {
  emit('vote', value)
}

const editArticle = (id) => {

  emit('edit-article', id)
}

const deleteArticle = () => 
{
  const confirmDelete = confirm("Вы уверены, что хотите удалить статью? Действие невозможно будет отменить")
  if(confirmDelete){
    emit('delete-article')}
}

const handleDeleteComment = (commentId) => {
  if (confirm('Вы уверены, что хотите удалить этот комментарий?')) {
    emit('delete-comment', commentId)
  }
}

const goBack = () => {
  emit('back-to-list')
}

onUnmounted(() => {
  
})

// Вычисляемое свойство для отформатированных комментариев
const formattedComments = computed(() => {
  return props.comments.map(comment => ({
    ...comment,
    formattedDate: formatDate(comment.publishedAt)
  }))
})
</script>

<template>
  <section class="article-detail glass stack">
    <div class="row space-between">
      <button class="btn btn-secondary" @click="goBack">← Назад к списку</button>
      <div class="row article-actions" v-if="isLoggedIn">
        <button class="btn btn-secondary" v-if="isAuthor" @click="editArticle(currentArticle.id)">Редактировать</button>
        <button class="btn btn-danger" v-if="isAuthor || isAdmin" @click="deleteArticle">Удалить</button>
      </div>
    </div>
    
    <div class="article-header stack">
      <h2 class="card-title brand-gradient">{{ currentArticle.title }}</h2>
      <div class="article-meta row">
        <span class="pill tag-theme">{{ currentArticle.topicName }}</span>
        <span class="pill">{{ formatDate(currentArticle.publishedAt) }}</span>
        <span class="pill">Автор: {{ currentArticle.authorName }}</span>
      </div>
      <div class="article-rating row">
        <span class="pill">Лайков: {{ likesCount }} · Дизлайков: {{ dislikesCount }}</span>
        <div v-if="isLoggedIn" class="rating-controls row">
          <button 
            class="btn btn-ghost" 
            :class="{ 'active-rating': isLiked }"
            @click="vote(1)"
          >
            👍 {{ isLiked ? 'Вам нравится' : 'Нравится' }}
          </button>
          <button 
            class="btn btn-ghost" 
            :class="{ 'active-rating': isDisliked }"
            @click="vote(-1)"
          >
            👎 {{ isDisliked ? 'Вам не нравится' : 'Не нравится' }}
          </button>
        </div>
      </div>
    </div>
     <div class="article-image-section" v-if="currentArticle.hasImage">
      <img 
        :src="currentArticle.imageUrl" 
        :alt="currentArticle.title"
        class="article-image"
        @error="handleImageError"
      />
      
    </div>
    
    <div class="article-content">
      <p>{{ currentArticle.content }}</p>
    </div>
    
    <div class="comments-section stack">
      <h3 class="card-title">Комментарии ({{ comments.length }})</h3>
      
      <div v-if="isLoggedIn" class="comment-form stack">
        <textarea 
          class="input" 
          v-model="commentText" 
          placeholder="Оставьте комментарий..."
          rows="3"
        ></textarea>
        <button 
          class="btn btn-primary" 
          @click="submitComment" 
          :disabled="!commentText.trim()"
        >
          Отправить
        </button>
      </div>
      
      <div class="comments-list stack" v-if="formattedComments.length > 0">
        <div 
          v-for="comment in formattedComments" 
          :key="comment.id" 
          class="comment glass"
        >
          <div class="comment-header row space-between">
            <span class="comment-author">{{ comment.authorName || 'Аноним' }}</span>
            <span class="comment-date">{{ comment.formattedDate }}</span>
          </div>
          <p class="comment-text">{{ comment.content }}</p>
          <button 
            v-if="isAdmin || isAuthor" 
            class="btn btn-ghost btn-delete-small" 
            @click="handleDeleteComment(comment.id)"
          >
            Удалить
          </button>
        </div>
      </div>
      
      <div v-else class="no-comments">
        <p>Пока нет комментариев. Будьте первым!</p>
      </div>
    </div>
  </section>
</template>

<style scoped>
.article-detail { width: 100%; padding: 1.75rem; border-radius: var(--radius); }
.article-header { margin-bottom: .5rem; }
.article-content { margin-bottom: 1rem; font-size: 1.1rem; line-height: 1.9; }
.comments-section { margin-top: 1rem; }
.comment { border-radius: 12px; padding: 1rem; position: relative; margin-bottom: 1rem;}
.btn-delete-small { 
  position: absolute; 
  bottom: .5rem; 
  right: .5rem; 
  font-size: 0.8rem; 
  padding: 0.3rem 0.6rem; 
}
.input{ padding: 1rem; }
.rating-controls { gap: .4rem; }
.no-comments { 
  text-align: center; 
  color: #666; 
  padding: 2rem; 
  font-style: italic;
}

/* Стиль для активной оценки - фиолетовый текст */
.active-rating {
  color: #8b5cf6 !important; /* Фиолетовый цвет */
  border-color: #8b5cf6 !important;
}

@media (max-width: 768px) {
  .article-detail { padding: 1rem; }
}
</style>