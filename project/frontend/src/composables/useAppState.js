import { reactive, computed } from 'vue'

// Simple global state for demo purposes (no backend)
const state = reactive({
  currentUser: null, // { id, email, nickname, role: 'guest'|'user'|'admin', status }
  users: [
    { id: 1, email: 'admin@example.com', nickname: 'Admin', role: 'admin', status: 'active' },
    { id: 2, email: 'user@example.com', nickname: 'UserOne', role: 'user', status: 'active' },
  ],
  articles: [
    {
      id: 101,
      title: 'Введение в веб-разработку',
      content: 'Текст статьи...'
        + ' Эта публикация демонстрирует структуру приложения и адаптивную верстку.',
      theme: 'Технологии',
      publishedAt: '2024-01-15',
      status: 'active', // 'active' | 'blocked'
      authorId: 2,
      votes: [ { userId: 2, value: 5 } ],
      comments: [
        { id: 5001, authorId: 2, text: 'Отличная статья!', publishedAt: '2024-01-16', status: 'active' },
      ],
    },
    {
      id: 102,
      title: 'Основы алгоритмов',
      content: 'Демонстрационная статья о базовых алгоритмах и структурах данных.',
      theme: 'Наука',
      publishedAt: '2024-02-03',
      status: 'active',
      authorId: 2,
      votes: [],
      comments: [],
    },
  ],
})

export function useAppState() {
  const isLoggedIn = computed(() => !!state.currentUser)
  const isAdmin = computed(() => state.currentUser?.role === 'admin')
  const currentUserId = computed(() => state.currentUser?.id ?? null)

  const login = ({ email }) => {
    const user = state.users.find(u => u.email === email)
    if (!user) return false
    if (user.status === 'blocked') return false
    state.currentUser = user
    return true
  }

  const register = ({ email, nickname }) => {
    const exists = state.users.some(u => u.email === email)
    if (exists) return false
    const newUser = { id: Date.now(), email, nickname, role: 'user', status: 'active' }
    state.users.push(newUser)
    state.currentUser = newUser
    return true
  }

  const logout = () => { state.currentUser = null }

  const addArticle = ({ title, content, theme }) => {
    if (!isLoggedIn.value) return false
    const newArticle = {
      id: Date.now(), title, content, theme,
      publishedAt: new Date().toISOString().slice(0,10), status: 'active',
      authorId: currentUserId.value, votes: [], comments: [],
    }
    state.articles.push(newArticle)
    return newArticle.id
  }

  const editArticle = (id, data) => {
    const a = state.articles.find(a => a.id === id)
    if (!a) return false
    Object.assign(a, data)
    return true
  }

  const deleteArticle = (id) => {
    const idx = state.articles.findIndex(a => a.id === id)
    if (idx !== -1) state.articles.splice(idx,1)
  }

  const blockArticle = (id) => {
    const a = state.articles.find(a => a.id === id)
    if (a) a.status = 'blocked'
  }

  const voteArticle = (id, value) => {
    if (!isLoggedIn.value) return false
    const a = state.articles.find(a => a.id === id)
    if (!a) return false
    const prev = a.votes.find(r => r.userId === currentUserId.value)
    if (prev) prev.value = value
    else a.votes.push({ userId: currentUserId.value, value })
    return true
  }

  const addComment = (id, text) => {
    if (!isLoggedIn.value) return false
    const a = state.articles.find(a => a.id === id)
    if (!a) return false
    a.comments.push({ id: Date.now(), authorId: currentUserId.value, text, publishedAt: new Date().toISOString().slice(0,10), status: 'active' })
    return true
  }

  const blockUser = (id) => {
    const u = state.users.find(u => u.id === id)
    if (u) u.status = 'blocked'
  }

  const blockComment = (articleId, commentId) => {
    const a = state.articles.find(a => a.id === articleId)
    if (!a) return
    const c = a.comments.find(c => c.id === commentId)
    if (c) c.status = 'blocked'
  }

  return {
    state,
    isLoggedIn,
    isAdmin,
    currentUserId,
    login,
    register,
    logout,
    addArticle,
    editArticle,
    deleteArticle,
    blockArticle,
    voteArticle,
    addComment,
    blockUser,
    blockComment,
  }
}