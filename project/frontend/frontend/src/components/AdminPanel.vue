<script setup>
import { ref } from 'vue'

const activeTab = ref('users') // 'users', 'articles', 'comments'

const changeTab = (tab) => {
  activeTab.value = tab
}

const deleteUser = (userId) => {
  // Логика удаления пользователя
}

const deleteArticle = (articleId) => {
  // Логика удаления статьи
}

const deleteComment = (commentId) => {
  // Логика удаления комментария
}
</script>

<template>
  <section class="admin-section">
    <h2>Панель администратора</h2>
    
    <div class="admin-tabs">
      <button 
        class="tab-btn" 
        :class="{ active: activeTab === 'users' }"
        @click="changeTab('users')"
      >
        Пользователи
      </button>
      <button 
        class="tab-btn" 
        :class="{ active: activeTab === 'articles' }"
        @click="changeTab('articles')"
      >
        Статьи
      </button>
      <button 
        class="tab-btn" 
        :class="{ active: activeTab === 'comments' }"
        @click="changeTab('comments')"
      >
        Комментарии
      </button>
    </div>
    
    <div class="admin-content">
      <!-- Управление пользователями -->
      <div class="users-list" v-if="activeTab === 'users'">
        <div class="user-item">
          <span class="user-email">user@example.com</span>
          <span class="user-nickname">username</span>
          <span class="user-role">Пользователь</span>
          <span class="user-status">Активен</span>
          <button 
            class="btn btn-delete-small" 
            @click="deleteUser(1)"
          >
            Удалить
          </button>
        </div>
        <!-- Здесь будут другие пользователи -->
      </div>
      
      <!-- Управление статьями -->
      <div class="articles-list" v-if="activeTab === 'articles'">
        <div class="article-item">
          <span class="article-title">Название статьи</span>
          <span class="article-author">Автор: username</span>
          <span class="article-date">01.01.2023</span>
          <button 
            class="btn btn-delete-small" 
            @click="deleteArticle(1)"
          >
            Удалить
          </button>
        </div>
        <!-- Здесь будут другие статьи -->
      </div>
      
      <!-- Управление комментариями -->
      <div class="comments-list" v-if="activeTab === 'comments'">
        <div class="comment-item">
          <span class="comment-text">Текст комментария...</span>
          <span class="comment-author">Автор: username</span>
          <span class="comment-date">01.01.2023</span>
          <button 
            class="btn btn-delete-small" 
            @click="deleteComment(1)"
          >
            Удалить
          </button>
        </div>
        <!-- Здесь будут другие комментарии -->
      </div>
    </div>
  </section>
</template>

<style scoped>
.admin-section {
  background: var(--glass-bg);
  border: 1px solid var(--glass-border);
  backdrop-filter: saturate(140%) blur(14px);
  -webkit-backdrop-filter: saturate(140%) blur(14px);
  border-radius: var(--radius);
  box-shadow: var(--shadow);
  padding: 2rem;
  width: 100%;
}

.admin-tabs {
  display: flex;
  gap: 1rem;
  margin-bottom: 2rem;
  border-bottom: 1px solid #ddd;
  padding-bottom: 1rem;
  flex-wrap: wrap;
}

.tab-btn {
  background: none;
  border: none;
  padding: 0.5rem 1rem;
  cursor: pointer;
  font-weight: 500;
  color: #6c757d;
  transition: color 0.3s;
}

.tab-btn.active {
  color: #007bff;
  border-bottom: 2px solid #007bff;
}

.tab-btn:hover {
  color: #007bff;
}

.users-list, .articles-list, .comments-list {
  width: 100%;
}

.user-item, .article-item, .comment-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1rem;
  border-bottom: 1px solid #ddd;
  flex-wrap: wrap;
  gap: 0.5rem;
}

.user-email, .user-nickname, .user-role, .user-status,
.article-title, .article-author, .article-date,
.comment-text, .comment-author, .comment-date {
  flex: 1;
  min-width: 150px;
}

.btn-delete-small {
  background-color: #dc3545;
  color: white;
  padding: 0.25rem 0.5rem;
  font-size: 0.875rem;
  border-radius: 4px;
  border: none;
  cursor: pointer;
}

/* Адаптивность */
@media (max-width: 992px) {
  .user-item, .article-item, .comment-item {
    flex-direction: column;
    align-items: flex-start;
  }
  
  .btn-delete-small {
    margin-top: 0.5rem;
    align-self: flex-end;
  }
}

@media (max-width: 576px) {
  .admin-section {
    padding: 1.5rem;
  }
  
  .admin-tabs {
    flex-direction: column;
    gap: 0.5rem;
  }
  
  .tab-btn.active {
    border-left: 2px solid #007bff;
    border-bottom: none;
  }
}
</style>