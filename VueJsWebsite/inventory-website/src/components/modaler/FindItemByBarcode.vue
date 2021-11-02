<template>
  <b-modal
  hide-header-close
    id="FindItemByBarcode"
    v-model="showFindItemByBarcode"
    no-close-on-backdrop
    size="lg"
    title="Tilføj nye ting"
  >
    <label style="font-weight:bold">Stregkode</label>
    <b-form-input
      id="addBarcode"
      readonly
      required
      v-model="barcode"
      placeholder="Scan strejkode..."
    ></b-form-input>
    <b-row style="margin-top:15px" v-if="ItemIsLoaded">
      <b-col lg="3">
        <b-img fluid :src="ImageUri" style="height:150px"></b-img>
      </b-col>
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
      <b-col lg="3" style="text-align:center">
        <label style="font-weight:bold">Kategori</label>
        <br />
        <label>{{item.category}}</label>
        <br />
        <label style="font-weight:bold; margin-top:15px">Antal</label>
        <br />
        <label>{{item.amountLeft}}</label>
      </b-col>
      <b-col lg="3">
        <label style="font-weight:bold; margin-top:80px">Tilføj eller fjern</label>
        <b-form-input v-model="amount" type="number" placeholder="Tilføj eller fjern"></b-form-input>
      </b-col>
    </b-row>
    <template v-slot:modal-footer>
      <div class="w-100">
        <b-button
          style="margin-left:10px"
          class="float-right"
          variant="primary"
          @click="addAmountToItem"
        >Tilføj ting</b-button>
        <b-button style="margin-left:10px" class="float-right" @click="closeBarcodeModal">Annuller</b-button>
      </div>
    </template>
    
  </b-modal>
</template>

<script>
export default {
  name: "FindItemByBarcode",
  data() {
    return {
      showFindItemByBarcode: this.value,
      selectedImage: process.env.VUE_APP_IMAGEPLACEHOLDER,
      localIsLoanItem: this.isLoanItem,
      barcode: "",
      barcodeIsReading: false,
      amount: 0,
      item: {}
    };
  },
  props: {
    value: { type: Boolean, default: false }
  },
  model: {
    prop: "value",
    event: "input"
  },
  watch: {
    value(val) {
      this.showFindItemByBarcode = val;
    },
    showFindItemByBarcode() {
      this.$emit("input", this.showFindItemByBarcode);
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
    ItemIsLoaded() {
      return !(
        Object.entries(this.item).length === 0 &&
        this.item.constructor === Object
      )
    },
    ImageUri: {
      cache: false,
      get: function() {
        if (Object.keys(this.item).length == 0) {
          return process.env.VUE_APP_IMAGEPLACEHOLDER;
        } else {
          return this.item.image.imageUri;
        }
      }
    },
    Catagory: {
      cache: false,
      get: function() {
        if (Object.keys(this.item).length === 0) {
          return "";
        } else {
          return this.item.category.categoryName;
        }
      }
    },
    IsLoan: {
      cache: false,
      get: function() {
        if (Object.keys(this.item).length === 0) {
          return "";
        } else if (this.item.inventory.inventoryType == "Loan") {
          return true
        }
        else {
          return false
        }
      }
    },
  },
  methods: {
    getItem() {
      this.$store
        .dispatch('LCF/FindItem', {
          barcode: this.barcode,
          IsLoan: false
        })
        .then(Response => {
          this.item = Response
        })
        .catch(() => {
          this.item = {}
        })
    },
    addAmountToItem() {
        this.$store
          .dispatch("barcodeManagement/addBarcodeToItem", {
            itemId: this.item.itemId,
            barcode: this.barcode,
            isLoanItem: false,
            amount: this.amount
          })
          .then(() => {
            this.barcode = "";
            this.amount = 0;
            this.showFindItemByBarcode = false;
            this.item = {}
            this.$emit("updateItemBarcodes");
          })
          .catch(reject => {
            alert(reject);
          });
    },
    closeBarcodeModal() {
      this.barcode = "";
      this.item = {}
      this.amount = 0;
      this.showFindItemByBarcode = false;
    },
    BarcodeReader(e){
        var y = String.fromCharCode(e.keyCode)
        if (e.keyCode == 13 && this.barcodeIsReading)
        {
            this.barcodeIsReading = false
            this.getItem()
        }
        if (this.barcodeIsReading)
            this.barcode += y
        if(y == 'ß'){
            this.barcodeIsReading = true
            this.Barcode = ''
            if ("activeElement" in document)
                document.activeElement.blur();
            }
    }
  },
  mounted() {
        document.addEventListener("keypress", this.BarcodeReader)
  },
  destroyed(){
      document.removeEventListener("keypress", this.BarcodeReader)
  }
}
</script>