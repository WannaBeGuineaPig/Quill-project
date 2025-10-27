<script setup>
import { useRoute,useRouter } from 'vue-router';
import { computed, ref, onMounted } from 'vue';
import { useAppState } from '@/composables/useAppState';
import ArticleForm from '@/components/ArticleForm.vue';

const route = useRoute()
const router = useRouter()
const { isLoggedIn, state , addNewArticle, editArticleInfo, getArticleId, getTopics} = useAppState()

const isEdit =  computed(() => route.name === 'editArticle')
const articleId = isEdit.value? Number(route.params.id):null
const currentArticle = ref(null)

onMounted(async()=>{
    await getTopics()
    console.log("isEdit", isEdit)
    if(isEdit.value &&articleId) {
        try {
        
        console.log('Загрузка статьи с ID:', articleId)
        
        currentArticle.value = await getArticleId(articleId)
        console.log('Загруженная статья:', currentArticle.value)
        console.log("Фото статьи", currentArticle.value.imageUrl)}
        catch (error) {
        console.error('Ошибка загрузки:', error)
        } 
    }
})

const handleAddingNewArticle =async (data)=>{
    console.log("data",data)
    const ok = await addNewArticle(data)
     if(ok.success){
      alert("Успешно сохранено!")
      router.push({ name: 'profile' })
    }
    else{
      alert(ok.message)
    }
}

const handleEditArticle =async(data) =>{
    const ok = await editArticleInfo(data)
      if(ok.success){
      alert("Успешно сохранено!")
      router.push({ name: 'home' })
    }
    else{
      alert(ok.message)
    }
}

const handleCancelling = ()=>{
    router.push({name:"home"})
}


</script>

<template>
  <ArticleForm
    :editMode ="isEdit"
    :article ="currentArticle"
    @adding="handleAddingNewArticle"
    @editing ="handleEditArticle"
    @cancelling = "handleCancelling"
    
  />
</template>