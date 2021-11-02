<template>
  <b-modal
  hide-header-close
    id="addBarcodeToItem"
    v-model="showAddBarcodeToItem"
    no-close-on-backdrop
    size="lg"
    title="Tilføj stregkode"
  >
    <label style="font-weight:bold">Stregkode</label>
    <b-form-input
      id="addBarcode"
      readonly
      required
      v-model="Barcode"
      placeholder="Scan strejkode..."
    ></b-form-input>
    <b-row style="margin-top:15px">
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
        <label>{{Catagory}}</label>
        <br />
        <label style="font-weight:bold; margin-top:15px">Antal</label>
        <br />
        <label>{{item.amountLeft}}</label>
      </b-col>
      <b-col lg="3" v-if="!IsLoan">
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
          @click="addBarcode"
        >Tilføj stregkode</b-button>
        <b-button
          style="margin-left:10px"
          class="float-right"
          variant="danger"
          @click="removeBarcode"
          :disabled="item.barcodes.length == 0"
        >Fjern stregkode</b-button>
        <b-button style="margin-left:10px" class="float-right" @click="closeBarcodeModal">Annuller</b-button>
        <b-button
          style="margin-left:10px"
          class="float-left"
          variant="primary"
          :disabled="Barcode == ''"
          @click="PrintBarcode"
        >Print strejkode</b-button>
      </div>
    </template>
    
  </b-modal>
</template>

<script>
import * as bpac from 'src/assets/Scripts/bpac.js'
export default {
  name: "AddBarcodeToItem",
  data() {
    return {
      showAddBarcodeToItem: this.value,
      selectedImage: process.env.VUE_APP_IMAGEPLACEHOLDER,
      localIsLoanItem: this.isLoanItem,
      barcode: "",
      barcodeIsReading: false,
      amount: 0,
      localitem: this.item
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
      this.showAddBarcodeToItem = val;
    },
    showAddBarcodeToItem() {
      this.$emit("input", this.showAddBarcodeToItem);
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
    Barcode: {
      cache: false,
      get: function() {
        if (Object.keys(this.item).length === 0 || this.barcode != '') {
          return this.barcode
        }
        if (this.item.barcodes.length != 0) {
            return this.item.barcodes[0]
          }
          return this.barcode
      },
      set: function(value) {
        this.barcode = value
      }
    },
    Template: {
      cache: false,
      get: function() {
        if (this.Barcode.length == 13) {
          return 'https://splagersyssa.blob.core.windows.net/templates/Barcode.lbx'
        }
        else
          return 'https://splagersyssa.blob.core.windows.net/templates/code128-barcode.lbx'
      }
    }
  },
  methods: {
    async PrintBarcode()
    {
    if(bpac.IsExtensionInstalled() == false)
        {
          const agent = window.navigator.userAgent.toLowerCase();
          const ischrome = (agent.indexOf('chrome') !== -1) && (agent.indexOf('edge') === -1)  && (agent.indexOf('opr') === -1)
          if(ischrome)
            window.open('https://chrome.google.com/webstore/detail/ilpghlfadkjifilabejhhijpfphfcfhb', '_blank');
          return;
        }

      const objDoc = bpac.IDocument;
      const ret = await objDoc.Open(this.Template);
      
      if (ret != false) {
        
      const barcode = await objDoc.GetObject("barcode");
      var str = this.Barcode
      if (str.length == 13 || str.length == 10 ) {
        barcode.Text = str
      }
      else {
        barcode.Text = '0000000000000'
      }
      const model = await objDoc.GetObject("model")
      model.Text = this.item.model
      const category = await objDoc.GetObject("brand")
      category.Text = this.item.brand
      objDoc.StartPrint("",0);
      objDoc.PrintOut(1,0);
      objDoc.EndPrint();
      objDoc.Close();
      }
    },
    addBarcode() {
        this.$store
          .dispatch("barcodeManagement/addBarcodeToItem", {
            itemId: this.item.itemId,
            barcode: this.Barcode,
            isLoanItem: this.IsLoan,
            amount: this.amount
          })
          .then(() => {
            this.Barcode = "";
            this.amount = 0;
            this.showAddBarcodeToItem = false;
            this.$emit("updateItemBarcodes");
          })
          .catch(reject => {
            alert(reject);
          });
    },
    removeBarcode() {
      this.$store
        .dispatch("barcodeManagement/removeBarcodeFromItem", {
          barcode: this.Barcode,
          isConsumptionItem: !this.IsLoan,
          amount: this.amount
        })
        .then(() => {
          this.Barcode = "";
          this.amount = 0;
          this.showAddBarcodeToItem = false;
          this.$emit("updateItemBarcodes");
        });
      this.selectedItem = Object;
      this.showDeleteSelectedItemModal = false;
    },
    closeBarcodeModal() {
      this.Barcode = "";
      this.amount = 0;
      this.showAddBarcodeToItem = false;
    },
    BarcodeReader(e){
        var y = String.fromCharCode(e.keyCode)
        if (e.keyCode == 13 && this.barcodeIsReading)
        {
            this.barcodeIsReading = false
        }
        if (this.barcodeIsReading)
            this.Barcode += y
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