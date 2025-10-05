<script setup>
import { ref } from 'vue'

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
  }
})

const emit = defineEmits(['edit-article', 'delete-article', 'back-to-list', 'vote'])

const commentText = ref('')

const submitComment = () => {
  // Логика отправки комментария
  commentText.value = ''
}

const vote = (value) => {
  emit('vote', value)
}

const editArticle = () => {
  emit('edit-article')
}

const deleteArticle = () => {
  emit('delete-article')
}

const deleteComment = (commentId) => {
  // Логика удаления комментария
}

const goBack = () => {
  emit('back-to-list')
}
</script>

<template>
  <section class="article-detail glass stack">
    <div class="row space-between">
      <button class="btn btn-secondary" @click="goBack">← Назад к списку</button>
      <div class="row article-actions" v-if="isLoggedIn">
        <button class="btn btn-secondary" v-if="isAuthor" @click="editArticle">Редактировать</button>
        <button class="btn btn-danger" v-if="isAuthor || isAdmin" @click="deleteArticle">Удалить</button>
      </div>
    </div>
    
    <div class="article-header stack">
      <h2 class="card-title brand-gradient">Название статьи</h2>
      <div class="article-meta row">
        <span class="pill tag-theme">Технологии</span>
        <span class="pill">01.01.2023</span>
        <span class="pill">Автор: username</span>
      </div>
      <div class="article-rating row">
        <span class="pill">Лайков: 0 · Дизлайков: 0</span>
        <div v-if="isLoggedIn" class="rating-controls row">
          <button class="btn btn-ghost" @click="vote(1)">👍</button>
          <button class="btn btn-ghost" @click="vote(-1)">👎</button>
        </div>
      </div>
    </div>
    
    <div class="article-content">
      <p>Текст статьи будет здесь...</p>
    </div>
    
    <div class="comments-section stack">
      <h3 class="card-title">Комментарии</h3>
      
      <div v-if="isLoggedIn" class="comment-form stack">
        <textarea class="input" v-model="commentText" placeholder="Оставьте комментарий..."></textarea>
        <button class="btn btn-primary" @click="submitComment">Отправить</button>
      </div>
      
      <div class="comments-list stack">
        <div class="comment glass">
          <div class="comment-header row space-between">
            <span class="comment-author">username</span>
            <span class="comment-date">01.01.2023</span>
          </div>
          <p class="comment-text">Текст комментария...</p>
          <button v-if="isAdmin" class="btn btn-ghost btn-delete-small" @click="deleteComment(1)">Удалить</button>
        </div>
        <!-- Здесь будут другие комментарии -->
      </div>
    </div>
  </section>
</template>

<style scoped>
.article-detail { width: 100%; padding: 1.75rem; border-radius: var(--radius); }
.article-header { margin-bottom: .5rem; }
.article-content { margin-bottom: 1rem; font-size: 1.1rem; line-height: 1.9; }
.comments-section { margin-top: 1rem; }
.comment { border-radius: 12px; padding: 1rem; position: relative; }
.btn-delete-small { position: absolute; bottom: .5rem; right: .5rem; }
.rating-controls { gap: .4rem; }

@media (max-width: 768px) {
  .article-detail { padding: 1rem; }
}
</style>