<template >
  <b-container style="margin-top: 2%">
    <b-card bg-variant="light">
      <template v-slot:header>
        <b-nav card-header tabs>
          <b-nav-item :active="IsLoanForm" @click="CurrentFormMode = FormModes.Loan">Udlån</b-nav-item>
          <b-nav-item :active="IsReturnForm" @click="CurrentFormMode = FormModes.Return">Aflever</b-nav-item>
          <b-nav-item
            :active="IsConsumtionForm"
            @click="CurrentFormMode = FormModes.Consumtion"
          >Forbrug</b-nav-item>
        </b-nav>
      </template>
      <b-form-group :label="FormTitleText" label-class="font-weight-bold pt-0" class="mb-0">
        <b-row>
          <b-col lg="6">
            <b-form-group label="Vælg bruger:" label-align="center"></b-form-group>
            <div class="text-center">
              <b-button
                id="SelectUserButton"
                :disabled="userList.length == 0"
                pill
                v-b-modal.SelectUserModal
                variant="primary"
                size="lg"
              >{{GetUserButtonText}}</b-button>
            </div>
            <div class="text-center" style="margin-top: 1%" v-if="userLoaded">
              <b-col>
                <label>Brugernavn: {{user.username}}</label>
              </b-col>
              <b-col>
                <label>Navn: {{user.firstName}} {{user.lastName}}</label>
              </b-col>
            </div>
          </b-col>
          <b-col lg="6">
            <b-form-group label="Vælg ting:" label-align="center"></b-form-group>
            <b-form-input
              id="loanform"
              readonly
              required
              v-model="barcode"
              placeholder="Scan strejkode..."
            ></b-form-input>
            <div style="margin-top: 1%" v-if="ItemIsLoaded">
              <b-img left style="height: 120px" fluid :src="imageUri"></b-img>
              <div>
                <b-col>
                  <label>Mærke: {{item.brand}}</label>
                </b-col>
                <b-col>
                  <label>Model: {{item.model}}</label>
                </b-col>
                <b-col>
                  <label>Kategory: {{item.category}}</label>
                </b-col>
                <b-col>
                  <label :class="{'text-danger': item.amountLeft == 0}">Mængde: {{item.amountLeft}}</label>
                </b-col>
              </div>
            </div>
          </b-col>
          <b-col
            v-if="IsConsumtionForm && ItemIsLoaded"
            lg="12"
            style="margin-top: 1%; margin-bottom: 1%;"
          >
            <b-form-input
              class="float-right"
              style="width: 10%"
              type="number"
              placeholder="Antal.."
              v-model="Amount"
            ></b-form-input>
            <div class="float-right" style="margin: auto; width: 50%; padding: 10px;">
              <label class="float-right">Antal</label>
            </div>
          </b-col>
        </b-row>
        <b-button
          v-if="!IsReturnForm && !IsConsumtionForm"
          variant="success"
          :disabled="!userLoaded || !ItemIsLoaded || item.amountLeft == 0"
          @click="loanItem"
        >Udlån</b-button>
        <b-button
          v-else-if="IsReturnForm"
          variant="danger"
          :disabled="!userLoaded || !ItemIsLoaded"
          @click="ReturnItem"
        >Aflever</b-button>
        <b-button
          v-else-if="IsConsumtionForm"
          variant="success"
          :disabled="!userLoaded || !ItemIsLoaded || Amount == 0"
          @click="ConsumeItem"
        >udlevere</b-button>
      </b-form-group>
    </b-card>
    <SelectUserModal
      :UserList="userList"
      :isFetching="isFetching"
      v-model="user"
      @load="loadData"
      @cansel="cancelFetch"
    />
  </b-container>
</template>

<script>
const FormModes = {
  Loan: 0,
  Return: 1,
  Consumtion: 2
}

export default {
  name: 'Loan',
  data() {
    return {
      timer: null,
      user: {},
      userList: [],
      item: {},
      barcode: '',
      barcodeIsReading: false,
      isFetching: false,
      failedFetches: 0,
      Amount: null,
      FormModes,
      CurrentFormMode: FormModes.Loan
    }
  },
  computed: {
    userLoaded() {
      return !(
        Object.entries(this.user).length === 0 &&
        this.user.constructor === Object
      )
    },
    ItemIsLoaded() {
      return !(
        Object.entries(this.item).length === 0 &&
        this.item.constructor === Object
      )
    },
    GetUserButtonText() {
      if (this.userLoaded) {
        return 'Vælg anden bruger'
      } else {
        return 'Vælg bruger'
      }
    },
    imageUri() {
      if (this.ItemIsLoaded) {
        return this.item.image.imageUri
      } else {
        return process.env.VUE_APP_IMAGEPLACEHOLDER
      }
    },
    FormTitleText() {
      if (this.IsReturnForm) {
        return 'Afleverings formular'
      } else if (this.IsConsumtionForm) {
        return 'Forbrugs formular'
      } else {
        return 'Udlånings formular'
      }
    },
    IsLoanForm() {
      if (this.CurrentFormMode == this.FormModes.Loan) {
        return true
      } else {
        return false
      }
    },
    IsReturnForm() {
      if (this.CurrentFormMode == this.FormModes.Return) {
        return true
      } else {
        return false
      }
    },
    IsConsumtionForm() {
      if (this.CurrentFormMode == this.FormModes.Consumtion) {
        return true
      } else {
        return false
      }
    }
  },
  watch: {
    IsReturnForm() {
      if (!this.IsReturnForm) {
        this.loadData()
      } else if (this.ItemIsLoaded) {
        this.getReturnUsers()
      } else {
        this.userList = []
      }
      this.user = {}
    },
    IsConsumtionForm() {
      if (this.barcode != '') {
        this.getItem()
      }
    }
  },
  methods: {
    getItem() {
      this.$store
        .dispatch('LCF/FindItem', {
          barcode: this.barcode,
          IsLoan: !this.IsConsumtionForm
        })
        .then(Response => {
          this.item = Response
          if (this.IsReturnForm) {
            this.getReturnUsers()
          }
        })
        .catch(() => {
          this.item = {}
        })
    },
    ReturnItem() {
      this.$store
        .dispatch('LCF/ReturnItem', {
          userId: this.user.userId,
          itemBarcode: this.barcode
        })
        .then(() => {
          this.clearForm()
        })
    },
    getReturnUsers() {
      if (!this.IsReturnForm) {
        this.cancelFetch()
        return
      }
      this.userList = []
      this.isFetching = true
      this.$store
        .dispatch('LCF/getReturnLoanUsers', this.barcode)
        .then(users => {
          // eslint-disable-next-line no-console
          console.log(
            'Got some data, now lets show something in this component'
          )
          this.userList = users
          this.isFetching = false
          this.failedFetches = 0
        })
        .catch(() => {
          // eslint-disable-next-line no-console
          console.log('no users')

          this.failedFetches++
          if (this.failedFetches <= 5) {
            this.timer = setTimeout(() => {
              this.getReturnUsers()
            }, 5000)
          } else {
            this.isFetching = false
            this.failedFetches = 0
          }
        })
    },
    loanItem() {
      this.$store
        .dispatch('LCF/LoanItem', {
          userId: this.user.userId,
          ItemBarcode: this.barcode
        })
        .then(() => {
          this.clearForm()
        })
        .catch(() => {
          alert('error')
        })
    },
    ConsumeItem() {
      this.$store
        .dispatch('LCF/ConsumeItem', {
          UserId: this.user.userId,
          ItemBarcode: this.barcode,
          ItemAmount: parseInt(this.Amount)
        })
        .then(() => {
          this.clearForm()
        })
        .catch(() => {
          alert('error')
        })
    },
    loadData() {
      if (this.IsReturnForm) {
        this.cancelFetch()
        return
      }
      this.isFetching = true
      this.userList = []
      this.$store
        .dispatch('UserManagement/GetBasicUsersFromDepartment')
        .then(users => {
          // eslint-disable-next-line no-console
          console.log(
            'Got some data, now lets show something in this component'
          )
          this.userList = users
          this.isFetching = false
          this.failedFetches = 0
        })
        .catch(() => {
          // eslint-disable-next-line no-console
          console.log('no users')

          this.failedFetches++
          if (this.failedFetches <= 5) {
            this.timer = setTimeout(() => {
              this.loadData()
            }, 5000)
          } else {
            this.isFetching = false
            this.failedFetches = 0
          }
        })
    },
    cancelFetch() {
      clearTimeout(this.timer)
      this.failedFetches = 0
      this.isFetching = false
    },
    clearForm() {
      this.barcode = ''
      this.item = {}
      this.user = {}
      if (this.IsReturnForm) {
        this.userList = []
      }
    },
    BarcodeReader(e) {
      var y = String.fromCharCode(e.keyCode)
      if (e.keyCode == 13 && this.barcodeIsReading) {
        this.barcodeIsReading = false
        this.getItem()
      }
      if (this.barcodeIsReading) this.barcode += y
      if (y == 'ß') {
        this.barcodeIsReading = true
        this.barcode = ''
        if ('activeElement' in document) document.activeElement.blur()
      }
    }
  },
  created() {
    this.loadData()
  },
  mounted() {
    document.addEventListener('keypress', this.BarcodeReader)
  },
  destroyed() {
    document.removeEventListener('keypress', this.BarcodeReader)
    clearTimeout(this.timer)
  }
}
</script>