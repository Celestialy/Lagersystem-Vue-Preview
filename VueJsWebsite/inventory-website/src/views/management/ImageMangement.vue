<template>
  <b-container>
    <div>
      <b-row>
        <b-col lg="12" style="margin-top:15px; margin-bottom:15px">
          <b-button class="float-right" variant="primary" v-b-modal.UploadImageModal>Upload billed</b-button>
          <b-row>
            <b-col lg="8" style="margin-bottom:10px">
              <b-button :disabled="isFetching" v-on:click="getImages">Opdater tabel</b-button>
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
              id="imageTable"
              outlined
              :busy="isFetching"
              :items="Images"
              :fields="fields"
              :filter="search"
              show-empty
              :per-page="perPage"
              :current-page="currentPage"
              primary-key="imageId"
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
                <h4>Ingen billeder fundet!</h4>
                <b-button v-on:click="getImages">Refresh table</b-button>
              </template>
              <template v-slot:cell(image)="data">
                <b-img class="imgThumbnail" :src="data.item.imageUri"></b-img>
              </template>
              <template v-slot:cell(button)="data">
                <div>
                  <b-row>
                    <b-col lg="6">
                      <b-button variant="danger" :title="data.item.isUsed? 'Billed er i brug..' : ''" :disabled="data.item.isUsed" @click="deleteImage(data.item)">Slet</b-button>
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
            aria-controls="imageTable"
          />
        </b-col>
      </b-row>
    </div>
    <UploadImage></UploadImage>
  </b-container>
</template>

<script>
import smoothReflow from "vue-smooth-reflow";
export default {
  name: 'ImageManagement',
    mixins: [smoothReflow],
  data() {
    return {
      timer: null,
      currentPage: 1,
      totalRows: 0,
      perPage: 10,
      isFetching: false,
      fields: [
        { key: 'image', label: '' },
        { key: 'imageName', label: 'Navn', sortable: true },
        { key: 'button', label: '' }
      ],
      search: '',
      sortBy: ''
    }
  },
  computed: {
    Images() {
      return this.$store.getters['images/getImages']
    },
    test(test)
    {
      if (test) {
        return 'Billed er i brug'
      } else {
        return ''
      }
    }
  },
  methods: {
      deleteImage(image){
          var result = confirm('er du sikker på du vil delete: ' + image.imageName)
          if (result) {
              this.$store.dispatch('images/deleteImage', image.imageId).catch(msg =>
              alert(msg))
          }
      },
    getImages() {
      this.isFetching = true
      this.$store
        .dispatch('images/getImages', { params: {
          getImageMode: 1
        }})
        .then(() => {
          // eslint-disable-next-line no-console
          console.log(
            'Got some data, now lets show something in this component'
          )
          this.totalRows = this.Images.length
          this.failedFetches = 0
          this.isFetching = false
        })
        .catch(() => {
          // eslint-disable-next-line no-console
          console.log('no images found')
          this.failedFetches++
          if (this.failedFetches <= 5) {
            this.timer = setTimeout(() => {
              this.getImages()
            }, 5000)
          } else {
            this.failedFetches = 0
            this.isFetching = false
          }
        })
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
  destroyed() {
    clearTimeout(this.timer)
  },
  created(){
      this.getImages()
  },
  mounted() {
    this.$smoothReflow({
      el: ".tablecontainer"
    });
  }
}
</script>
