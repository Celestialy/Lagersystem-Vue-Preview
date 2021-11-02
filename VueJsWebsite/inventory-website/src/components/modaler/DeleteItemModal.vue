<template>
    <b-modal
      id="deleteSelectedItemModal"
      size="lg"
      v-model="ShowModal"
      no-close-on-backdrop
      :title="selectedItemModalTitle"
    >
      <b-row>
        <b-col lg="12">
          <p
            style="font-size:20px"
          >Er du sikker på at du vil fjerne {{SelectedItem.brand}} {{SelectedItem.model}} fra lageret?</p>
        </b-col>
      </b-row>
      <template v-slot:modal-footer>
        <b-button class="float-right" @click="$emit('Change', false)">Annuller</b-button>
        <b-button
          style="margin-left:10px"
          class="float-right"
          variant="danger"
          @click="deleteItem"
        >Ja</b-button>
      </template>
    </b-modal>
</template>

<script>
export default {
    name: 'DeleteItem',
    data() {
        return {
            
        }
    },
    props: {
        SelectedItem: Object,
        ShowModal: Boolean
    },
    model: {
         prop: 'ShowModal',
         event: 'Change'
    },
    computed: {
      selectedItemModalTitle(){
        return this.SelectedItem.brand + " - " + this.SelectedItem.model
      },
    },
    methods: {
        deleteItem() {
           if (this.SelectedItem.inventory.inventoryId == 2) {
             this.$store
               .dispatch("inventoryManagement/deleteConsumptionItem", {
                 itemId: this.SelectedItem.itemId,
                 inventoryId: this.SelectedItem.inventory.inventoryId
               })
               .then(() => {
                 this.totalRows--;
                 this.$emit('deleted');
               });
             this.$emit('Change', false)
           } else {
             this.$store
               .dispatch("inventoryManagement/deleteLoanItem", {
                 itemId: this.SelectedItem.itemId,
                 inventoryId: this.SelectedItem.inventory.inventoryId
               })
               .then(() => {
                 this.totalRows--;
                 this.$emit('deleted');
               });
             this.$emit('Change', false)
           }
    },
    }
}
</script>