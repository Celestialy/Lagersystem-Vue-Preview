/* eslint-disable no-unused-vars */
import Vue from 'vue'
import Vuex from 'vuex'
import auth from './modules/auth'
import UserManagement from './modules/UserManagement'
import LCF from './modules/LoanConsumtionForm'
import userInventory from './modules/userInventory'
import inventoryManagement from './modules/inventoryManagement'
import barcodeManagement from './modules/barcodeManagement'
import categoryManagement from './modules/categoryManagement'
import images from './modules/Images'
import stats from './modules/statistics'
import departmentManagement from './modules/departmentManagement'

Vue.use(Vuex)

export default new Vuex.Store({
  modules: {
    auth,
    UserManagement,
    LCF,
    userInventory,
    inventoryManagement,
    barcodeManagement,
    categoryManagement,
    images,
    stats,
    departmentManagement
  }
})