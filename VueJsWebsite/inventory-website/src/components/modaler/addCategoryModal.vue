<template>
  <b-modal
    id="addCategory"
    v-model="showAddCategory"
    no-close-on-backdrop
    size="lg"
    title="Opret Kategori"
  >
    <label style="font-weight:bold">Navn</label>
    <b-form-input id="addCategory" required v-model="categoryName" placeholder="Kategorinavn..."></b-form-input>
    <b-row style="margin-top:15px">
      <b-col lg="3">
        <label style="font-weight:bold">Mærke</label>
        <br />
        <label>{{item.brand}}</label>
        <br />
        <label style="font-weight:bold; margin-top:15px">Model</label>
        <br />
        <label>{{item.model}}</label>
        <br />
      </b-col>
    </b-row>
    <template v-slot:modal-footer>
      <div class="w-100">
        <b-button
          style="margin-left:10px"
          class="float-right"
          variant="primary"
          @click="addCategory"
        >Opret Kategori</b-button>
        <b-button
          style="margin-left:10px"
          class="float-right"
          @click="closeAddCategoryModal"
        >Annuller</b-button>
      </div>
    </template>
  </b-modal>
</template>

<script>
export default {
  name: "addCategoryModal",
  data() {
    return {
      showAddCategory: this.value
    };
  },
  props: {
    item: { type: Object, required: true },
    value: { type: Boolean, default: false }
  },
  model: {
    prop: "value",
    event: "input"
  },
  watch: {
    value(val) {
      this.showAddCategory = val;
    },
    showAddCategory() {
      this.$emit("input", this.showAddCategory);
    },
    amount(val) {
      if (val == "") {
        this.amount = 0;
      } else {
        this.amount = parseInt(val);
      }
    }
  },
  computed: {
    ImageUri: {
      cache: false,
      get: function() {
        if (Object.keys(this.localitem).length === 0) {
          return "https://inventoryimagestorage.blob.core.windows.net/public-images/ImagePlaceholder.png";
        } else {
          return this.item.image.imageUri;
        }
      }
    },
    Catagory: {
      cache: false,
      get: function() {
        if (Object.keys(this.localitem).length === 0) {
          return "";
        } else {
          return this.item.category.categoryName;
        }
      }
    },
    IsLoan: {
      cache: false,
      get: function() {
        if (Object.keys(this.localitem).length === 0) {
          return "";
        } else if (this.item.inventory.inventoryType == "Loan") {
          return true;
        } else {
          return false;
        }
      }
    }
  },
  methods: {
    addBarcode() {
      this.$store
        .dispatch("barcodes/addBarcodeToItem", {
          itemId: this.item.itemId,
          barcode: this.barcode,
          isLoanItem: this.IsLoan,
          amount: this.amount
        })
        .then(() => {
          this.barcode = "";
          this.amount = 0;
          this.showAddCategory = false;
          this.$emit("updateItemBarcodes");
        })
        .catch(reject => {
          alert(reject);
        });
    },
    removeBarcode() {
      this.$store
        .dispatch("barcodes/removeBarcodeFromItem", {
          barcode: this.barcode,
          isConsumptionItem: !this.IsLoan,
          amount: this.amount
        })
        .then(() => {
          this.barcode = "";
          this.amount = 0;
          this.showAddCategory = false;
          this.$emit("updateItemBarcodes");
        });
      this.selectedItem = Object;
      this.showDeleteSelectedItemModal = false;
    },
    closeBarcodeModal() {
      this.barcode = "";
      this.amount = 0;
      this.showAddCategory = false;
    }
  },
  mounted() {
    window.addEventListener("keypress", e => {
      var y = String.fromCharCode(e.keyCode);
      if (e.keyCode == 13 && this.barcodeIsReading) {
        this.barcodeIsReading = false;
      }
      if (this.barcodeIsReading) this.barcode += y;
      if (y == "ß") {
        this.barcodeIsReading = true;
        this.barcode = "";
      }
    });
  }
};
</script>