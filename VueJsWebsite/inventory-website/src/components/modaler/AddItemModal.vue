<template>
  <b-modal
    id="addItemModal"
    v-model="showAddItemModal"
    cancel-title="Annuller"
    @cancel="closeModal"
    @ok="addItem"
    no-close-on-backdrop
    size="lg"
    title="Tilføj ting til lager"
    @show="$emit('show')"
  >
    <b-form>
      <b-row>
        <b-col lg="6" style="margin-bottom:15px">
          <label>Mærke</label>
          <b-form-input v-model="itemBrand" type="text" required placeholder="Mærke"></b-form-input>
        </b-col>
        <b-col lg="6">
          <label>Model</label>
          <b-form-input v-model="itemModel" type="text" required placeholder="Model"></b-form-input>
        </b-col>
        <b-col lg="6" style="margin-bottom:15px">
          <label>Beskrivelse</label>
          <b-form-input v-model="itemDescription" type="text" placeholder="Beskrivelse"></b-form-input>
        </b-col>
        <b-col lg="6">
          <label>Kategori</label>
          <b-form-select
            v-model="dropdownSelected"
            :options="Categories"
            value-field="id"
            text-field="value"
          ></b-form-select>
        </b-col>
        <b-col lg="4">
          <b-button style="width:130px" v-b-modal.selectImageModal>Vælg billede</b-button>
          <br />
          <b-button style="width:130px; margin-top:15px" v-b-modal.UploadImageModal>Upload billede</b-button>
        </b-col>
        <b-col lg="2">
          <b-img style="height:105px" fluid :src="SelectedImage.imageUri"></b-img>
        </b-col>
      </b-row>
    </b-form>
    <template v-slot:modal-footer>
      <div class="w-100">
        <b-button
          style="margin-left:10px"
          class="float-right"
          variant="success"
          @click="addItem"
        >Tilføj</b-button>
        <b-button style="margin-left:10px" class="float-right" @click="closeModal">Annuller</b-button>
      </div>
    </template>
  </b-modal>
</template>

<script>
export default {
  name: "AddItem",
  data() {
    return {
      showAddItemModal: false,
      dropdownSelected: null,
      itemBrand: "",
      itemModel: "",
      itemDescription: ""
    };
  },
  props: {
    SelectedImage: Object,
    isLoan: Boolean,
    Categories: Array
  },
  methods: {
    addItem() {
      if (this.dropdownSelected != null) {
        if (this.isLoan) {
          this.$store
            .dispatch("inventoryManagement/addLoanItem", {
              inventoryId: 1,
              brand: this.itemBrand,
              model: this.itemModel,
              description: this.itemDescription,
              categoryId: this.dropdownSelected,
              imageId: this.SelectedImage.imageId
            })
            .then(() => {
              this.itemBrand = "";
              this.itemModel = "";
              this.itemDescription = "";
              this.selectedImage = null;

              this.$emit("GetItems");
              this.showAddItemModal = false;
            })
            .catch(reject => {
              alert(reject);
            });
        } else {
          this.$store
            .dispatch("inventoryManagement/addConsumptionItem", {
              inventoryId: 2,
              brand: this.itemBrand,
              model: this.itemModel,
              description: this.itemDescription,
              categoryId: this.dropdownSelected,
              imageId: this.SelectedImage.imageId
            })
            .then(() => {
              this.itemBrand = "";
              this.itemModel = "";
              this.itemDescription = "";
              this.selectedImage = null;

              this.$emit("GetItems");
              this.showAddItemModal = false;
            })
            .catch(reject => {
              alert(reject);
            });
        }
      }
    },
    closeModal() {
      this.dropdownSelected = null;
      this.itemBrand = "";
      this.itemModel = "";
      this.itemDescription = "";
      this.showAddItemModal = false;
    }
  }
};
</script>