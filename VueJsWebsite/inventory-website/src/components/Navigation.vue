<template>
    <div>
  <b-navbar type="dark" variant="dark">
    <b-navbar-nav >
        <b-nav-item exact-active-class="active" to="/" >Home</b-nav-item>
        <b-nav-item v-if="CanManagerInvetory" exact-active-class="active" to="/loan">Udlån/Forbrug</b-nav-item>
        <b-nav-item v-if="CanManagerInvetory" exact-active-class="active" to="/loanhistory">Udlåns log</b-nav-item>
        <b-nav-item v-if="CanManagerInvetory" exact-active-class="active" to="/consumtionhistory">Forbrugs log</b-nav-item>
    </b-navbar-nav>
    <b-navbar-nav class="ml-auto">
      <b-nav-item-dropdown right v-if="isAuthenticated">
          <!-- Using 'button-content' slot -->
          <template v-slot:button-content>
            <em>{{ username }}</em>
          </template>
          <b-dropdown-item v-if="isManager" to="/management" >Management</b-dropdown-item>
          <b-dropdown-item v-else-if="CanManagerInvetory" to="/management/LoanManagement" >Management</b-dropdown-item>
          <b-dropdown-item v-else-if="isAdmin && !isManager" to="/management/Admin" >Management</b-dropdown-item>
          <b-dropdown-item v-b-modal.ChangePassword>Change Password</b-dropdown-item>
          <b-dropdown-item v-on:click="logout" >Sign Out</b-dropdown-item>
        </b-nav-item-dropdown>
        <b-nav-item v-else exact-active-class="active" to="/login">Login</b-nav-item>
        <b-nav-item v-if="!isAuthenticated" exact-active-class="active" to="/register">Register</b-nav-item>
    </b-navbar-nav>
  </b-navbar>
  <ChangePassword/>
</div>
</template>
<script>

export default {
  name: 'Navigation',
  computed: {
    isAuthenticated(){
      return this.$store.getters['auth/isAuthenticated']
    },
    username(){
      return this.$store.getters['auth/username']
    },
    isManager(){
      return this.$store.getters['auth/isManager']
    },
    CanManagerInvetory(){
      return this.$store.getters['auth/CanManagerInvetory']
    },
    isAdmin() {
      return this.$store.getters['auth/isAdmin']
    }
  },
  methods: {
    logout(){
      this.$store.dispatch('auth/destroyToken').then(() => {
            this.$router.go('/')
          })
    }
  }
}
</script>