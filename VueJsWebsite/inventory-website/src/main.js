import Vue from 'vue'
import App from './App.vue'
import router from './router'
import store from './store'
import BootstrapVue from 'bootstrap-vue'
import axios from 'axios'
import changepassword from 'components/modaler/ChangePassword.vue'
import resetpassword from 'components/modaler/ResetPassword.vue'
import SelectUser from 'components/modaler/selectUserModal.vue'
import addBarcodeToItemModal from 'components/modaler/AddBarcodeToItemModal.vue'
import UploadImage from 'components/modaler/UploadImage.vue'
import SelectImage from 'components/modaler/SelectImageModal.vue'
import DeleteItem from 'components/modaler/DeleteItemModal.vue'
import EditItem from 'components/modaler/EditItem.vue'
import MoreAboutItem from 'components/modaler/MoreAboutItem.vue'
import AddItem from 'components/modaler/AddItemModal.vue'
import CategoryModal from 'components/modaler/categoryAddEditDeleteModal.vue'
import Finditem from 'components/modaler/FindItemByBarcode.vue'

Vue.config.productionTip = false
Vue.use(BootstrapVue)



import { BootstrapVueIcons } from 'bootstrap-vue'
Vue.use(BootstrapVueIcons)

const token = localStorage.getItem('access_token')
if (token) {
  axios.defaults.headers.common['Authorization'] = 'bearer ' + token
  store.dispatch("auth/refreshToken").catch(() => {
    router.push('/login')
  })
}

Vue.component('ChangePassword', changepassword)
Vue.component('ResetPassword', resetpassword)
Vue.component('SelectUserModal', SelectUser)
Vue.component('addBarcodeToItemModal', addBarcodeToItemModal)
Vue.component('UploadImage', UploadImage)
Vue.component('SelectImage', SelectImage)
Vue.component('DeleteItem', DeleteItem)
Vue.component('EditItem', EditItem)
Vue.component('MoreAboutItem', MoreAboutItem)
Vue.component('AddItem', AddItem)
Vue.component('CategoryModal', CategoryModal)
Vue.component('Finditem', Finditem)


new Vue({
  router,
  store,
  render: h => h(App)
}).$mount('#app')
