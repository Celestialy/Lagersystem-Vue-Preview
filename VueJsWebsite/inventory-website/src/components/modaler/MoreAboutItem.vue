<template>
    <b-modal
      id="MoreAboutItem"
      v-model="localShowModal"
      ok-only
      no-close-on-backdrop
      size="lg"
      :title="title"
    >
      <b-form>
        <b-row>
          <b-col lg="6" style="margin-bottom:15px">
            <label style="font-weight:bold">Mærke</label>
            <br />
            <label>{{SelectedItem.brand}}</label>
          </b-col>
          <b-col lg="6">
            <label style="font-weight:bold">Model</label>
            <br />
            <label>{{SelectedItem.model}}</label>
          </b-col>
          <b-col lg="6" style="margin-bottom:15px">
            <label style="font-weight:bold">Beskrivelse</label>
            <br />
            <label>{{SelectedItem.description}}</label>
          </b-col>
          <b-col lg="6">
            <label style="font-weight:bold">Kategori</label>
            <br />
            <label>{{SelectedItem.category.categoryName}}</label>
          </b-col>
          <b-col lg="2">
            <b-img style="height:105px" fluid :src="SelectedItem.image.imageUri"></b-img>
          </b-col>
          <b-col lg="6" v-if="isConsumtion">
            <label style="font-weight:bold">Mængde</label>
            <br />
            <label>{{SelectedItem.amountLeft}}</label>
          </b-col>
        </b-row>
      </b-form>
    </b-modal>
</template>

<script>
export default {
    name: 'MoreAboutItem',
    data() {
        return{
            localShowModal: this.ShowModal
        }
    },
    props:{
        SelectedItem: Object,
        ShowModal: Boolean
    },
    model: {
        prop: 'ShowModal',
        event: 'change'
    },
    watch: {
        ShowModal(val){
            this.localShowModal = val
        },
        localShowModal(){
            this.$emit('change', this.localShowModal)
        }
    },
    computed: {
        isConsumtion: {
          cache: false, 
          get() {
              return this.SelectedItem.inventory.inventoryId == 2
          }
      },
       title: {
          cache: false, 
          get() {
              return this.SelectedItem.brand + ' - ' + this.SelectedItem.model
          }
      }
    }
}
</script>