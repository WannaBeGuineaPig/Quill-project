<script setup>
import { ref } from 'vue'

const emit = defineEmits(['save-profile', 'view-article'])

const profileData = ref({
  email: 'user@example.com',
  nickname: 'username'
})

const saveProfile = () => {
  emit('save-profile', profileData.value)
}

const viewArticle = (articleId) => {
  emit('view-article', articleId)
}
</script>

<template>
  <section class="profile-section card">
    <h2 class="card-title brand-gradient">Профиль пользователя</h2>
    <div class="profile-info">
      <div class="form-group">
        <label for="profile-email">Email:</label>
        <input 
          type="email" 
          id="profile-email" 
          class="input"
          v-model="profileData.email" 
        />
      </div>
      <div class="form-group">
        <label for="profile-nickname">Никнейм:</label>
        <input 
          type="text" 
          id="profile-nickname" 
          class="input"
          v-model="profileData.nickname" 
        />
      </div>
      <button class="btn btn-primary" @click="saveProfile">
        Сохранить изменения
      </button>
    </div>
    
    <div class="user-articles">
      <h3 class="card-title">Мои статьи</h3>
      <div class="articles-grid">
        <!-- Здесь будут статьи пользователя -->
        <div class="article-card card">
          <h3 class="article-title">Моя статья</h3>
          <p class="article-theme">Тематика: Технологии</p>
          <p class="article-date">Опубликовано: 01.01.2023</p>
          <div class="article-rating">Лайки: 0 · Дизлайки: 0</div>
          <div class="article-actions">
            <button class="btn btn-view" @click="viewArticle(1)">
              Просмотр
            </button>
          </div>
        </div>
      </div>
    </div>
  </section>
</template>

<style scoped>
.profile-section { padding: 2rem; width: 100%; }

.profile-info {
  margin-bottom: 3rem;
}

.form-group {
  margin-bottom: 1.5rem;
}

.form-group label {
  display: block;
  margin-bottom: 0.5rem;
  font-weight: 500;
}

.form-group input {
  width: 100%;
  padding: 0.75rem;
  border: 1px solid #ddd;
  border-radius: 4px;
}

.user-articles h3 {
  margin-bottom: 1.5rem;
}

.articles-grid { display:grid; grid-template-columns: repeat(auto-fill, minmax(360px, 1fr)); gap: 1.25rem; }

.article-card { padding: 1.5rem; transition: transform 0.25s ease; height: 100%; display:flex; flex-direction:column; }
.article-card:hover { transform: translateY(-2px); }

.article-title { margin-bottom: 0.5rem; }

.article-theme, .article-date { opacity: .8; font-size: 0.95rem; margin-bottom: 0.5rem; }

.article-rating {
  margin: 1rem 0;
  font-weight: 500;
}

.article-actions {
  margin-top: auto;
  padding-top: 1rem;
}

/* Адаптивность */
@media (max-width: 768px) {
  .profile-section {
    padding: 1.5rem;
  }
}
</style>