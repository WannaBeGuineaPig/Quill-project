import axios from 'axios'

const BASE_URL = 'http://localhost:5091'

const api = axios.create({
  baseURL: BASE_URL,
  headers: {
    'Content-Type': 'application/json'
  }
})



export const registerUser = async (userData) => {
  try {
    const response = await api.post('/api/User/RegistrationUser', userData)
    console.log('Успешный ответ:', response.data)
    return response.data
  } catch (error) {
    const errorMessage = error.response?.data || 'Ошибка регистрации '+error
    console.log(error)
    throw new Error(errorMessage)
  }
}

export const loginUser = async (data) => {
  try {
    const response = await api.post('/api/User/AuthorizationUser', data)
    return response.data
  } catch (error) {
    const errorMessage = error.response?.data || 'Ошибка авторизации'+response
    throw new Error(errorMessage)
  }
}

export const changeUserInfo = async(userData) =>{
  try{
      const response = await api.put("/api/User/ChangeUserInfo", userData)
      return response.data

  }
  catch(error){
    const errorMessage = error.response?.data || 'Ошибка сохранения данных '+error
    console.log(error)
    throw new Error(errorMessage)
  }
}

export const loadArticles = async(filtrData) => {
  try{
    const params = new URLSearchParams()
    
    if (filtrData.search) params.append('search', filtrData.search)

    if (filtrData.category && filtrData.category !== -1) params.append('category', filtrData.category)

    params.append('sortBy', filtrData.sortBy || 'newest')
    console.log("params",params)

  const response = await api.get(`/api/Article/GetFilteredArticles?${params}`)
  return response.data
}
  catch(error){
     const errorMessage = error.response?.data || 'Ошибка получения данных '+error
    console.log(error)
    throw new Error(errorMessage)
  }
}

export const getArticleOnId = async(id) => {
  try{
    const response = await api.get(`/api/Article/GetArticleOnId?id=${id}`)
    return response.data
  }
  catch(error){
    const errorMessage = error.response?.data || 'Ошибка получения данных '+error
    console.log(error)
    throw new Error(errorMessage)
  }
}

export const getUserArticle = async(userId) => {
try{
    const params = new URLSearchParams()
    params.append("userId",userId)
    const response = await api.get(`/api/Article/GetUsersArticles?${params}`)
    console.log(userId)
    console.log(response)
    return response.data
  }
  catch(error){
    const errorMessage = error.response?.data || 'Ошибка получения данных '+error
    console.log(error)
    throw new Error(errorMessage)
  }

}

export const getAllTopics = async() =>{
  try{
   
    const response = await api.get("/api/Topics/GetTopics")
    
    console.log("topics",response)
     console.log("topics data",response.data)
    return response.data
  }
  catch(error){
    const errorMessage = error.response?.data || 'Ошибка получения данных '+error
    console.log(error)
    throw new Error(errorMessage)
  }
}

// Комментарии

export const getArticleComments = async (articleId) => {
  try {
    const response = await api.get(`/api/Comment/GetCommentsArticle/${articleId}`)
    return response.data
  } catch (error) {
    const errorMessage = error.response?.data || 'Ошибка получения комментариев'
    console.log(error)
    throw new Error(errorMessage)
  }
}

export const addNewComment = async (commentData) => {
  try {
    const response = await api.post('/api/Comment/AddNewComment', commentData)
    return response.data
  } catch (error) {
    const errorMessage = error.response?.data || 'Ошибка добавления комментария'
    console.log(error)
    throw new Error(errorMessage)
  }
}

export const deleteCommentApi = async (commentId) => {
  try {
    const response = await api.put('/api/Comment/ChangeStatusComment', {
      id: commentId,
      status: 'deleted'
    })
    return response.data
  } catch (error) {
    const errorMessage = error.response?.data || 'Ошибка удаления комментария'
    console.log(error)
    throw new Error(errorMessage)
  }
}

export const getLikesCount = async (articleId) => {
  try {
    const response = await api.get(`/api/Rating/GetLikeRatingsArticle/${articleId}`)
    return response.data
  } catch (error) {
    console.error('Ошибка получения лайков:', error)
    return 0
  }
}

export const getDislikesCount = async (articleId) => {
  try {
    const response = await api.get(`/api/Rating/GetDislikeRatingsArticle/${articleId}`)
    return response.data
  } catch (error) {
    console.error('Ошибка получения дизлайков:', error)
    return 0
  }
}

export const getUserRating = async (userId, articleId) => {
  try {
    const response = await api.get(`/api/Rating/GetUserArticleRating?userId=${userId}&articleId=${articleId}`)
    return response.data
  } catch (error) {
    console.error('Ошибка получения рейтинга:', error)
    return 0
  }
}

export const setUserRating = async (ratingData) => {
  try {
    const response = await api.post('/api/Rating/SetRatingUser', ratingData)
    return response.data
  } catch (error) {
    const errorMessage = error.response?.data || 'Ошибка установки рейтинга'
    console.log(error)
    throw new Error(errorMessage)
  }
}

export const deleteUserRating = async (ratingData) => {
  try {
    const response = await api.delete('/api/Rating/DeleteRating', { data: ratingData })
    return response.data
  } catch (error) {
    const errorMessage = error.response?.data || 'Ошибка удаления рейтинга'
    console.log(error)
    throw new Error(errorMessage)
  }
}
