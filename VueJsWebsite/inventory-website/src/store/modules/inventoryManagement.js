/* eslint-disable no-console */
// eslint-disable-next-line no-unused-vars
import axios from 'axios'
const API_URL = process.env.VUE_APP_API_URL


export default {
    namespaced: true,
    state: {
        ItemsList: [],
        CategoriesList: [],
    },
    getters: {
        getItems(state) {
            return state.ItemsList
        },
        getCategories(state) {
            return state.CategoriesList
        }
    },
    mutations: {
        updateCategoryList(state, categories) {
            categories.forEach(item => {
                state.CategoriesList.push({ id: item.categoryID, value: item.categoryName });
            });
        },
        removeCategoryFromList(state, categoryId) {
            state.CategoriesList.splice(state.CategoryList.map(x => { return x.categoryId }).indexOf(categoryId), 1)
        },
        removeCategory(state) {
            state.CategoriesList = []
        },
        updateItemList(state, items) {
            state.ItemsList = items
        },
        removeItemFromList(state, itemId) {
            state.ItemsList.splice(state.ItemsList.map(x => { return x.itemId }).indexOf(itemId.itemId), 1)
        },
        removeItems(state) {
            state.ItemsList = []
        }
    },
    actions: {
        getCategories(context) {
            return new Promise((resolve, reject) => {
                context.commit('removeCategory')
                axios.get(API_URL + 'category/get_all_categories')
                    .then(response => {
                        let items = response.data
                        context.commit('updateCategoryList', items)
                        resolve(response)
                    }).catch(error => {
                        error.message = error.response.data
                        context.commit('updateCategoryList', { categoryID: 0, categoryName: 'Ingen kategorier' })
                        console.log(error)
                        reject(error)
                    })
            })
        },

        getLoanItems(context, inventoryId) {
            return new Promise((resolve, reject) => {
                context.commit('removeItems')
                axios.get(API_URL + 'loanItem/get_all_loan_items', {
                    params: {
                        inventoryId: inventoryId
                    }
                }).then(response => {
                    let items = response.data
                    context.commit('updateItemList', items)
                    resolve(response)
                }).catch(error => {
                    error.message = error.response.data
                    console.log(error)
                    reject(error)
                })
            })
        },
        addLoanItem(context, item) {
            return new Promise((resolve, reject) => {
                axios.post(API_URL + 'loanitem/add_loan_item', item).then(response => {
                    resolve(response)
                }).catch(error => {
                    error.message = error.response.data
                    console.log(error)
                    reject(error)
                })
            })
        },
        editLoanItem(context, item) {
            return new Promise((resolve, reject) => {
                axios.put(API_URL + 'loanItem/edit_loan_item', item
                ).then(response => {
                    resolve(response)
                }).catch(error => {
                    error.message = error.response.data
                    console.log(error)
                    reject(error)
                })
            })
        },
        deleteLoanItem(context, item) {
            return new Promise((resolve, reject) => {
                axios.delete(API_URL + 'loanItem/delete_loan_item', {
                    params: item
                }).then(response => {
                    context.commit('removeItemFromList', item)
                    resolve(response)
                }).catch(error => {
                    error.message = error.response.data
                    console.log(error)
                    reject(error)
                })
            })
        },

        getConsumptionItems(context, inventoryId) {
            return new Promise((resolve, reject) => {
                context.commit('removeItems')
                axios.get(API_URL + 'consumptionItem/get_all_consumption_items', {
                    params: {
                        inventoryId: inventoryId
                    }
                }).then(response => {
                    let items = response.data
                    context.commit('updateItemList', items)
                    resolve(response)
                }).catch(error => {
                    error.message = error.response.data
                    console.log(error)
                    reject(error)
                })
            })
        },
        addConsumptionItem(context, item) {
            return new Promise((resolve, reject) => {
                axios.post(API_URL + 'consumptionitem/add_consumption_item', item).then(response => {
                    resolve(response)
                }).catch(error => {
                    error.message = error.response.data
                    console.log(error)
                    reject(error)
                })
            })
        },
        editConsumptionItem(context, item) {
            return new Promise((resolve, reject) => {
                axios.put(API_URL + 'consumptionItem/edit_consumption_item', item
                ).then(response => {
                    resolve(response)
                }).catch(error => {
                    error.message = error.response.data
                    console.log(error)
                    reject(error)
                })
            })
        },
        deleteConsumptionItem(context, item) {
            return new Promise((resolve, reject) => {
                axios.delete(API_URL + 'consumptionItem/delete_consumption_item', {
                    params: item
                }).then(response => {
                    context.commit('removeItemFromList', item)
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