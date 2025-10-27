import { reactive, computed,ref } from 'vue'
import { registerUser, loginUser, changeUserInfo,loadArticles,getArticleOnId,
   getUserArticle, getAllTopics, getArticleComments, addNewComment, deleteCommentApi, 
   getUserRating, setUserRating, deleteUserRating, addArticle, editArticle,
   deleteArticle} from '../userApi'

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
  categories:ref([])
})

export function useAppState() {
  const isLoggedIn = computed(() => !!state.currentUser)
  const isAdmin = computed(() => state.currentUser?.role === 'admin')
  const currentUserId = computed(() => state.currentUser?.id ?? null)


  const formatDate = (dateString) => {
  if (!dateString) return ''
  
  const date = new Date(dateString)
  return date.toLocaleString('ru-RU', {
    day: '2-digit',
    month: '2-digit', 
    year: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  })
}

  const initAuth = () => {
    const savedUser = localStorage.getItem('currentUser')
    const savedToken = localStorage.getItem('token')
    console.log(savedUser)
    if (savedUser && savedToken ) {
      try {
        state.currentUser = JSON.parse(savedUser)
        console.log('Пользователь восстановлен из localStorage:', state.currentUser)
      } catch (error) {
        console.error('Ошибка восстановления пользователя:', error)
        
        localStorage.removeItem('currentUser')
        localStorage.removeItem('token')
      }
    }
  }

  const login = async (data) => {
       try {
      const result = await loginUser({email:data.email, password:data.password})
      
      state.currentUser = result
      console.log(state.currentUser)
      localStorage.setItem('currentUser', JSON.stringify(result))
      localStorage.setItem('token', result.token)
      console.log(result)
      console.log("ls",localStorage.getItem('currentUser'))
      return { success: true, message: "Успешная авторизация" }
    } catch (error) {
      return { success: false, message: error.message }
    }
    // const user = state.users.find(u => u.email === email)
    // if (!user) return false
    // if (user.status === 'blocked') return false
    // state.currentUser = user
    // return true
  }

  const register = async (data) => {
    // const exists = state.users.some(u => u.email === email)
    // if (exists) return false
    // const newUser = { id: Date.now(), email, nickname, role: 'user', status: 'active' }
    // state.users.push(newUser)
    // state.currentUser = newUser
    // return true
    try{
      const userData = {
      email:data.email,
      nickname:data.nickname,
      password:data.password,
      role:"user",
      status:"active"
    }
    console.log('Отправка данных:', userData)

    const result = await registerUser(userData)
    return { success: true, message: "Успешная регистрация" }
  }
  catch(error){
    console.log(error)
    return { success: false, message: error.message }
  }
  }

  const editUser = async(data) => {
    try{
      console.log("Data",data)
      console.log("Data.email", data.email)
       console.log("CurrentUser",state.currentUser)
       
    const userData ={
      id:state.currentUser.id,
      email:data.email,
      nickname:data.nickname,
      password:state.currentUser.password,
      status:state.currentUser.status,
      role:state.currentUser.role
    }
    console.log('Отправка данных:', userData)

    const result = await changeUserInfo(userData) 
    state.currentUser = result
      console.log(state.currentUser)
      localStorage.setItem('currentUser', JSON.stringify(result))
       localStorage.setItem('token', result.token)
    }
     catch(error){
    console.log("editUser",error)
     console.log("ls",localStorage.getItem('currentUser'))
    return { success: false, message: error.message }
  }
  }

  const logout = () => { 
    state.currentUser = null
    localStorage.removeItem('currentUser')
    localStorage.removeItem('token')
   }

  const loadArticlesList = async(filtr) =>{
    try {
      const filterParams = {
        search: filtr.search ? filtr.search: "",
        category: filtr.category !== 0 ? filtr.category : -1,
        sortBy: filtr.sortBy || 'newest',
        pageSize: 20
      }
    const result = await loadArticles(filterParams) 
    console.log("articles", result)
    state.articles = result
  }
   
    catch(error){

      console.log("error",error)
    }
  }
  const getArticleId = async(id) =>{
     try {
        const result = await getArticleOnId(id) 
        console.log("article", result)
        const article = { ...result }
        article.hasImage = false
        
        if (result.image) {
          
          article.imageUrl = await createImageUrlFromBase64(result.image)
          console.log(article.imageUrl)
          article.hasImage= true
    }
    
    return article
  }
   
    catch(error){

      console.log("error",error)
    }
  }

const createImageUrlFromBase64 = async(base64String) => {
  try {
    // Определяем MIME type по началу base64 строки
    let mimeType = 'image/jpeg' // по умолчанию
    
    if (base64String.startsWith('iVBOR')) {
      mimeType = 'image/png'
    } else if (base64String.startsWith('/9j/')) {
      mimeType = 'image/jpeg'
    } else if (base64String.startsWith('R0lGOD')) {
      mimeType = 'image/gif'
    } else if (base64String.startsWith('UklGR')) {
      mimeType = 'image/webp'
    }
    
    // ✅ Создаем Data URL
    const dataUrl = `data:${mimeType};base64,${base64String}`
    console.log(`Created ${mimeType} Data URL`)
    
    return dataUrl
    
  } catch (error) {
    console.error("Error creating image URL from base64:", error)
    return null
  }
}

const createBase64FromImage = async(file,maxSizeMB = 5) =>{
  try{
  return new Promise((resolve, reject) => {
    const maxSizeBytes = maxSizeMB * 1024 * 1024
    
    if (file.size > maxSizeBytes) {
      reject(new Error(`Размер файла превышает ${maxSizeMB}MB`))
      return
    }
    
    if (!file.type.startsWith('image/')) {
      reject(new Error('Файл должен быть изображением'))
      return
    }
    
    const reader = new FileReader()
    
    reader.onload = () => {
      const base64String = reader.result.split(',')[1]
      resolve(base64String)
    }
    
    reader.onerror = () => {
      reject(new Error('Не удалось прочитать файл'))
    }
    
    reader.readAsDataURL(file)
  })}
  catch{
    console.log("ошибка преобразования изображения")
  }
}




  const getArticlesOnUser = async() => {
    try {
      console.log("userId",state.currentUser.id)
      const result = await getUserArticle(state.currentUser.id) 
      console.log("articlesUser", result)
      return result
    }
   
    catch(error){
      console.log("error",error)
    }
  }

  const addNewArticle = async(articleData) => {
    try{
    if (!isLoggedIn.value) return false
    const newArticle = {
      id: 0, 
      title : articleData.title, 
      content : articleData.content, 
      publishedAt:'2025-12-13',
      status:"active",
      idTopics:articleData.topicId, 
      authorId: currentUserId.value, 
      image: articleData.image,

    }
    console.log("добавление статьи", newArticle)
    const result = await addArticle(newArticle)
     return { success: true, message: "Успешно сохранено" }}
    catch(error){
    console.log("addingArticle",error)
    return { success: false, message: error.message }
  }
}

  const editArticleInfo = async(articleData) => {
    try{
    if (!isLoggedIn.value) return false
    const newArticle = {
      id: articleData.id, 
      title : articleData.title, 
      content : articleData.content, 
       publishedAt:"2025-12-13",
      status:"active",
      idTopics:articleData.topicId, 
      authorId: currentUserId.value, 
      image: articleData.image,
    }
    console.log("редактирование статьи", newArticle)
    const result = await editArticle(newArticle)
     return { success: true, message: "Успешно сохранено" }}
    catch(error){
    console.log("editArticle",error)
    return { success: false, message: error.message }
  }
  }

  const deleteArticleStat = async(id) => {
   try{
      const statusData ={
        id:id,
        status:"deleted"
      }
      const result = await deleteArticle(statusData)
       return { success: true, message: "Успешно удалено" }
   }
   catch{
       return { success: false, message: error.message }
   }
  }

  const blockArticle = (id) => {
    const a = state.articles.find(a => a.id === id)
    if (a) a.status = 'blocked'
  }

  

  // const voteArticle = (id, value) => {
  //   if (!isLoggedIn.value) return false
  //   const a = state.articles.find(a => a.id === id)
  //   if (!a) return false
  //   const prev = a.votes.find(r => r.userId === currentUserId.value)
  //   if (prev) prev.value = value
  //   else a.votes.push({ userId: currentUserId.value, value })
  //   return true
  // }
  const voteArticle = async (id, value) => {
    if (!isLoggedIn.value) return false
    
    try {
      const currentRating = await getUserArticleRating(id)
      
      // Если пользователь уже поставил такой же рейтинг - удаляем его
      if (currentRating === value) {
        await removeUserArticleRating(id)
      } else {
        // Иначе устанавливаем новый рейтинг
        await setUserArticleRating(id, value)
      }
      
      return true
    } catch (error) {
      console.error("Ошибка голосования:", error)
      return false
    }
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

  const getTopics =async() =>{
    try{
    const result = await getAllTopics()
       state.categories = result.map(topic => topic.topicName)
      console.log("state categories",state.categories)
  }
    catch{}
  }

  const loadComments = async (articleId) => {
  try {
    console.log(`Загрузка комментариев для статьи ${articleId}`)
    const comments = await getArticleComments(articleId)
    console.log("Загруженные комментарии:", comments)
    return comments
  } catch (error) {
    console.error("Ошибка загрузки комментариев:", error)
    return []
  }
}

const addCommentToArticle = async (articleId, text) => {
  if (!isLoggedIn.value) {
    console.error("Пользователь не авторизован")
    return false
  }
  
  try {
    console.log(`Добавление комментария к статье ${articleId}: ${text}`)
    
    const commentData = {
      articleId: articleId,
      authorId: currentUserId.value,
      content: text
    }
    
      const result = await addNewComment(commentData)
      console.log('Комментарий добавлен:', result)
      return true
    } catch (error) {
      console.error("Ошибка добавления комментария:", error)
      return false
    }
  }

  const deleteComment = async (commentId) => {
    try {
      console.log(`Удаление комментария ${commentId}`)
      await deleteCommentApi(commentId)
      console.log('Комментарий удален')
      return true
    } catch (error) {
      console.error("Ошибка удаления комментария:", error)
      return false
    }
  }

  const getUserArticleRating = async (articleId) => {
    if (!isLoggedIn.value) return 0
    
    try {
      const rating = await getUserRating(currentUserId.value, articleId)
      return rating
    } catch (error) {
      console.error("Ошибка получения рейтинга пользователя:", error)
      return 0
    }
  }

  const setUserArticleRating = async (articleId, ratingValue) => {
    if (!isLoggedIn.value) return false
    
    try {
      const ratingData = {
        articleId: articleId,
        userId: currentUserId.value,
        rating1: ratingValue
      }
      
      await setUserRating(ratingData)
      return true
    } catch (error) {
      console.error("Ошибка установки рейтинга:", error)
      return false
    }
  }

  const removeUserArticleRating = async (articleId) => {
    if (!isLoggedIn.value) return false
    
    try {
      const ratingData = {
        articleId: articleId,
        userId: currentUserId.value
      }
      
      await deleteUserRating(ratingData)
      return true
    } catch (error) {
      console.error("Ошибка удаления рейтинга:", error)
      return false
    }
  }


  return {
    state,
    isLoggedIn,
    isAdmin,
    currentUserId,
    login,
    register,
    logout,
    deleteArticleStat,
    blockArticle,
    voteArticle,
    addComment,
    blockUser,
    blockComment,
    editUser,
    initAuth,
    loadArticlesList,
    getArticleId,
    formatDate,
    getArticlesOnUser,
    getTopics,
    loadComments,
    addCommentToArticle,
    deleteComment,
    getUserArticleRating,
    setUserArticleRating,
    removeUserArticleRating,
    createBase64FromImage,
    addNewArticle,
    editArticleInfo, 
    
  }
}
