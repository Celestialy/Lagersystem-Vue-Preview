/* eslint-disable no-console */
// eslint-disable-next-line no-unused-vars
import axios from 'axios'
// eslint-disable-next-line no-unused-vars
const API_URL = process.env.VUE_APP_API_URL

export default {
    namespaced: true,
    state: {
        BarcodesList: []
    },
    getters: {
        getBarcodes(state) {
            return state.BarcodesList
        }
    },
    mutations: {
        updateItemList(state, barcodes) {
            state.BarcodesList = barcodes
        },
        removeItemFromList(state, barcodeId) {
            state.BarcodesList.splice(state.BarcodesList.map(x => { return x.barcodeId }).indexOf(barcodeId), 1)
        },
        removeItems(state) {
            state.BarcodesList = []
        }
    },
    actions: {
        addBarcodeToItem(context, barcode) {
            return new Promise((resolve, reject) => {
                axios.post(API_URL + 'itemBarcode/add_barcode_to_item', barcode).then(response => {
                    resolve(response)
                }).catch(error => {
                    error.message = error.response.data
                    console.log(error)
                    reject(error)
                })
            })
        },
        removeBarcodeFromItem(context, item) {
            return new Promise((resolve, reject) => {
                axios.delete(API_URL + 'itemBarcode/remove_barcode_from_item', {
                    params: item
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