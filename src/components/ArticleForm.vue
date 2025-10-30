<script setup>
import { ref , onMounted, computed, watch} from 'vue'
import { useAppState } from '@/composables/useAppState'
const {state, getTopics, createBase64FromImage, } = useAppState()

const props = defineProps({
  editMode: {
    type: Boolean,
    default: false
  },
  article: {
    type: Object,
   default:null
  }
})

const emit = defineEmits(['adding','editing', 'cancelling'])
const hasChanges = ref(false)

const showThemeDropdown = ref(false)

const themes = computed (() => {
return [...state.categories]
})

const articleTopicId = computed(() => {
   
  console.log("choosed", selectedTheme.value)
  const ind = themes.value.findIndex(c => c === selectedTheme.value) +1
 
  console.log("id",ind)
  return ind
})
const selectedTheme = ref()

const fileInput = ref(null)
const imagePreview = ref('')

const articleData = ref({
  id:null,
  title: "",
  content: "",
  topicId: "",
  image:"",
})

const selectTheme = (theme) => {
  selectedTheme.value = theme
  showThemeDropdown.value = false
  hasChanges.value = true
  console.log('Выбрана тема:', theme)
}

const handleImageUpload = (event) => {
  const file = event.target.files[0]
  
  if (!file) return
  
  if (!file.type.startsWith('image/')) {
    alert('Пожалуйста, выберите файл изображения')
    resetFileInput()
    return
  }
  
  if (file.size > 5 * 1024 * 1024) {
    alert('Размер файла не должен превышать 5MB')
    resetFileInput()
    return
  }
  
  const reader = new FileReader()
  reader.onload = (e) => {
     const base64String = e.target.result
    imagePreview.value = base64String
    articleData.value.image = base64String.split(',')[1]
  }
  hasChanges.value=true
  reader.readAsDataURL(file)
}

const removeImage = () => {
  hasChanges=true
  imagePreview.value = ''
  articleData.value.image = null
  resetFileInput()
}

const resetFileInput = () => {
  if (fileInput.value) {
    fileInput.value.value = ''
  }
}

watch(() => props.article, (newArticle) => {
  console.log("prp",props.article)
  if (newArticle && props.editMode) {
    
    articleData.value.id = props.article.id
    articleData.value.title = props.article.title
    imagePreview.value = props.article.imageUrl
    articleData.value.content = props.article.content
    articleData.value.topicId = themes.value.findIndex(c => c===props.article.topicName) -1
     selectedTheme.value = props.article.topicName
  }
}, { immediate: true })


const textChanged =()=>{
  hasChanges=true
}

const submitForm = () => {
   const title = articleData.value.title?.trim()
  const content = articleData.value.content?.trim()
  
  if (!title) {
    alert('Название статьи не может быть пустым')
    return
  }
  
  if (!content) {
    alert('Текст статьи не может быть пустым')
    return
  }
  articleData.value.topicId = articleTopicId
  console.log(articleData)
  if(props.editMode){
    console.log("1",articleData.value.image)
    console.log("2", props.article.image )
    console.log("3",articleData.value.image==null)
    if(!articleData.value.image && props.article.image){
      console.log("uslovie")
      articleData.value.image = props.article.image
    }
    console.log(articleData)
    emit('editing',articleData.value)
  }
  else{
    emit('adding', articleData.value)
  }
  
}

const cancelForm = () => {
  const confirmLeave = confirm('У вас есть несохраненные изменения. Вы уверены, что хотите уйти? Данные будут потеряны.')
    
    if (confirmLeave){
        emit('cancelling')
    }
  
}
</script>

<template>
  <div class="article-form card">
    <h2 class="card-title brand-gradient">{{ editMode ? 'Редактировать статью' : 'Создать новую статью' }}</h2>
    
    <form @submit.prevent="submitForm">
      <div class="form-group">
        <label for="title">Название статьи:</label>
        <input 
          type="text" 
          id="title" 
          class="input"
          v-model="articleData.title" 
          required
        />
      </div>
      
      <div class="form-group">
        <label for="theme">Тематика:</label>
        <div class="custom-select-wrapper">
          <button 
            id="theme" 
            type="button"
            class="custom-select-toggle"
            @click="showThemeDropdown = !showThemeDropdown"
            @blur="showThemeDropdown = false"
          >
            <span>{{ selectedTheme || 'Выберите тематику' }}</span>
            <span class="dropdown-arrow">▼</span>
          </button>
          
          <div v-if="showThemeDropdown" class="custom-select-dropdown">
            <div
              v-for="theme in themes"
              :key="theme"
              class="dropdown-option"
              :class="{ active: selectedTheme === theme }"
              @mousedown="selectTheme(theme)"
            >
              <span class="option-label">{{ theme }}</span>
            </div>
          </div>
        </div>
      </div>

      
      
<div class="form-group">
    <label for="image">Изображение статьи:</label>
    <div class="image-upload-section">
      <!-- Загрузка файла -->
      <div class="file-input-wrapper">
        <input 
          type="file" 
          id="image" 
          ref="fileInput"
          class="file-input"
          accept="image/*"
          @change="handleImageUpload"
        />
        <label for="image" class="file-input-label">
          <span class="file-input-text">
            {{ (imagePreview || (editMode && article?.image)) ? 'Изменить изображение' : 'Выберите изображение' }}
          </span>
          <span class="file-input-button">Обзор</span>
        </label>
      </div>
      
      <!-- Предпросмотр нового изображения -->
      <div v-if="imagePreview" class="image-preview">
        <img :src="imagePreview" alt="Предпросмотр" class="preview-image" />
        <button 
          type="button" 
          class="btn btn-danger btn-small" 
          @click="removeImage"
        >
          Удалить
        </button>
      </div>
      
      <!-- Текущее изображение при редактировании -->
      <div v-if="editMode && article?.hasImage" class="current-image-info">
        <p>Текущее изображение:</p>
        <img :src="imagePreview" alt="Текущее изображение" class="current-image" />
      </div>
      
      <div class="image-hint">
          <small> *Поддерживаемые форматы: JPG, PNG, GIF. Максимальный размер: 5MB</small>
        </div>
    </div>
  </div>
      <div class="form-group">
        <label for="content">Текст статьи:</label>
        <textarea 
          id="content" 
          class="input"
          v-model="articleData.content" 
          rows="10" 
          @change="textChanged"
          required
        ></textarea>
      </div>
      
      <div class="form-actions">
        <button type="button" class="btn btn-secondary" @click="cancelForm">
          Отмена
        </button>
        <button type="submit" class="btn btn-primary">
          {{ editMode ? 'Сохранить изменения' : 'Опубликовать статью' }}
        </button>
      </div>
    </form>
  </div>
</template>

<style scoped>
.article-form { max-width: 880px; margin: 0 auto; padding: 2rem; }
.article-form h2 { margin-bottom: 1rem; text-align: center; }
.form-group { margin-bottom: 1.25rem; }
.form-group label { display:block; margin-bottom:.5rem; font-weight:600; }
.form-actions { display:flex; justify-content:flex-end; gap:1rem; margin-top: 1.5rem; }

.form-group input,.custom-select-wrapper {
  position: relative;
  width: 100%;
}
.form-group textarea, .form-group input, .custom-select-toggle {
  width: 100%;
  padding: 0.75rem;
  border: 1px solid #ddd;
  border-radius: 4px;
  background: black;
}

.custom-select-dropdown {
  max-height: 200px;
  overflow-y: auto;
}

.image-upload-section {
  display: flex;
  flex-direction: column;
}

.image-preview {
  display: flex;
  flex-direction: column;
  align-items:center;
  gap: 0.5rem;
}

.preview-image {
  max-width: 300px;
  max-height: 200px;
  border-radius: 8px;
  border: 2px solid var(--border);
}

.btn-small {
  padding: 0.25rem 0.75rem;
  font-size: 0.875rem;
}

/* Стили для поля загрузки файла */
.file-input-wrapper {
  position: relative;
}

.file-input {
  position: absolute;
  opacity: 0;
  width: 0;
  height: 0;
}

.file-input-label {
  display: inline-flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.75rem 1rem;
  background: var(--surface);
  border: 2px dashed var(--border);
  border-radius: 8px;
  cursor: pointer;
  transition: all 0.2s ease;
}

.file-input-label:hover {
  border-color: var(--accent-1);
  background: color-mix(in srgb, var(--accent-1) 10%, transparent);
}

.file-input-text {
  color: var(--text);
}

.file-input-button {
  padding: 0.25rem 0.75rem;
  background: var(--accent-1);
  color: white;
  border-radius: 4px;
  font-size: 0.875rem;
}

/* Информация о текущем изображении */
.current-image-info {
  padding: 1rem;
  background: var(--surface);
  border-radius: 8px;
  border: 1px solid var(--border);
}

.current-image {
  max-width: 200px;
  max-height: 150px;
  border-radius: 6px;
  margin-top: 0.5rem;
}

/* Подсказка */
.image-hint {
  display: flex;
  margin-top: 0.5rem;
  align-self: flex-end;
}

.image-hint small {
  color: var(--text-secondary);
  font-size: 0.875rem;
}
</style>