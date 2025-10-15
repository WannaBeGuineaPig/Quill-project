<script setup>
import { ref } from 'vue'

const props = defineProps({
  editMode: {
    type: Boolean,
    default: false
  },
  article: {
    type: Object,
    default: () => ({
      title: '',
      content: '',
      theme: ''
    })
  }
})

const emit = defineEmits(['submit', 'cancel'])

const articleData = ref({
  title: props.article.title,
  content: props.article.content,
  theme: props.article.theme
})

const themes = [
  'Технологии',
  'Наука',
  'Искусство',
  'Спорт',
  'Политика',
  'Экономика',
  'Образование',
  'Здоровье',
  'Путешествия',
  'Другое'
]

const submitForm = () => {
  emit('submit', articleData.value)
}

const cancelForm = () => {
  emit('cancel')
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
        <select 
          id="theme" 
          class="input"
          v-model="articleData.theme" 
          required
        >
          <option value="" disabled>Выберите тематику</option>
          <option v-for="theme in themes" :key="theme" :value="theme">
            {{ theme }}
          </option>
        </select>
      </div>
      
      <div class="form-group">
        <label for="content">Текст статьи:</label>
        <textarea 
          id="content" 
          class="input"
          v-model="articleData.content" 
          rows="10" 
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
</style>