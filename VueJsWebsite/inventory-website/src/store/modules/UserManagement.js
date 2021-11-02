/* eslint-disable no-unused-vars */
import axios from 'axios'
const API_URL = process.env.VUE_APP_API_URL


export default {
    namespaced: true,
    state: {
        userList: [],
        usersInDepartmentList: []
    },
    getters: {
        getUsers(state){
            return state.userList
        },
        getUsersInDepartment(state){
            return state.usersInDepartmentList
        },
        
    },
    mutations: {
        UpdateUserList(state, users){
            state.userList = users
        },
        RemoveUserFromList(state, userid)
        {
            state.userList.splice(state.userList.map(x => { return x.userId}).indexOf(userid), 1)
        },
        RemoveUsers(state){
            state.userList = []
        },

        UpdateUsersInDepartmentList(state, users){
            state.usersInDepartmentList = users
        },
        RemoveUsersInDepartmentFromList(state, userid)
        {
            state.usersInDepartmentList.splice(state.usersInDepartmentList.map(x => { return x.userId}).indexOf(userid), 1)
        },
        RemoveUsersInDepartment(state){
            state.usersInDepartmentList = []
        }
    },
    actions: {
        GetUsersWithoutDepartment(context){
            return new Promise((resolve, reject) => {
                context.commit('RemoveUsers')
                axios.get(API_URL + 'user/get_users_without_department').then(response => {
                    let users = response.data
                    context.commit('UpdateUserList', users)
                    resolve(response)
                }).catch(error => {
                    error.message = error.response.data
                    // eslint-disable-next-line no-console
                    console.log(error)
                    reject(error)
                })
            })
        },
        GetUsersFromDepartment(context){
            return new Promise((resolve, reject) => {
                context.commit('RemoveUsersInDepartment')
                axios.get(API_URL + 'User/get_users_from_department').then(response => {
                    let users = response.data
                    context.commit('UpdateUsersInDepartmentList', users)
                    resolve(users)
                }).catch(error => {
                    error.message = error.response.data
                    // eslint-disable-next-line no-console
                    console.log(error)
                    reject(error)
                })
            })
        },
        GetBasicUsersFromDepartment(context){
            return new Promise((resolve, reject) => {
                axios.get(API_URL + 'User/get_basic_users_from_department').then(response => {
                    let users = response.data
                    resolve(users)
                }).catch(error => {
                    error.message = error.response.data
                    // eslint-disable-next-line no-console
                    console.log(error)
                    reject(error)
                })
            })
        },
        addUserToDepartment(context, userid){
            return new Promise((resolve, reject) => {
                axios.post(API_URL + 'user/add_department_user', userid).then(response => {

                    context.commit('RemoveUserFromList', userid.userId)
                    resolve(response)

                }).catch(error => {
                    // eslint-disable-next-line no-console
                    console.log(error)
                    reject(error)
                })
            })
        },
        updateRoles(context, updateRolesDTO){
            return new Promise((resolve,reject) => {
                axios.post(API_URL + 'roleuser/update_user_roles', {
                    UserId: updateRolesDTO.userId,
                    Roles: updateRolesDTO.roles
                }).then(response => {
                    resolve(response)
                }).catch(error => {
                    // eslint-disable-next-line no-console
                    console.log(error)
                    reject(error)
                })
            })
        },
        RemoveUserFromDepartment(context, userid){
            return new Promise((resolve, reject) => {
                axios.put(API_URL + 'user/delete_department_user', {
                    userId: userid
                }).then(response => {
                    
                    context.commit('RemoveUserFromList', userid)
                    resolve(response)
                    
                }).catch(error => {
                    // eslint-disable-next-line no-console
                    console.log(error)
                    reject(error)
                })
            })
        },
        editUser(context, user){
            return new Promise((resolve, reject) => {
                axios.put(API_URL + 'user/edit_user', {
                    UserId: user.userId,
                    FirstName: user.firstName,
                    LastName: user.lastName,
                    Email: user.email,
                    Username: user.username
                }).then(response => {
                    resolve(response)

                }).catch(error => {
                    // eslint-disable-next-line no-console
                    console.log(error)
                    reject(error)
                })
            })
        },
        addRoleToUser(context,roleUser){
            return new Promise((resolve, reject) => {
                axios.post(API_URL + 'roleuser/add_roleuser', {
                    RoleName: roleUser.roleName,
                    UserId: roleUser.userId
                }).then(response => {
                    resolve(response)

                }).catch(error => {
                    // eslint-disable-next-line no-console
                    console.log(error)
                    reject(error)
                })
            })
        },
        resetPassword(context, resetPasswordDTO){
            return new Promise((resolve, reject) => {
                axios.post(API_URL + 'user/reset_password', resetPasswordDTO)
                .then(response => {
                    resolve(response)
                }).catch(error => {
                    // eslint-disable-next-line no-console
                    console.log(error)
                    reject(error)
                })
            })
        },
        DeleteUser(context, userid){
            return new Promise((resolve, reject) => {
                axios.delete(API_URL + 'user/delete_user', userid)
                .then(response => {
                    context.commit('RemoveUserFromList', userid)
                    resolve(response)
                }).catch(error => {
                    // eslint-disable-next-line no-console
                    console.log(error)
                    reject(error)
                })
            })
        }
    }
}