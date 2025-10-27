<script setup>
import { onMounted, ref, computed } from 'vue'
import { useAppState } from '../composables/useAppState'

const props = defineProps({
  user: {
    type: Object,
    default: () => ({})
  }
})

const {getArticlesOnUser, formatDate} = useAppState()

const emit = defineEmits(['editUser', 'view-article'])


const visibleArticlesCount = ref(8)
const articlesPerLoad = 8
const userArticlesList = ref([])

const visibleArticles = computed(() => {
 
  return userArticlesList.value.slice(0, visibleArticlesCount.value)
})

const hasMoreArticles = computed(() => {
  return visibleArticlesCount.value < userArticlesList.value.length
})


const loadArticles = async() => {
  try{
  if (!props.user?.id) return
    const articles = await getArticlesOnUser()
    console.log('Articles loaded:', articles)
    userArticlesList.value = Array.isArray(articles) ? articles : []
  
  }
  catch(error){

    
  }

}

onMounted(async () => {
 
  await loadArticles()
})
// watch(() => props.user, (newUser) => {
//   if (newUser && newUser.email) {
//     profileData.value.email = newUser.email
//     profileData.value.nickname = newUser.nickname || ''
//   } else {
//     profileData.value.email = ""
//     profileData.value.nickname = ""
//   }
// }, { immediate: true })

const profileData = ref({
  email: props.user?.email || "",
  nickname: props.user?.nickname|| ''
})

const saveProfile = () => {
  console.log("ProfileData",profileData)
  emit('editUser', profileData.value)
  alert("Успешное сохранение")
}

const viewArticle = (articleId) => {
  emit('view-article', articleId)
}

const loadMore = () => {
  if (hasMoreArticles.value) {
    visibleArticlesCount.value += articlesPerLoad
  }
}

</script>

<template>
  <section class="profile-section card">
    <h2 class="card-title brand-gradient">Профиль пользователя</h2>
    <div class="profile-info">
      <div class="form-group">
        <label for="profile-email">Email:</label>
        <input 
          type="email" 
          id="profile-email" 
          class="input"
          v-model="profileData.email" 
        />
      </div>
      <div class="form-group">
        <label for="profile-nickname">Никнейм:</label>
        <input 
          type="text" 
          id="profile-nickname" 
          class="input"
          v-model="profileData.nickname" 
        />
      </div>
      <button class="btn btn-primary" @click="saveProfile">
        Сохранить изменения
      </button>
    </div>
    
    <div class="user-articles">
      <h3 class="card-title">Мои статьи</h3>
      <div class="articles-grid">
         <article v-for="article in visibleArticles" 
        :key="article.id"
       class="card article-card stack">
        <!-- Здесь будут статьи пользователя -->
        <!-- <div class="article-card card"> -->
          <h3 class="article-title">{{article.title}}</h3>
          <p class="article-theme">Тематика: {{article.topicName}}</p>
          <p class="article-date">Опубликовано: {{  formatDate(article.publishedAt)}}</p>
          <div class="article-rating">Лайки: {{ article.likes }} · Дизлайки: {{ article.dislikes }}</div>
          <div class="article-actions">
            <button class="btn btn-view" @click="viewArticle(article.id)">
              Просмотр
            </button>
          </div> 
        </article>
        <!-- </div> -->
       
      </div>
    </div>
    <div class="load-more" v-if="hasMoreArticles">
      <button class="btn btn-outline" @click="loadMore">
        Загрузить еще 
      </button>
    </div>
  </section>
</template>

<style scoped>
.profile-section { padding: 2rem; width: 100%; }

.profile-info {
  margin-bottom: 3rem;
}

.form-group {
  margin-bottom: 1.5rem;
}

.form-group label {
  display: block;
  margin-bottom: 0.5rem;
  font-weight: 500;
}

.form-group input {
  width: 100%;
  padding: 0.75rem;
  border: 1px solid #ddd;
  border-radius: 4px;
}
.load-more{
  display: flex;
  justify-content: center;
}
.load-more button{
  margin-top: 20px;
  color: var(--color-heading);
  background: rgba(124,58,237,0.15);
}


.user-articles h3 {
  margin-bottom: 1.5rem;
}

.articles-grid { display:grid; grid-template-columns: repeat(auto-fill, minmax(360px, 1fr)); gap: 1.25rem; }

.article-card { padding: 1.5rem; transition: transform 0.25s ease; height: 100%; display:flex; flex-direction:column; }
.article-card:hover { transform: translateY(-2px); }

.article-title { margin-bottom: 0.5rem; }

.article-theme, .article-date { opacity: .8; font-size: 0.95rem; margin-bottom: 0.5rem; }

.article-rating {
  margin: 1rem 0;
  font-weight: 500;
}

.article-actions {
  margin-top: auto;
  padding-top: 1rem;
}

/* Адаптивность */
@media (max-width: 768px) {
  .profile-section {
    padding: 1.5rem;
  }
}
</style>