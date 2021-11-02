import store from 'src/store'

// eslint-disable-next-line no-unused-vars
function isAuth(){
    if (store.getters['auth/isAuthenticated']) {
        return true
    } else {
        return false
    }
}

const isManager = (to, from, next) => {
    
      
    if (store.getters['auth/isManager']) {
      next()
      return
    }
    next({
        path: from.path,
    })
  }

  export default{
      isManager
  }