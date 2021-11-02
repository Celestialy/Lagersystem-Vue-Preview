/* eslint-disable no-console */
import axios from 'axios'
const API_URL = process.env.VUE_APP_API_URL


export default {
    namespaced: true,
    state: {
        ImagesList: []
    },
    getters: {
        getImages(state) {
            return state.ImagesList
        }
    },
    mutations: {
        addImageToList(state, image) {
            state.ImagesList.push(image)
        },
        updateImageList(state, images) {
            state.ImagesList = images
        },
        removeImageFromList(state, imageId) {
            state.ImagesList.splice(state.ImagesList.map(x => { return x.imageId }).indexOf(imageId), 1)
        },
        removeImage(state) {
            state.ImagesList = []
        },
    },
    actions: {
        uploadImage(context, formData) {
            return new Promise((resolve, reject) => {
                axios.post(API_URL + 'image/upload_images', formData, {
                    headers: { 'Content-Type': 'multipart/form-data' }
                }).then(response => {
                    let image = response.data
                    image = {
                        imageId: image.id,
                        imageName: image.imageName,
                        imageUri: image.imageUri,
                        departmentId: image.departmentId
                    }
                    context.commit('addImageToList', image)
                    resolve(image)
                }).catch(error => {
                    error.message = error.response.data
                    console.log(error)
                    reject(error)
                })
            })
        },

        getImages(context, params) {
            return new Promise((resolve, reject) => {
                context.commit('removeImage')
                axios.get(API_URL + 'image/get_all_images', params)
                    .then(response => {
                        let items = response.data
                        context.commit('updateImageList', items)
                        resolve(response)
                    }).catch(error => {
                        if (error.response.data.length > 0) {
                            error.message = error.response.data
                        }
                        console.log(error)
                        reject(error)
                    })
            })
        },
        deleteImage(context, imageId) {
            return new Promise((resolve, reject) => {
                axios.delete(API_URL + 'image/delete_images', {
                    params: {
                        ImageIdList: imageId
                    }
                })
                    .then(response => {
                        context.commit('removeImageFromList', imageId)
                        resolve(response)
                    }).catch(error => {
                        if (error.response.data.length > 0) {
                            error.message = error.response.data
                        }
                        console.log(error)
                        reject(error)
                    })
            })
        },
    }

}