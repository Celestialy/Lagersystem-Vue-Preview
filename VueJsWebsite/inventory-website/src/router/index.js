import Vue from 'vue'
import VueRouter from 'vue-router'
// eslint-disable-next-line no-unused-vars
import authorize from './authorize'
import Home from '../views/Home.vue'
import PageNotFound from '../views/NotFound.vue'
import Login from '../views/Auth/Login.vue'
import Register from '../views/Auth/Register.vue'
import store from 'src/store'
import management from 'src/views/management'
import department from 'src/views/management/mangement.vue'
import addUsersToDepartment from 'src/views/management/viewComponents/addUsersToDepartment.vue'
import LoanForm from 'src/views/inventoryManagement/Loan.vue'
import LoanHistory from 'src/views/inventoryManagement/LoanHistory.vue'
import ConsumtionHistory from 'src/views/inventoryManagement/ConsumtionHistory.vue'
import CategoryManagement from 'src/views/management/CategoryManagement.vue'
import loanManagement from 'src/views/management/LoanManagement.vue'
import consumptionManagement from 'src/views/management/ConsumptionManagement.vue'
import ImageManagement from 'src/views/management/ImageMangement.vue'
import UserManagement from 'src/views/admin/userManagement.vue'

Vue.use(VueRouter)

const routes = [
  {
    path: '/',
    name: 'home',
    component: Home
  },
  { path: "*", component: PageNotFound },
  {
    path: '/login',
    name: 'login',
    component: Login,
    meta: {
      requiresAnonymous: true
    },
  },
  {
    path: '/register',
    name: 'register',
    component: Register,
    meta: {
      requiresAnonymous: true
    },
  },
  {
    path: '/loan',
    name: 'loan',
    component: LoanForm,
    meta: {
      requiresAuth: true
    },
  },
  {
    path: '/loanhistory',
    name: 'loanHistory',
    component: LoanHistory,
    meta: {
      requiresAuth: true
    },
  },
  {
    path: '/consumtionhistory',
    name: 'consumtionhistory',
    component: ConsumtionHistory,
    meta: {
      requiresAuth: true
    },
  },
  {
    path: '/management',
    component: management,
    //beforeEnter: authorize.isManager,
    meta: {
      requiresAuth: true
    },
    children: [
      {
        path: '/',
        name: 'ManageDepartment',
        component: department,
      },
      {
        path: 'AddUsersToDepartment',
        name: 'addUsersToDepartment',
        component: addUsersToDepartment
      },
      {
        path: 'CategoryManagement',
        name: 'categoryManagement',
        component: CategoryManagement
      },
      {
        path: 'LoanManagement',
        name: 'loanManagement',
        component: loanManagement
      },
      {
        path: 'ConsumptionManagement',
        name: 'consumptionManagement',
        component: consumptionManagement
      },
      {
        path: 'ImageManagement',
        name: 'imageManagement',
        component: ImageManagement
      },
      {
        path: 'UserManagement',
        name: 'userManagement',
        component: UserManagement
      }
    ]
  },
  
]

const router = new VueRouter({
  mode: 'history',
  base: process.env.BASE_URL,
  routes
})

router.beforeEach((to, from, next) => {
  if (to.matched.some(record => record.meta.requiresAuth)) {
    // this route requires auth, check if logged in
    // if not, redirect to login page.
    if (!store.getters['auth/isAuthenticated']) {
      next({
        path: '/login',
      })
    } else {
      next()
    }
  } else if (to.matched.some(record => record.meta.requiresAnonymous)) {
    // this route requires auth, check if logged in
    // if not, redirect to login page.
    if (store.getters['auth/isAuthenticated']) {
      next({
        path: from.path,
      })
    } else {
      next()
    }
  } else {
    next() // make sure to always call next()!
  }
})


export default router
