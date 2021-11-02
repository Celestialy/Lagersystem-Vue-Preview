<template>
  <div>
    <b-container>
      <b-row>
        <b-col lg="6" class="my-1">
          <b-button-group class="mt-1">
            <b-button :disabled="isFetching" v-on:click="loadData">Opdater tabel</b-button>
            <b-button v-show="isFetching" v-on:click="cancelFetch">Cancel</b-button>
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

          <!--b-col lg="6" class="my-1 float-right">
            <b-check-group 
            size="sm">
              <b-checkbox v-model="IsNotReturnedOnly">Mangler at Aflever</b-checkbox>
            </b-check-group>
          </b-col-->
        
      </b-row>
      <b-table
        id="HistoryTable"
        outlined
        :busy="isFetching"
        :items="Items"
        :fields="fields"
        :filter="search"
        show-empty
        :per-page="perPage"
        :current-page="currentPage"
        @filtered="onFilter"
        :sort-by.sync="sortBy"
        select-mode="single"
      >
        <template v-slot:head(button)>
          <b-button @click="sortBy = ''; search = ''">Nulstil filter</b-button>
        </template>

        <template v-slot:table-busy>
          <div class="text-center text-danger my-2">
            <b-spinner class="align-middle"></b-spinner>
            <strong>Loading...</strong>
          </div>
        </template>

        <template v-slot:empty="scope">
          <h4>Vi kunne ikke finde noget forbrug...</h4>
          <b-button v-on:click="loadData">Refresh table</b-button>
        </template>

        <template v-slot:cell(image)="data">
          <b-img class="imgThumbnail" :src="data.item.consumptionItem.imageUri"></b-img>
        </template>
        <template v-slot:cell(button)="row">
          <b-button variant="primary" @click="row.toggleDetails">mere</b-button>
        </template>

        <template v-slot:row-details="row">
          <b-card>
            <b-row>
              <b-col lg="2">
                <b-img height="120" :src="row.item.consumptionItem.imageUri"></b-img>
              </b-col>
              <b-col lg="3">
                <b-row>
                  <b-col>
                    <b>Mærke:</b>
                    {{row.item.consumptionItem.brand}}
                  </b-col>
                </b-row>
                <b-row>
                  <b-col>
                    <b>Model:</b>
                    {{row.item.consumptionItem.model}}
                  </b-col>
                </b-row>
                <b-row>
                  <b-col>
                    <b>Kategori:</b>
                    {{row.item.consumptionItem.category}}
                  </b-col>
                </b-row>
                <b-row>
                  <b-col>
                    <b>Mængde:</b>
                    {{row.item.consumptionItem.amount}}
                  </b-col>
                </b-row>
              </b-col>
              <b-col>
                <b-row>
                  <b-col>
                    <b>Beskrivelse:</b>
                    {{row.item.consumptionItem.description}}
                  </b-col>
                </b-row>
                <b-row>
                  <b-col>
                    <b>Udlånt:</b>
                    {{row.item.consumptionDate}}
                  </b-col>
                </b-row>
              </b-col>
            </b-row>
          </b-card>
        </template>
      </b-table>
      <b-pagination
        v-if="totalRows > 10"
        align="center"
        v-model="currentPage"
        :total-rows="totalRows"
        :per-page="perPage"
        aria-controls="departmentTable"
      />
    </b-container>
  </div>
</template>

<script>
// eslint-disable-next-line no-unused-vars
var timer;

export default {
  name: "ConsumptionHistory",
  data() {
    return {
      fields: [
        { key: "user.username", label: "Brugernavn", sortable: true },
        { key: "user.firstName", label: "Fornavn", sortable: true },
        { key: "user.lastName", label: "Efternavn", sortable: true },
        { key: "image", label: "" },
        { key: "consumptionItem.brand", label: "Mærke", sortable: true },
        { key: "consumptionItem.model", label: "Model" },
        { key: "consumptionItem.category", label: "Kategori", sortable: true },
        { key: "consumptionItem.amount", label: "mængde", sortable: true },
        { key: "button", label: "" }
      ],
      isFetching: true,
      search: "",
      currentPage: 1,
      totalRows: 0,
      perPage: 10,
      sortBy: "",
      IsNotReturnedOnly: false
    };
  },
  computed: {
    Items() {
      return this.$store.getters["LCF/GetItems"];
    },
    ItemAmount() {
      return this.$store.getters["LCF/GetItems"].length;
    },

  },
  methods: {
    onFilter(filteredItems) {
      this.totalRows = filteredItems.length;
      this.currentPage = 1;
    },
    loadData() {
      this.isFetching = true;
      this.$store
        .dispatch("LCF/GetConsumptionHistory")
        .then(() => {
          // eslint-disable-next-line no-console
          console.log(
            "Got some data, now lets show something in this component"
          );
          this.totalRows = this.ItemAmount;
          this.isFetching = false;
          this.failedFetches = 0;
        })
        .catch(() => {
          // eslint-disable-next-line no-console
          console.log("no loans");
          this.totalRows = 0;
          this.failedFetches++;
          if (this.failedFetches <= 5) {
            timer = setTimeout(() => {
              this.loadData();
            }, 5000);
          } else {
            this.isFetching = false;
            this.failedFetches = 0;
          }
        });
    },
    cancelFetch() {
      clearTimeout(timer);
      this.failedFetches = 0;
      this.isFetching = false;
    },
  },
  created() {
    this.loadData();
  }
};
</script>