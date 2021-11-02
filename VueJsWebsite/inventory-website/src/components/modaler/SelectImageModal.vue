<template>
    <b-modal
      id="selectImageModal"
      size="lg"
      title="Vælg billede"
      v-model="showSelectImageModal"
      no-close-on-backdrop
      scrollable
      ok-only
      hide-header-close
    >
      <template v-slot:modal-header>
        <b-row style="width:100%">
          <b-col lg="3">
            <h5>Vælg billede</h5>
          </b-col>
          <b-col lg="9">
            <b-form-input type="search" v-model="imageSearch" placeholder="Skriv for at søge..."></b-form-input>
          </b-col>
        </b-row>
      </template>
      <b-form-group>
        <b-row>
          <b-col lg="3" v-for="image in SearchedImages" :key="image.imageId">
            <b-form-radio
              v-model="selectedImage"
              class="item-select"
              :value="image"
              :class="{'item-select-active' :selectedImage == image}"
            >
              <label class="imageSize">{{image.imageName}}</label>
              <b-img class="img-hover" style="max-height:105px; max-width:105px" :src="image.imageUri"></b-img>
            </b-form-radio>
          </b-col>
        </b-row>
      </b-form-group>
      <template v-slot:modal-footer>
        <div class="w-100">
          <p class="float-left">Valgt billede navn: {{selectedImage.imageName}}</p>
          <b-button
            style="margin-left:10px"
            class="float-right"
            variant="primary"
            @click="okSelectImageModal"
          >Ok</b-button>
          <b-button
            style="margin-left:10px"
            class="float-right"
            @click="closeSelectImageModal"
          >Annuller</b-button>
        </div>
      </template>
    </b-modal>
</template>
<script>

// eslint-disable-next-line no-unused-vars
var timer;

export default {
    name: 'SelectImage',
    data() {
        return {
            imageSearch: "",
            showSelectImageModal: false,
            failedFetches: 0,
            selectedImage: this.Image,
            notFoundMessage: ''
        }
    },
    props: {
        Image: Object
    },
    model: {
        prop: 'Image',
        event: 'change'
    },
    watch: {
      Image(val){
        this.selectedImage = val
      }
    },
    computed: {
        SearchedImages() {
            if (this.imageSearch == "") {
                return this.Images;
            } else {
                let _imagesList = [];
                for (var i = 0; i < this.Images.length; i++) {
                if (
                    this.Images[i].imageName
                    .toLowerCase()
                    .includes(this.imageSearch.toLowerCase())
                ) {
                    _imagesList.push(this.Images[i]);
                }
                }
                return _imagesList;
            }
        },
        Images() {
      return this.$store.getters["images/getImages"];
    },
    },
    methods: {
        getImages() {
      this.$store
        .dispatch("images/getImages")
        .then(() => {
          // eslint-disable-next-line no-console
          console.log(
            "Got some data, now lets show something in this component"
          );
          this.failedFetches = 0;
        })
        .catch(() => {
          // eslint-disable-next-line no-console
          console.log("no images found");
          this.failedFetches++;
          if (this.failedFetches <= 5) {
            timer = setTimeout(() => {
              this.getImages();
            }, 5000);
          } else {
            this.failedFetches = 0;
            this.notFoundMessage = "Der blev ikke fundet nogen billeder...";
          }
        });
    },
    okSelectImageModal() {
      if (this.selectedImage.imageId != 0) {
        this.$emit('change', this.selectedImage)
        this.showSelectImageModal = false;
      } else {
        alert("Error");
      }
    },
    closeSelectImageModal() {
      this.showSelectImageModal = false;
    },
    },
    created(){
        this.getImages()
    }
}
</script>