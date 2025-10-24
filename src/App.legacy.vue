<script setup>
import { ref } from 'vue'
import Header from './components/header.vue'
import ArticlesList from './components/ArticlesList.vue'
import ArticleDetail from './components/ArticleDetail.vue'
import ArticleForm from './components/ArticleForm.vue'
import AuthForms from './components/AuthForms.vue'
import UserProfile from './components/UserProfile.vue'
import AdminPanel from './components/AdminPanel.vue'


// Состояние приложения
const currentView = ref('articles') // 'articles', 'article-detail', 'article-form', 'login', 'register', 'profile', 'admin'
const isLoggedIn = ref(false)
const isAdmin = ref(false)
const userNickname = ref('Пользователь')
const authFormType = ref('login')
const editMode = ref(false)
const currentArticle = ref(null)

// Методы для навигации
const navigate = (view) => {
  currentView.value = view
}

// Методы для аутентификации
const login = () => {
  currentView.value = 'login'
  authFormType.value = 'login'
}

const register = () => {
  currentView.value = 'register'
  authFormType.value = 'register'
}

const logout = () => {
  isLoggedIn.value = false
  isAdmin.value = false
  currentView.value = 'articles'
}

const handleLogin = (data) => {
  console.log('Login data:', data)
  isLoggedIn.value = true
  // Для демонстрации, в реальном приложении здесь будет запрос к API
  userNickname.value = 'Пользователь'
  currentView.value = 'articles'
}

const handleRegister = (data) => {
  console.log('Register data:', data)
  isLoggedIn.value = true
  userNickname.value = data.nickname
  currentView.value = 'articles'
}

const switchAuthForm = (formType) => {
  authFormType.value = formType
  currentView.value = formType
}

// Методы для работы со статьями
const viewArticle = (articleId) => {
  console.log('View article:', articleId)
  currentArticle.value = { id: articleId }
  currentView.value = 'article-detail'
}

const addArticle = () => {
  editMode.value = false
  currentArticle.value = null
  currentView.value = 'article-form'
}

const editArticle = () => {
  editMode.value = true
  currentView.value = 'article-form'
}

const handleArticleSubmit = (articleData) => {
  console.log('Article data:', articleData)
  currentView.value = 'articles'
}

const cancelArticleForm = () => {
  currentView.value = editMode.value ? 'article-detail' : 'articles'
}

const saveProfile = (profileData) => {
  console.log('Profile data:', profileData)
  userNickname.value = profileData.nickname
}
</script>

<template>
  <div class="app-container">
    <Header 
      :isLoggedIn="isLoggedIn" 
      :isAdmin="isAdmin" 
      :userNickname="userNickname"
      @navigate="navigate"
      @login="login"
      @register="register"
      @logout="logout"
    />
    
    <main class="main-content">
      <!-- Список статей -->
      <ArticlesList 
        v-if="currentView === 'articles'" 
        :isLoggedIn="isLoggedIn"
        @view-article="viewArticle"
        @add-article="addArticle"
      />
      
      <!-- Детальная страница статьи -->
      <ArticleDetail>
        v-if="currentView === 'article-detail'" 
        :isLoggedIn="isLoggedIn"
        :isAdmin="isAdmin"
        :isAuthor="true" <!-- В реальном приложении здесь будет проверка -->
        @edit-article="editArticle"
        @delete-article="navigate('articles')"
        @back-to-list="navigate('articles')"
      </ArticleDetail> 
      
      <!-- Форма создания/редактирования статьи -->
      <ArticleForm 
        v-if="currentView === 'article-form'" 
        :editMode="editMode"
        :article="currentArticle || {}"
        @submit="handleArticleSubmit"
        @cancel="cancelArticleForm"
      />
      
      <!-- Формы авторизации и регистрации -->
      <AuthForms 
        v-if="currentView === 'login' || currentView === 'register'" 
        :formType="authFormType"
        @switch-form="switchAuthForm"
        @login="handleLogin"
        @register="handleRegister"
      />
      
      <!-- Профиль пользователя -->
      <UserProfile 
        v-if="currentView === 'profile'" 
        @save-profile="saveProfile"
        @view-article="viewArticle"
      />
      
      <!-- Панель администратора -->
      <AdminPanel 
        v-if="currentView === 'admin' && isAdmin" 
      />
    </main>
    
    <footer class="app-footer">
      <p>&copy; 2023 Система управления статьями</p>
    </footer>
  </div>
</template>

<style>
* {
  box-sizing: border-box;
  margin: 0;
  padding: 0;
}

body {
  font-family: Arial, sans-serif;
  line-height: 1.6;
  color: #333;
  background-color: #f4f4f4;
}

.app-container {
  display: flex;
  flex-direction: column;
  min-height: 100vh;
  width: 100%;
}

.main-content {
  flex: 1;
  padding: 2rem;
  max-width: 1200px;
  margin: 0 auto;
  width: 100%;
  display: flex;
  justify-content: center;
}

.btn {
  padding: 0.5rem 1rem;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-weight: 500;
  transition: background-color 0.3s;
}

.btn-primary {
  background-color: #007bff;
  color: white;
}

.btn-search {
  background-color: #28a745;
  color: white;
}

.btn-view {
  background-color: #17a2b8;
  color: white;
}

.btn-edit {
  background-color: #ffc107;
  color: #333;
}

.btn-delete, .btn-delete-small {
  background-color: #dc3545;
  color: white;
}

.app-footer {
  background-color: #343a40;
  color: white;
  text-align: center;
  padding: 1.5rem;
  margin-top: 2rem;
  width: 100%;
}

/* Адаптивность */
@media (max-width: 992px) {
  .main-content {
    padding: 1.5rem;
  }
}

@media (max-width: 576px) {
  .main-content {
    padding: 1rem;
  }
  
  .app-footer {
    padding: 1rem;
  }
}
</style>