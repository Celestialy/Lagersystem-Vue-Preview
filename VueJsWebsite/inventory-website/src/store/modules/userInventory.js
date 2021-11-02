import axios from 'axios'
const API_URL = process.env.VUE_APP_API_URL


export default {
    namespaced: true,
    state: {
        UserLoansList: []
    },
    getters: {
        getUserLoans(state) {
            return state.UserLoansList
        }
    },
    mutations: {
        updateUserLoansList(state, loans) {
            state.UserLoansList = loans
        },
        removeUserLoanFromList(state, loanId) {
            state.UserLoansList.splice(state.UserLoansList.map(x => { return x.loanId }).indexOf(loanId), 1)
        },
        removeUserLoans(state) {
            state.UserLoansList = []
        }
    },
    actions: {
        GetUserLoans(context, showAvailableItems) {
            return new Promise((resolve, reject) => {
                context.commit('removeUserLoans')
                axios.get(API_URL + 'userloan/get_all_loans', {
                    params: {
                        showAvailableItems: showAvailableItems
                    }
                }).then(response => {
                    let userLoans = response.data
                    context.commit('updateUserLoansList', userLoans)
                    resolve(response)
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