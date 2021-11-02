<template>
<b-nav  vertical class="wrapper">
   <b-button @click="isSidebarExpanded = !isSidebarExpanded">{{ isSidebarExpanded ? '-' : '+'}}</b-button>
    <div :class="sidebarClasses" class="sidebar-wrapper" >
      <b-nav-item v-if="isManager && inDepartment" exact-active-class="active" to="/management" >{{DepartmnetName}}</b-nav-item>
      <b-nav-item v-if="isManager && inDepartment" exact-active-class="active" to="/management/AddUsersToDepartment" >Tilføj til afdeling</b-nav-item>
      <b-nav-item v-if="inDepartment" exact-active-class="active" to="/management/CategoryManagement">Kategorier</b-nav-item>
      <b-nav-item v-if="inDepartment" exact-active-class="active" to="/management/ImageManagement">Billeder</b-nav-item>
      <b-nav-item v-if="inDepartment" exact-active-class="active" to="/management/LoanManagement">Udlån</b-nav-item>
      <b-nav-item v-if="inDepartment" exact-active-class="active" to="/management/ConsumptionManagement">Forbrug</b-nav-item>
      <hr v-if="isAdmin && isManager" style="height:2px;border-width:0;background-color:gray">
      <b-nav-item v-if="isAdmin" exact-active-class="active" to="/management/UserManagement">Admin</b-nav-item>
    </div>
    
</b-nav>
</template>
<script>

export default {
  name: 'sidebar',
  data() {
    return {
      isSidebarExpanded: false,
    } 
  },
  computed: {
    sidebarClasses(){
      let classes = []
      if (this.isSidebarExpanded) {
        classes.push("expanded")
      }
      return classes
    },
    
      DepartmnetName(){
        return this.$store.state.auth.user.departmentName

      },
      isManager(){
      return this.$store.getters['auth/isManager']
    },
    isAdmin() {
      return this.$store.getters['auth/isAdmin']
    },
    inDepartment() {
      return this.$store.getters['auth/inDepartment']
    }
  }
}
</script>