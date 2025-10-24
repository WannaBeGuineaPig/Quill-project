<script setup>
import { useRouter } from 'vue-router'
import { useAppState } from '../composables/useAppState'
import UserProfile from '../components/UserProfile.vue'

const router = useRouter()
const { state, editUser } = useAppState()

const currentUser =  state.currentUser


const handleSave = async(data) => {
  if (state.currentUser) {
    console.log(data)
    const ok = await editUser(data)

    if(ok.success){ 
      alert("Успешное изменение ")
    }
    else{
      alert(ok.message)
    }
  }
}

const handleViewArticle = (id) => {
  router.push({ name: 'article', params: { id } })
}
</script>

<template>
  <UserProfile :user ="currentUser" @editUser="handleSave" @view-article="handleViewArticle" />
</template>