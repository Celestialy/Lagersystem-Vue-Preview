/* eslint-disable no-console */
// eslint-disable-next-line no-unused-vars
import axios from 'axios'
// eslint-disable-next-line no-unused-vars
const API_URL = process.env.VUE_APP_API_URL


export default {
    namespaced: true,
    state: {
        CategoriesList: [],
    },
    getters: {
        getCategories(state) {
            return state.CategoriesList
        }
    },
    mutations: {
        updateCategoryList(state, categories) {
            categories.forEach(item => {
                state.CategoriesList.push({ categoryId: item.categoryID, categoryName: item.categoryName, isUsed: item.isUsed });
            });
        },
        removeCategoryFromList(state, categoryId) {
            state.CategoriesList.splice(state.CategoriesList.map(x => { return x.categoryId }).indexOf(categoryId), 1)
        },
        removeCategory(state) {
            state.CategoriesList = []
        }
    },
    actions: {
        getCategories(context, params) {
            return new Promise((resolve, reject) => {
                context.commit('removeCategory')
                axios.get(API_URL + 'category/get_all_categories', params)
                    .then(response => {
                        let items = response.data
                        context.commit('updateCategoryList', items)
                        resolve(response)
                    }).catch(error => {
                        error.message = error.response.data
                        context.commit('updateCategoryList', { categoryId: 0, categoryName: 'Ingen kategorier', isUsed: false })
                        console.log(error)
                        reject(error)
                    })
            })
        },
        addCategory(context, item) {
            return new Promise((resolve, reject) => {
                axios.post(API_URL + 'category/add_category', item).then(response => {
                    resolve(response)
                }).catch(error => {
                    error.message = error.response.data
                    console.log(error)
                    reject(error)
                })
            });
        },
        editCategory(context, item) {
            return new Promise((resolve, reject) => {
                axios.put(API_URL + 'category/edit_category', item
                ).then(response => {
                    resolve(response)
                }).catch(error => {
                    error.message = error.response.data
                    console.log(error)
                    reject(error)
                })
            })
        },
        deleteCategory(context, item) {
            console.log(item)
            return new Promise((resolve, reject) => {
                axios.delete(API_URL + 'category/delete_category', {
                    params: item
                }).then(response => {
                    let deleteItem = item
                    context.commit('removeCategoryFromList', deleteItem.categoryId)
                    resolve(response)
                }).catch(error => {
                    error.message = error.response.data
                    console.log(error)
                    reject(error)
                })
            })
        }
    }
}