/* eslint-disable no-unused-vars */
/* eslint-disable no-console */
import axios from 'axios'
const API_URL = process.env.VUE_APP_API_URL


export default {
    namespaced: true,
    state: {
        BaseStats: {},
        LowStock: [],
        LowStockLoan: [],
        MostUsedItems: [],
        MostLoanedItems: [],
    },
    getters: {
        GetEveryStatistic(state){
            return state
        },
        IsLoadingCompleted(state){
            return {
                BaseStatsCompleted: Object.keys(state.BaseStats).length === 0? false : true,
                LowStockCompleted: Object.keys(state.LowStock).length === 0? false : true,
                LowStockLoanCompleted: Object.keys(state.LowStockLoan).length === 0? false : true,
                MostUsedItemsCompleted: Object.keys(state.MostUsedItems).length === 0? false : true,
                MostLoanedItemsCompleted: Object.keys(state.MostLoanedItems).length === 0? false : true
            } 
        }
    },
    mutations: {
        UpdateBaseStats(state, newstats)
        {
            state.BaseStats = newstats
        },
        UpdateLowStock(state, items)
        {
            for (let index = 0; index < items.length; index++) {
                items[index].id = index
            }
            state.LowStock = items
        },
        UpdateLowStockLoan(state, items)
        {
            for (let index = 0; index < items.length; index++) {
                items[index].id = index
            }
            state.LowStockLoan = items
        },
        UpdateMostLoanedItem(state, items)
        {
            
            state.MostLoanedItems = items
        },
        UpdateMostUsedItem(state, items)
        {
            for (let index = 0; index < items.length; index++) {
                items[index].id = index
            }

            state.MostUsedItems = items
        }
    },
    actions: {
        GetStatistics(context)
        {
            return new Promise((resolve, reject) => {
                let Responses
                axios.get(API_URL + 'statistics/get_inventory_statistics').then(response => {
                    context.commit('UpdateBaseStats', response.data)
                    Responses += response
                }).catch(error => {
                    error.message = error.response.data
                    console.log(error)
                    reject(error)
                })
                axios.get(API_URL + 'statistics/get_low_stock_items').then(response => {
                    context.commit('UpdateLowStock', response.data)
                    Responses += response
                }).catch(error => {
                    error.message = error.response.data
                    console.log(error)
                    reject(error)
                })
                axios.get(API_URL + 'statistics/get_low_stock_loaned_items').then(response => {
                    context.commit('UpdateLowStockLoan', response.data)
                    Responses += response
                }).catch(error => {
                    error.message = error.response.data
                    console.log(error)
                    reject(error)
                })
                axios.get(API_URL + 'statistics/get_most_loaned_item').then(response => {
                    context.commit('UpdateMostLoanedItem', response.data)
                    Responses += response
                }).catch(error => {
                    error.message = error.response.data
                    console.log(error)
                    reject(error)
                })
                axios.get(API_URL + 'statistics/get_most_used_item').then(response => {
                    context.commit('UpdateMostUsedItem', response.data)
                    Responses += response
                }).catch(error => {
                    error.message = error.response.data
                    console.log(error)
                    reject(error)
                })
                resolve(Responses)
            })
        }
    }

}
