import axios from 'axios'
const API_URL = process.env.VUE_APP_API_URL

export default {
    namespaced: true,
    state: {
        token: localStorage.getItem('access_token') || null,
        user: {
            username: '',
            departmentName: '',
            roleNames: []
        },
    },
    getters: {
        isAuthenticated(state) {
            return state.token !== null
        },
        username(state) {
            return state.user.username || ''
        },
        isManager(state) {
            if (state.user.roleNames.includes('Manager')) {
                return true
            }
            else {
                return false
            }
        },
        CanManagerInvetory(state) {
            if (state.user.roleNames.includes('InventoryManager') || state.user.roleNames.includes('Manager')) {
                return true
            }
            else {
                return false
            }
        },
        isAdmin(state) {
            if (state.user.roleNames.includes('Admin')) {
                return true
            }
            else {
                return false
            }
        },
        inDepartment(state) {
            if (state.departmentName == "") {
                return false
            }
            else {
                return true
            }
        }
    },
    mutations: {
        getToken(state, token) {
            state.token = token
        },
        destroyToken(state) {
            state.token = null
            state.user = {
                username: '',
                departmentName: '',
                roleNames: []
            }
        },
        getUser(state, user) {
            state.user = user
        }
    },
    actions: {
        getToken(context, credentials) {
            return new Promise((resolve, reject) => {
                axios.post(API_URL + 'account/token', {
                    username: credentials.username,
                    password: credentials.password,
                }).then(response => {
                    const token = response.data

                    localStorage.setItem('access_token', token)
                    axios.defaults.headers.common['Authorization'] = 'bearer ' + token
                    context.commit('getToken', token)
                    context.dispatch('getUser').then(resolve(response))
                }).catch(error => {
                    // eslint-disable-next-line no-console
                    console.log(error)
                    reject(error)
                })
            })
        },
        refreshToken(context) {
            return new Promise((resolve, reject) => {
                axios.post(API_URL + 'account/refresh_token').then(response => {
                    const token = response.data

                    localStorage.setItem('access_token', token)
                    axios.defaults.headers.common['Authorization'] = 'bearer ' + token
                    context.commit('getToken', token)
                    context.dispatch('getUser').then(resolve(response)).catch(reject())
                }).catch(error => {
                    // eslint-disable-next-line no-console
                    console.log(error)
                    localStorage.removeItem('access_token');
                    delete axios.defaults.headers.common['Authorization']
                    context.commit('destroyToken')
                    reject(error)
                })
            })
        },
        destroyToken(context) {
            if (context.getters.isAuthenticated) {
                return new Promise((resolve) => {
                    localStorage.removeItem('access_token');
                    delete axios.defaults.headers.common['Authorization']
                    context.commit('destroyToken')
                    resolve(true)
                })
            }
        },
        getUser(context) {
            return new Promise((resolve, reject) => {
                axios.get(API_URL + 'account/getCurrentUser').then(response => {
                    let user = response.data
                    context.commit('getUser', user)
                    resolve(user)
                }).catch(error => {
                    // eslint-disable-next-line no-console
                    console.log(error)
                    reject(error)
                })
            })
        },
        changePassword(context, password) {
            return new Promise((resolve, reject) => {
                axios.post(API_URL + 'account/change_password', password).then(response => {
                    resolve(response)
                }).catch(error => {
                    // eslint-disable-next-line no-console
                    console.log(error)
                    reject(error)
                })
            })
        },
        Register(context, newUser) {
            return new Promise((resolve, reject) => {
                axios.post(API_URL + 'account/register', newUser).then(response => {
                    const token = response.data

                    localStorage.setItem('access_token', token)
                    axios.defaults.headers.common['Authorization'] = 'bearer ' + token
                    context.commit('getToken', token)
                    context.dispatch('getUser').then(resolve(response))
                }).catch(error => {
                    reject(error)
                })
            })
        }
    },
}