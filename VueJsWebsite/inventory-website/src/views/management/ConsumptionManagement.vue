<template>
  <b-container>
    <div>
      <b-row>
        <b-col lg="12" style="margin-top:15px; margin-bottom:15px">
          
          <b-button
            class="float-right"
            variant="primary"
            v-b-modal.addItemModal
            style="margin-left: 10px;"
          >Tilføj ting til lager</b-button>
           <b-button
            class="float-right"
            variant="success"
            @click="showFinditem = true"
          >Find og tilføj</b-button>

          <b-row>
            <b-col lg="8" style="margin-bottom:10px">
              <b-button :disabled="isFetching" v-on:click="getItems">Opdater tabel</b-button>
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
              id="inventoryTable"
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
                  @click="
                    sortBy = '';
                    search = '';
                  "
                >Nulstil filter</b-button>
              </template>
              <template v-slot:table-busy>
                <div class="text-center text-danger my-2">
                  <b-spinner class="align-middle"></b-spinner>
                  <strong>Loading...</strong>
                </div>
              </template>
              <template v-slot:empty>
                <h4>{{ notFoundMessage }}</h4>
                <b-button v-on:click="getItems">Refresh table</b-button>
              </template>
              <template v-slot:cell(image)="data">
                <b-img class="imgThumbnail" :src="data.item.image.imageUri"></b-img>
              </template>
              <template v-slot:cell(amountLeft)="data">
                <p v-if="data.item.amountLeft > 25">{{ data.item.amountLeft }}</p>
                <p
                  v-if="data.item.amountLeft < 26 && data.item.amountLeft > 5"
                  style="color:darkOrange"
                >{{ data.item.amountLeft }}</p>
                <p v-if="data.item.amountLeft < 6" style="color:red">{{ data.item.amountLeft }}</p>
              </template>
              <template v-slot:cell(button)="data">
                <div>
                  <b-row>
                    <b-col lg="3">
                      <b-button
                        variant="primary"
                        @click="selectedItemModal(data.item, 'About')"
                      >Mere</b-button>
                    </b-col>
                    <b-col lg="3">
                      <b-button variant="warning" @click="fillEditItemModal(data.item)">Rediger</b-button>
                    </b-col>
                    <b-col lg="3">
                      <b-button variant="warning" @click="addBarcodeToItem(data.item)">Stregkoder</b-button>
                    </b-col>
                    <b-col lg="3">
                      <b-button
                        variant="danger"
                        @click="selectedItemModal(data.item, 'Delete')"
                        :disabled="data.item.barcodes.length > 0" :title="data.item.barcodes.length > 0? 'Der er stadig stejkoder' : ''"
                      >Slet</b-button>
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
            aria-controls="inventoryTable"
          />
        </b-col>
      </b-row>
    </div>
    
    <AddItem :SelectedImage="SelectedImage" :Categories="Categories" @GetItems="getItems()" :isLoan="false" @show="SelectedImage = null"></AddItem>

    <MoreAboutItem v-model="showMoreAboutItem" :SelectedItem="SelectedItem"></MoreAboutItem>

    <EditItem
      v-model="showEditItemModal"
      :SelectedItem="SelectedItem"
      :SelectedImage="SelectedImage"
      :Categories="Categories"
      @GetItems="getItems()"
    ></EditItem>

    <DeleteItem v-model="showDeleteSelectedItemModal" :SelectedItem="SelectedItem"></DeleteItem>

    <SelectImage v-model="SelectedImage"></SelectImage>
    <UploadImage :Image.sync="SelectedImage"></UploadImage>

    <addBarcodeToItemModal
      :item="selectedItem"
      v-model="showAddBarcodeToItem"
      @updateItemBarcodes="getItems"
    ></addBarcodeToItemModal>

    <Finditem v-model="showFinditem" @updateItemBarcodes="getItems"> </Finditem>
  </b-container>
</template>

<script>
import smoothReflow from 'vue-smooth-reflow'

export default {
  name: 'ConsumptionInventoryManagement',
  mixins: [smoothReflow],
  data() {
    return {
      timer: null,
      
      requestURL: '',
      modalTitle: '',
      addItemsBtn: '',
      inventoryManagement: false,

      selectedImage: null,
      selectedItem: {},

      selectedItemModalTitle: '',

      showMoreAboutItem: false,
      showAddBarcodeToItem: false,
      showEditItemModal: false,
      showDeleteSelectedItemModal: false,

      fields: [
        { key: 'image', label: '' },
        { key: 'brand', label: 'Mærke', sortable: true },
        { key: 'model', label: 'Model', sortable: true },
        { key: 'category.categoryName', label: 'Kategori', sortable: true },
        { key: 'amountLeft', label: 'Mængde', sortable: true },
        { key: 'button', label: '' }
      ],
      isFetching: true,
      failedFetches: 0,
      search: '',
      currentPage: 1,
      totalRows: 0,
      perPage: 10,
      sortBy: '',
      activeModal: '',
      Items: Array,
      notFoundMessage: '',
      showFinditem: false
    }
  },
  computed: {
    ConsumptionItems() {
      return this.$store.getters['inventoryManagement/getItems']
    },

    ConsumptionItemAmount() {
      return this.$store.getters['inventoryManagement/getItems'].length
    },

    Categories() {
      var categories = [...this.$store.getters['inventoryManagement/getCategories']]
      categories.unshift({id: null, value:"Vælg kategori"})
      return categories
    },

    SelectedItem: {
      cache: false,
      get() {
        if (Object.entries(this.selectedItem).length === 0) {
          return {
            barcodes: [],
            inventory: {
              inventoryType: ''
            },
            category: {
              categoryName: ''
            },
            image: {
              imageUri: ''
            }
          }
        } else {
          return this.selectedItem
        }
      }
    },

    SelectedImage: {
      get() {
        if (this.selectedImage != null) {
          return this.selectedImage
        } else {
          return {
            imageId: 0,
            imageName: 'Intet billede valgt',
            imageUri: process.env.VUE_APP_IMAGEPLACEHOLDER
          }
        }
      },
      set(value) {
        this.selectedImage = value
      }
    },
    
  },
  methods: {
    ShowTable() {
      this.getCategories()
      this.getItems()
    },
    getCategories() {
      this.$store
        .dispatch('inventoryManagement/getCategories')
        .then(() => {
          // eslint-disable-next-line no-console
          console.log(
            'Got some data, now lets show something in this component'
          )
          this.failedFetches = 0
        })
        .catch(() => {
          // eslint-disable-next-line no-console
          console.log('no categories found')
          this.totalRows = 0
          this.failedFetches++
          if (this.failedFetches <= 5) {
            this.timer = setTimeout(() => {
              this.getCategories()
            }, 5000)
          } else {
            this.failedFetches = 0
            this.notFoundMessage = 'Der blev ikke fundet nogen kategorier...'
          }
        })
    },
    getItems() {
      this.isFetching = true
      this.$store
        .dispatch('inventoryManagement/getConsumptionItems', 2)
        .then(() => {
          // eslint-disable-next-line no-console
          console.log(
            'Got some data, now lets show something in this component'
          )
          this.totalRows = this.ConsumptionItemAmount
          this.Items = this.ConsumptionItems
          this.isFetching = false
          this.failedFetches = 0
        })
        .catch(() => {
          // eslint-disable-next-line no-console
          console.log('no items found')
          this.Items = []
          this.totalRows = 0
          this.failedFetches++
          if (this.failedFetches <= 5) {
            this.timer = setTimeout(() => {
              this.getItems()
            }, 5000)
          } else {
            this.isFetching = false
            this.failedFetches = 0
            this.notFoundMessage =
              'Der blev ikke fundet nogen ting i lageret...'
          }
        })
      this.modalTitle = 'Tilføj ting til lager'
      this.addItemsBtn = 'Tilføj ting til lager'
      this.inventoryManagement = false
    },
    addBarcodeToItem(item) {
      Object.assign(this.selectedItem, item)
      this.showAddBarcodeToItem = true
    },
    fillEditItemModal(item) {
      Object.assign(this.selectedItem, item)
      this.SelectedImage = this.selectedItem.image
      this.showEditItemModal = true
    },
    selectedItemModal(item, modalToOpen) {
      this.selectedItemModalTitle = item.brand + ' - ' + item.model
      Object.assign(this.selectedItem, item)
      this.selectedImage = this.selectedItem.image
      this.dropdownSelected = item.category.categoryID

      if (modalToOpen == 'About') this.showMoreAboutItem = true
      if (modalToOpen == 'Delete') this.showDeleteSelectedItemModal = true
    },
    cancel() {
      clearTimeout(this.timer)
      this.failedFetches = 0
      this.isFetching = false
    },
    onFilter(filteredItems) {
      this.totalRows = filteredItems.length
      this.currentPage = 1
    }
  },
  created() {
    this.ShowTable()
  },
  destroyed() {
    clearTimeout(this.timer)
  },
  mounted() {
    this.$smoothReflow({
      el: '#inventoryTable'
    })
    this.$smoothReflow({
      el: '.tablecontainer'
    })
  }
}
</script>
