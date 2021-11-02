<template>
<b-container fluid >
  <b-row>
    <b-col lg="6" class="my-1">
            <b-button-group class="mt-1">
              <b-button :disabled="isFetching" v-on:click="loadData" >Opdater tabel</b-button>
              <b-button v-show="isFetching" v-on:click="cancel" >Cancel</b-button>
            </b-button-group>
      </b-col>

      <b-col lg="6" class="my-1">
          <b-input-group size="sm">
            <b-form-input
              v-model="search"
              type="search"
              id="filterInput"
              placeholder="Skriv for at søge..."
            ></b-form-input>
          </b-input-group>
      </b-col>
    </b-row>
    
      
    <div class="tablecontainer">
     <b-table id="departmentTable" outlined :busy="isFetching" :items="Users" :fields="fields" :filter="search"
     show-empty :per-page="perPage" :current-page="currentPage" primary-key="username" @filtered="onFilter" :sort-by.sync="sortBy" >
      <template v-slot:head(button)="data">
        <b-button @click="sortBy = ''; search = ''" >Nulstil filter</b-button>
      </template>

       <template v-slot:table-busy>
        <div class="text-center text-danger my-2">
          <b-spinner class="align-middle"></b-spinner>
          <strong> Loading...</strong>
        </div>
      </template>
      <template v-slot:empty="scope">
        <h4>Vi kunne ikke finde nogen brugere der ikke er i en afdeling...</h4>
        <b-button v-on:click="loadData" >Refresh table</b-button>
      </template>
       <template v-slot:cell(button)="data">
         <b-button v-on:click="addUserToDepartment(data.item)" >Tilføj</b-button>
      </template>
      </b-table>
    </div>
      <b-pagination align="center" v-model="currentPage" :total-rows="totalRows" :per-page="perPage" aria-controls="departmentTable"/>
</b-container>
</template>

<script>

import smoothReflow from 'vue-smooth-reflow'

export default {
  name: 'DepartmentManager',
  mixins: [smoothReflow],
  data() {
    return {
      timer: null,
      fields: [{key: 'username',  label: 'Brugernavn',sortable: true}, 
        {key: 'firstName', label: 'Fornavn', sortable: true},
        {key: 'lastName', label: 'Efternavn', sortable: true},
        'email',
        {key: 'button', label: ''}],
      isFetching: true,
      failedFetches: 0,
      search: '',
      currentPage: 1,
      totalRows: 0,
      perPage: 10,
      sortBy: '',
    }
  },
  computed: {
    Users(){
      return this.$store.getters['UserManagement/getUsers']
    },
    UserAmount(){
      return this.$store.getters['UserManagement/getUsers'].length
    }
  },
  methods: {
    addUserToDepartment(user) {
      var result = confirm('er du sikker på du vil tilføje: ' + user.firstName + ' ' + user.lastName + ' fra din afdeling')
      if (result) {
        this.$store.dispatch('UserManagement/addUserToDepartment', {
              userId: user.userId,
              departmentId: 0
            }).then(() => {
          this.totalRows--
        })
      }
    },
    loadData(){
      this.isFetching = true
      this.$store.dispatch('UserManagement/GetUsersWithoutDepartment').then(() => {
      // eslint-disable-next-line no-console
      console.log("Got some data, now lets show something in this component")
      this.totalRows = this.UserAmount
      this.isFetching = false
      this.failedFetches = 0
    }).catch(() => {
      // eslint-disable-next-line no-console
      console.log("no users")
      this.totalRows = 0
      this.failedFetches++
      if (this.failedFetches <= 5) {
        this.timer = setTimeout(() => { this.loadData()}, 5000)
      }
      else {
        this.isFetching = false
        this.failedFetches = 0
      }
    })
    },
    cancel(){
      clearTimeout(this.timer)
      this.failedFetches = 0
      this.isFetching = false
    },
    onFilter(filteredItems){
      this.totalRows = filteredItems.length
      this.currentPage = 1
    }
  },
  created(){
    this.loadData()
    
  },
  destroyed() {
    clearTimeout(this.timer)
  },
  mounted() {
    this.$smoothReflow({
      el: '#departmentTable'
    })
    this.$smoothReflow({
      el: '.tablecontainer'
    })
  }
}
</script>