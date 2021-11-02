/* eslint-disable no-unused-vars */
import axios from 'axios'
const API_URL = process.env.VUE_APP_API_URL

export default {
    namespaced: true,
    state: {
        departmentsList: [],
    },
    getters: {
        getDepartments(state){
            return state.departmentsList || null
        }
    },
    mutations: {
        updateDepartmentList(state, departments){
            state.departmentsList = departments
        },
        removeDepartmentFromList(state, departmentId){
            state.departmentsList.splice(state.departmentsList.map(x => { return x.departmentId}).indexOf(departmentId), 1)
        },
        removeDepartments(state){
            state.departmentsList = []
        }
    },
    actions: {
        GetDepartments(context){
            return new Promise((resolve, reject) => {
                context.commit('removeDepartments')
                axios.get(API_URL + 'department/get_all_departments').then(response => {
                    let departments = response.data
                    // eslint-disable-next-line no-console
                    context.commit('updateDepartmentList', departments)
                    resolve(departments)
                }).catch(error => {
                    error.message = error.response.data
                    // eslint-disable-next-line no-console
                    console.log(error)
                    reject(error)
                })
            })
        }
    }
}