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

