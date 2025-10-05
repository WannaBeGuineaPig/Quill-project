<script setup>
import { ref } from 'vue'

const props = defineProps({
  isLoggedIn: {
    type: Boolean,
    default: false
  }
})

const emit = defineEmits(['view-article', 'add-article'])

const searchQuery = ref('')

// Категории и выбор активной
const categories = ['Все', 'Технологии', 'Наука', 'Искусство', 'Спорт', 'Политика', 'Экономика', 'Образование', 'Здоровье', 'Путешествия', 'Другое']
const selectedCategory = ref('Все')
const setCategory = (cat) => { selectedCategory.value = cat }

const searchArticles = () => {
  // Логика поиска статей
}

const viewArticle = (articleId) => {
  emit('view-article', articleId)
}

const addArticle = () => {
  emit('add-article')
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
      <article class="card article-card stack">
        <header class="card-header">
          <h3 class="card-title">Название статьи</h3>
          <span class="pill tag-theme">Технологии</span>
        </header>
        <div class="card-subtitle">Опубликовано: 01.01.2023 · Автор: username</div>
        <div class="row space-between">
          <div class="pill">👍: 0 · 👎: 0</div>
          <div class="article-actions">
            <button class="btn btn-secondary" @click="viewArticle(1)">Просмотр</button>
          </div>
        </div>
      </article>
      <!-- Здесь будут другие карточки статей -->
    </div>
  </section>
</template>

<style scoped>
.articles-section { width: 100%; }
.section-title { font-size: 1.4rem; font-weight: 800; }
.article-actions { display:flex; align-items:center; gap:.5rem; }

/* Увеличиваем карточки и сетку */
.articles-grid.grid-articles { grid-template-columns: repeat(auto-fill, minmax(420px, 1fr)); }
.article-card { padding: 1.75rem; gap: 1rem; }
.card-title { font-size: 1.75rem; }

/* Пиллы категорий */
.category-pill { background: var(--surface); cursor: pointer; }
.category-pill.active { background: linear-gradient(135deg, rgba(124,58,237,0.18), rgba(6,182,212,0.18)); border-color: var(--accent-2); }

/* Позволяем перенос строк */
.row.wrap { flex-wrap: wrap; }

@media (max-width: 768px) {
  .section-header { flex-direction: column; align-items: stretch; }
}
</style>