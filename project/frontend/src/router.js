import { createRouter, createWebHistory } from 'vue-router'

// Pages
import ArticlesPage from './pages/ArticlesPage.vue'
import ArticleDetailPage from './pages/ArticleDetailPage.vue'
import AuthPage from './pages/AuthPage.vue'
import ProfilePage from './pages/ProfilePage.vue'
import AdminPage from './pages/AdminPage.vue'

const routes = [
  { path: '/', name: 'home', component: ArticlesPage },
  { path: '/article/:id', name: 'article', component: ArticleDetailPage, props: true },
  { path: '/login', name: 'login', component: AuthPage, props: { formType: 'login' } },
  { path: '/register', name: 'register', component: AuthPage, props: { formType: 'register' } },
  { path: '/profile', name: 'profile', component: ProfilePage },
  { path: '/admin', name: 'admin', component: AdminPage },
]

const router = createRouter({
  history: createWebHistory(),
  routes,
})

export default router