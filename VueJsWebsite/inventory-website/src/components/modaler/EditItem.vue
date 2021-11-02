<template>
  <b-modal
    id="editItemModal"
    size="lg"
    :title="SelectedItem.itemBrand"
    v-model="ShowModal"
    no-close-on-backdrop
    hide-header-close
  >
    <b-form>
      <b-row>
        <b-col lg="6" style="margin-bottom:15px">
          <label>Mærke</label>
          <b-form-input v-model="item.brand" type="text" required placeholder="Mærke"></b-form-input>
        </b-col>
        <b-col lg="6">
          <label>Model</label>
          <b-form-input v-model="item.model" type="text" required placeholder="Model"></b-form-input>
        </b-col>
        <b-col lg="6" style="margin-bottom:15px">
          <label>Beskrivelse</label>
          <b-form-input v-model="item.description" type="text" placeholder="Beskrivelse"></b-form-input>
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
        <b-col lg="6" v-if="isConsumtion">
            <label>Mængde</label>
            <b-form-input
              :readonly="SelectedItem.barcodes.length == 0 && SelectedItem.amountLeft == 0"
              v-model="SelectedItem.amountLeft"
              type="number"
              placeholder="Mængde"
            ></b-form-input>
          </b-col>
      </b-row>
    </b-form>
    <template v-slot:modal-footer>
      <div class="w-100">
        <b-button
          style="margin-left:10px"
          class="float-right"
          variant="primary"
          @click="editItem"
        >Rediger</b-button>
        <b-button style="margin-left:10px" class="float-right" @click="$emit('Change', false)">Annuller</b-button>
      </div>
    </template>
  </b-modal>
</template>


<script>
export default {
  name: "EditItemModal",
  data() {
    return {
        item: this.SelectedItem,
        dropdownSelected: this.SelectedItem.category.categoryID,
    };
  },
  props: {
    SelectedItem: Object,
    ShowModal: Boolean,
    Categories: Array,
    SelectedImage: Object,
  },
  model: {
    prop: "ShowModal",
    event: "Change"
  },
  computed: {
      isConsumtion: {
          cache: false, 
          get() {
              return this.SelectedItem.inventory.inventoryId == 2
          }
      }
  },
  watch: {
    SelectedItem(val){
      this.dropdownSelected = val.category.categoryID
      this.item = val
    },
    
  },
  methods: {
    editItem() {
      if (!this.isConsumtion) {
        this.$store
          .dispatch("inventoryManagement/editLoanItem", {
            itemId: this.item.itemId,
            inventoryId: 1,
            brand: this.item.brand,
            model: this.item.model,
            description: this.item.description,
            categoryId: this.dropdownSelected,
            imageId: this.SelectedImage.imageId,
          })
          .then(() => {
              this.$emit('GetItems')
            this.$emit("Change", false);
          })
          .catch(reject => {
              this.$emit('GetItems')
            alert(reject);
          });
      } else {
        this.$store
          .dispatch("inventoryManagement/editConsumptionItem", {
            itemId: this.item.itemId,
            inventoryId: 2,
            brand: this.item.brand,
            model: this.item.model,
            description: this.item.description,
            categoryId: this.dropdownSelected,
            imageId: this.SelectedImage.imageId,
            amountLeft: parseInt(this.item.amountLeft)
          })
          .then(() => {
            this.$emit('GetItems')
            this.$emit("Change", false);
          })
          .catch(reject => {
            this.$emit('GetItems')
            alert(reject);
          });
      }
    }
  }
};
</script>