<script setup>
import { ref, computed, onMounted } from 'vue'
import { useAppState } from '../composables/useAppState'

const { state, loadArticlesList, formatDate, getTopics, getUserArticleRating } = useAppState()

const props = defineProps({
  isLoggedIn: {
    type: Boolean,
    default: false
  }
})

const emit = defineEmits(['view-article', 'add-article'])

const searchQuery = ref('')
const selectedCategory = ref('Все')
const selectedSort = ref('newest')
const showSortDropdown = ref(false)
const visibleArticlesCount = ref(20)
const articlesPerLoad = 10
const loading = ref(false)

const userRatings = ref({})

const sortOptions = [
  { value: 'newest', label: 'Сначала новые' },
  { value: 'oldest', label: 'Сначала старые' },
  { value: 'title', label: 'По названию (А-Я)' },
  { value: 'popular', label: 'По популярности' }
]

const filteredArticles = computed(() => {
  return Array.isArray(state.articles) ? [...state.articles] : []
})

const visibleArticles = computed(() => {
  console.log(state.articles)
  return filteredArticles.value.slice(0, visibleArticlesCount.value)
})

const hasMoreArticles = computed(() => {
  return visibleArticlesCount.value < filteredArticles.value.length
})

// Функция для загрузки рейтингов пользователя для всех статей
const loadUserRatings = async () => {
  if (!props.isLoggedIn) return
  
  try {
    const ratings = {}
    for (const article of state.articles) {
      const rating = await getUserArticleRating(article.id)
      ratings[article.id] = rating
    }
    userRatings.value = ratings
    console.log('Загружены рейтинги пользователя:', ratings)
  } catch (error) {
    console.error('Ошибка загрузки рейтингов:', error)
  }
}

const getRatingClass = (articleId, ratingType) => {
  const userRating = userRatings.value[articleId]
  if (ratingType === 'likes' && userRating === 1) {
    return 'bold-rating'
  }
  if (ratingType === 'dislikes' && userRating === -1) {
    return 'bold-rating'
  }
  return ''
}

const setCategory = (cat) => {
  selectedCategory.value = cat
  const data = {
    search: searchQuery.value,
    category: cat === "Все" ? -1 : categories.value.indexOf(cat),
    sortBy: selectedSort.value,
  }
  loadArticlesList(data)
}

const applySort = (value) => {
  selectedSort.value = value
  showSortDropdown.value = false
  console.log('Применена сортировка:', value)
  const data = {
    search: searchQuery.value,
    category: selectedCategory.value === "Все" ? 0 : selectedCategory.value,
    sortBy: selectedSort.value,
  }
  console.log("sorting", data)
  loadArticlesList(data)
}

const getSelectedLabel = () => {
  const selected = sortOptions.find(opt => opt.value === selectedSort.value)
  return selected ? selected.label : 'Сортировка'
}

const searchArticles = () => {
  const data = {
    search: searchQuery.value,
    category: selectedCategory.value === "Все" ? 0 : selectedCategory.value,
    sortBy: selectedSort.value,
  }
  console.log("searching", data)
  loadArticlesList(data)
}

const viewArticle = (articleId) => {
  emit('view-article', articleId)
}

const addArticle = () => {
  
  emit('add-article')
}

const categories = computed(() => {
  if (state.categories && state.categories.length > 0) {
    let a = ['Все', ...state.categories]
    console.log("a", a)
    return ['Все', ...state.categories]
  }
  else {
    console.log(state.categories.length)
    console.log("state.categories.length <0 ")
  }
})

onMounted(async () => {
  const data = {
    search: "",
    category: -1,
    sortBy: undefined,
  }

  loading.value = true
  try {
    await getTopics()
    console.log("mounted state categories", state.categories)
    await loadArticlesList(data)
    
    await loadUserRatings()
    
    console.log("mounted", data)
  } finally {
    loading.value = false
  }
})

const loadMore = () => {
  if (hasMoreArticles.value) {
    visibleArticlesCount.value += articlesPerLoad
  }
}
</script>

<template>   
  <section class="articles-section stack">
    <div class="section-header glass row space-between">
      <h2 class="section-title brand-gradient">Все статьи</h2>
      <div class="row" style="flex:1; max-width:560px;">
        <input 
          type="text"
          class="input"
          v-model="searchQuery"
          placeholder="Поиск по названию..."
        />
        <button class="btn btn-secondary" @click="searchArticles">Поиск</button>
      </div>

      <div class="custom-select-wrapper">
        <button 
          class="custom-select-toggle"
          @click="showSortDropdown = !showSortDropdown"
          @blur="showSortDropdown = false"
        >
          <span>{{ getSelectedLabel() }}</span>
          <span class="dropdown-arrow">▼</span>
        </button>
        
        <div v-if="showSortDropdown" class="custom-select-dropdown">
          <div
            v-for="option in sortOptions"
            :key="option.value"
            class="dropdown-option"
            :class="{ active: selectedSort === option.value }"
            @mousedown="applySort(option.value)"
          >
            <span class="option-label">{{ option.label }}</span>
          </div>
        </div>
      </div>
      <button 
        v-if="isLoggedIn"
        class="btn btn-primary"
        @click="addArticle"
      >Добавить статью</button>
    </div>

    <!-- Фильтр по категориям -->
    <div class="categories row wrap">
      <button
        v-for="cat in categories"
        :key="cat"
        class="pill category-pill"
        :class="{ active: selectedCategory === cat }"
        @click="setCategory(cat)"
      >{{ cat }}</button>
    </div>    

    <div class="articles-grid grid grid-articles">
      <!-- Article Card (sample) -->
      <article v-for="article in visibleArticles" 
        :key="article.id"
        class="card article-card stack">
        <header class="card-header">
          <h3 class="card-title">{{article.title}}</h3>
          <span class="pill tag-theme">{{article.topicName}}</span>
        </header>

        <footer class="card-footer">
          <div class="card-subtitle">Опубликовано: {{formatDate(article.publishedAt)}} · Автор: {{article.authorName}}</div>
          <div class="row space-between">
            <div class="rating row ">
              <div class="pill">
                <span :class="getRatingClass(article.id, 'likes')">❤️ {{ article.likes }} </span> 
              </div>
              <div class="pill">
                <span :class="getRatingClass(article.id, 'dislikes')">💔 {{article.dislikes}}</span>
              </div>
            </div>
            <div class="article-actions">
              <button class="btn btn-secondary" @click="viewArticle(article.id)">Просмотр</button>
            </div>
          </div>
        </footer>
      </article>
    </div>

    <div v-if="loading" class="loading">
      Загрузка статей...
    </div>

    <div class="load-more" v-if="hasMoreArticles">
      <button class="btn btn-outline" @click="loadMore">
        Загрузить еще
      </button>
    </div>
  </section>
</template>

<style scoped>
.articles-section { width: 100%; }
.section-title { font-size: 1.4rem; font-weight: 800; }
.article-actions { display:flex; align-items:center; gap:.5rem; }

.section-header { border: none; box-shadow: none; outline: none; background-color: transparent;}

.custom-select-wrapper {
  position: relative;
  min-width: 160px;
}
.card-footer {  
  margin-top: auto;
}

.rating { gap: 0.3rem}

.dropdown-arrow {
  font-size: 0.7em;
  transition: transform 0.3s ease;
}

.custom-select-toggle:hover .dropdown-arrow {
  transform: translateY(1px);
}



.bold-rating {
  font-weight: bold;
  color: #bdf6ffff;
}

.loading { text-align: center; padding: 2rem; color: var(--color-text); opacity: 0.7; font-style: italic;}


/* Увеличиваем карточки и сетку */
.articles-grid.grid-articles { grid-template-columns: repeat(auto-fill, minmax(420px, 1fr)); }
.article-card { padding: 1.75rem; gap: 1rem; }
.card-title { font-size: 1.75rem; }


.category-pill { background: var(--surface); cursor: pointer; color: white !important;}
.category-pill.active { background: linear-gradient(135deg, rgba(124,58,237,0.18), rgba(6,182,212,0.18)); border-color: var(--accent-2); box-shadow: 0 0 5px 3px rgba(48, 129, 173, 0.2); color: white !important; }

.row.wrap { flex-wrap: wrap; }

@media (max-width: 768px) {
  .section-header { flex-direction: column; align-items: stretch; }
  .custom-select-wrapper { min-width: 140px; }
}
</style>
