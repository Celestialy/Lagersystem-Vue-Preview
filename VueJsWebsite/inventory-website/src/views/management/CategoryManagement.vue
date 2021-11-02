<template>
  <b-container>
    <div>
      <b-row>
        <b-col lg="12" style="margin-top:15px; margin-bottom:15px">
          <b-button
            class="float-right"
            variant="primary"
            @click="updateCategory(Modes.add, {})"
          >Tilføj kategori</b-button>
          <b-row>
            <b-col lg="8" style="margin-bottom:10px">
              <b-button :disabled="isFetching" v-on:click="getCategories">Opdater tabel</b-button>
              <b-button v-show="isFetching" v-on:click="cancel">Cancel</b-button>
            </b-col>
            <b-col lg="4">
              <b-input-group>
                <b-form-input
                  v-model="search"
                  type="search"
                  id="filterInput"
                  placeholder="Skriv for at søge..."
                ></b-form-input>
              </b-input-group>
            </b-col>
          </b-row>

          <div class="tablecontainer" style="text-align:center">
            <b-table
              id="categoryTable"
              outlined
              :busy="isFetching"
              :items="Items"
              :fields="fields"
              :filter="search"
              show-empty
              :per-page="perPage"
              :current-page="currentPage"
              primary-key="name"
              @filtered="onFilter"
              :sort-by.sync="sortBy"
            >
              <template v-slot:head(button)>
                <b-button
                  style="width:100% margin-left:auto margin-right:auto"
                  @click="sortBy = ''; search = ''"
                >Nulstil filter</b-button>
              </template>
              <template v-slot:table-busy>
                <div class="text-center text-danger my-2">
                  <b-spinner class="align-middle"></b-spinner>
                  <strong>Loading...</strong>
                </div>
              </template>
              <template v-slot:empty>
                <h4>{{notFoundMessage}}</h4>
                <b-button v-on:click="getCategories">Refresh table</b-button>
              </template>
              <template v-slot:cell(button)="data">
                <div>
                  <b-row>
                    <b-col lg="6">
                      <b-button variant="warning" @click="updateCategory(Modes.edit, data.item)">Rediger</b-button>
                    </b-col>
                    <b-col lg="6">
                      <b-button variant="danger" :title="data.item.isUsed? 'Kategori er i brug..' : ''" :disabled="data.item.isUsed" @click="updateCategory(Modes.delete, data.item)">Slet</b-button>
                    </b-col>
                  </b-row>
                </div>
              </template>
            </b-table>
          </div>
          <b-pagination
            align="center"
            v-model="currentPage"
            :total-rows="totalRows"
            :per-page="perPage"
            aria-controls="categoryTable"
          />
        </b-col>
      </b-row>
    </div>

    <CategoryModal :SelectedCategory.sync="selectedCategory" v-model="showUpdateCategoryModal" :Mode="mode" @update="getCategories" @deleted="totalRows--"></CategoryModal>
  </b-container>
</template>

<script>
import smoothReflow from "vue-smooth-reflow";
import {Modes} from 'src/Enums/CategoryModalModes.js'

export default {
  name: "CategoryManagement",
  mixins: [smoothReflow],
  data() {
    return {
      timer: null,
      categoryName: "",
      selectedCategory: {},

      updateCategoryModalTitle: "",
      showUpdateCategoryModal: false,
      Modes,
      mode: Modes.add,
      fields: [
        { key: "categoryName", label: "Navn" },
        { key: "button", label: "" }
      ],
      isFetching: true,
      failedFetches: 0,
      search: "",
      currentPage: 1,
      totalRows: 0,
      perPage: 10,
      sortBy: "",
      activeModal: "",
      Items: Array,
      notFoundMessage: ""
    };
  },
  computed: {
    Categories() {
      return this.$store.getters["categoryManagement/getCategories"];
    },

    CategoriesAmount() {
      return this.$store.getters["categoryManagement/getCategories"].length;
    }
  },
  methods: {
    getCategories() {
      this.isFetching = true;
      this.$store
        .dispatch("categoryManagement/getCategories", { params: {
          categoryMode: 1
        }})
        .then(() => {
          // eslint-disable-next-line no-console
          console.log(
            "Got some data, now lets show something in this component"
          );
          this.totalRows = this.CategoriesAmount;
          this.Items = this.Categories;
          this.isFetching = false;
          this.failedFetches = 0;
        })
        .catch(() => {
          // eslint-disable-next-line no-console
          console.log("no items found");
          this.Items = [];
          this.totalRows = 0;
          this.failedFetches++;
          if (this.failedFetches <= 5) {
            this.timer = setTimeout(() => {
              this.getItems();
            }, 5000);
          } else {
            this.isFetching = false;
            this.failedFetches = 0;
            this.notFoundMessage =
              "Der blev ikke fundet nogen ting i lageret...";
          }
        });
    },
    updateCategory(modalMode, item) {
      this.mode = modalMode
      this.selectedCategory = item

      this.showUpdateCategoryModal = true;
    },
    cancel() {
      clearTimeout(this.timer);
      this.failedFetches = 0;
      this.isFetching = false;
    },
    onFilter(filteredItems) {
      this.totalRows = filteredItems.length;
      this.currentPage = 1;
    }
  },
  created() {
    this.getCategories();
  },
  destroyed() {
    clearTimeout(this.timer)
  },
  mounted() {
    this.$smoothReflow({
      el: "#categoryTable"
    });
    this.$smoothReflow({
      el: ".tablecontainer"
    });
  }
};
</script>