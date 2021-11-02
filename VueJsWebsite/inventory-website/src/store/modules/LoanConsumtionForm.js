/* eslint-disable no-console */
import axios from 'axios'
const API_URL = process.env.VUE_APP_API_URL


export default {
    namespaced: true,
    state: {
        item: {},
        History: [],
    },
    getters: {
        ItemIsLoaded(state){
            return state.item != {}
        },
        Item(state){
            return state.item
        },
        GetItems(state)
        {
            return state.History
        }
    },
    mutations: {
        UpdateItem(state, item){
            state.item = item
        },
        UpdateHistory(state, LoanHistory){
            state.History = LoanHistory
        },
        clearHistory(state){
            state.History = []
        }
    },
    actions: {
        GetLoanHistory(context){
            return new Promise((resolve, reject) => {
                context.commit('clearHistory')
                axios.get(API_URL + 'userloan/get_all_loans').then(response => {
                    context.commit('UpdateHistory', response.data)
                    resolve(response)
                }).catch(error => {
                    console.log(error.response.data)
                    reject(error)
                })
            })
        },
        GetConsumptionHistory(context){
            return new Promise((resolve, reject) => {
                context.commit('clearHistory')
                axios.get(API_URL + 'userConsumption/get_all_userConsumptions').then(response => {
                    context.commit('UpdateHistory', response.data)
                    resolve(response)
                }).catch(error => {
                    console.log(error.response.data)
                    reject(error)
                })
            })
        },
        FindItem(context, barcode){
            return new Promise((resolve, reject) => {
                axios.get(API_URL + 'itembarcode/find_item', {
                    params: barcode
                }).then(Response => {
                    let item = Response.data
                    context.commit('UpdateItem', item)
                    resolve(item)
                }).catch(error => {
                    console.log(error.response.data)
                    reject(error)
                })
            })
        },
        LoanItem(context, UserItem){
            return new Promise((resolve, reject) => {
                axios.post(API_URL + 'userloan/add_loan', UserItem).then(response => {
                    resolve(response)
                }).catch(error => {
                    console.log(error.response.data)
                    reject(error)
                })
            })
        },
        ConsumeItem(context, UserItem){
            return new Promise((resolve, reject) => {
                axios.post(API_URL + 'userConsumption/add_userConsumption', UserItem).then(response => {
                    resolve(response)
                }).catch(error => {
                    console.log(error.response.data)
                    reject(error)
                })
            })
        },
        getReturnLoanUsers(context, barcode){
            return new Promise((resolve, reject) => {
                axios.get(API_URL + 'userloan/Find_Loans_By_Barcode', {
                    params: {
                        barcode: barcode
                    }
                }).then(response => {
                    let users = response.data
                    resolve(users)
                }).catch(error => {
                    console.log(error.response.data)
                    reject(error)
                })
            })
        },
        ReturnItem(context, UserItem){
            console.log(UserItem)
            return new Promise((resolve, reject) => {
                axios.delete(API_URL + 'userloan/delete_loan', {
                    params: UserItem
                }).then(response => {
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