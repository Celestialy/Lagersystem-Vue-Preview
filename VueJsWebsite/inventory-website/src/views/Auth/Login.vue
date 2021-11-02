<template>
<b-container>
  <div class="login">
    <div class="loginboard">
      <div class="dash-content">
        <div class="dash-header">
          <h4>Opret ny bruger</h4>
        </div>
        <div class="dash-body">
          <h5>Login</h5>
          <label>Username</label>
          <div class="form-group">
            <input v-model="username" type="text" class="form-control" placeholder="Indtast dit brugernavn her..." />
          </div>
          <label>Password</label>
          <div class="form-group">
            <input v-model="password" class="form-control" type="password" placeholder="Indtast dit password her..." />
          </div>
        </div>
        <div class="dash-footer">
          <button :disabled="IsFetching" v-on:click="login()" class="btn btn-success">Login</button>
        </div>
      </div>
    </div>

  </div>
</b-container>
</template>

<script>
  export default
    {
      name: 'Login',
      data: function () {
        return {
          username: '',
          password: '',
          IsFetching: false,
        }
      },
      methods: {
        login(){
          this.IsFetching = true
          this.$store.dispatch('auth/getToken', {
            username: this.username,
            password: this.password
          }).then(() => {
            this.IsFetching = false
            this.$router.push('/')
          }).catch(()=> {
            this.IsFetching = false
          })
        }
      }
    }
</script>