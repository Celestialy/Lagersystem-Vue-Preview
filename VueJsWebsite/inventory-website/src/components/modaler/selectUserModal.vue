<template>
    <b-modal id="SelectUserModal" 
    no-close-on-backdrop
    title="Select user."
    size="lg"
    v-model="Showmodal"
    >
     <b-row>
      <b-col lg="6" class="my-1">
        <b-button-group class="mt-1">
          <b-button :disabled="isFetching" v-on:click="$emit('load')">Opdater tabel</b-button>
          <b-button v-show="isFetching" v-on:click="$emit('cancel')">Cancel</b-button>
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
      <b-table
        id="departmentTable"
        outlined
        :busy="isFetching"
        :items="UserList"
        :fields="fields"
        :filter="search"
        show-empty
        :per-page="perPage"
        :current-page="currentPage"
        primary-key="username"
        @filtered="onFilter"
        :sort-by.sync="sortBy"
      >
        <template v-slot:head(button)="data">
          <b-button @click="sortBy = ''; search = ''">Nulstil filter</b-button>
        </template>

        <template v-slot:table-busy>
          <div class="text-center text-danger my-2">
            <b-spinner class="align-middle"></b-spinner>
            <strong>Loading...</strong>
          </div>
        </template>
        <template v-slot:empty="scope">
          <h4>Vi kunne ikke finde nogen brugere der ikke er i en afdeling...</h4>
          <b-button v-on:click="loadData">Refresh table</b-button>
        </template>
        <template v-slot:cell(button)="data">
          <b-col lg="4">
                <b-button variant="primary" @click="select(data.item)">Vælg</b-button>
          </b-col>
        </template>
      </b-table>
    </div>
    <b-pagination v-if="totalRows > 10"
      align="center"
      v-model="currentPage"
      :total-rows="totalRows"
      :per-page="perPage"
      aria-controls="departmentTable"
    />
     <template  v-slot:modal-footer="{cancel}">
            <b-button class="float-right" variant="danger" @click="cancel()">Cancel</b-button>
       </template>
    </b-modal>
</template>


<script>
export default {
    name: 'SelectUser',
    data () {
        return {
            search: '',
            fields: [
                { key: 'username', label: 'Brugernavn', sortable: true },
                { key: 'firstName', label: 'Fornavn', sortable: true },
                { key: 'lastName', label: 'Efternavn', sortable: true },
                { key: 'button', label: '' }
            ],
            currentPage: 1,
            perPage: 10,
            sortBy: '',
            Showmodal: false,
            localValue: this.value,
        }
    },
    model: {
        prop: 'value',
        event: 'input'
    },
    props: {
        value: {
            type: Object,
            required: true
        },
        UserList: {
            type: Array,
            required: true
        },
        isFetching: {
            type: Boolean,
            default: false
        },
        //canselFetch: Function,
        //loadData: Function,
    },
    watch: {
        value(newVal){
            this.localValue = newVal
        },
        localValue(){
            this.$emit('input', this.localValue)
        }
    },
    computed: {
        totalRows(){
            return this.UserList.length
        }
    },
    methods: {
        select(user){
            this.localValue = user
            this.Showmodal = false
        },
        
        onFilter(filteredItems) {
            this.totalRows = filteredItems.length;
            this.currentPage = 1;
        },
    },
    created(){
    }
}
</script>