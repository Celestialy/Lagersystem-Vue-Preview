<template>
    <b-modal
      id="updateCategory"
      v-model="LocalShowModal"
      cancel-title="Annuller"
      no-close-on-backdrop
      size="lg"
      :title="ModalTitle"
      hide-header-close
    >
      <b-form v-if="Modes.delete != Mode">
        <b-row>
          <b-col lg="12" style="margin-bottom:15px">
            <label>Kategorinavn</label>
            <b-form-input
              v-if="Modes.add == Mode"
              v-model="categoryName"
              type="text"
              required
              placeholder="Kategorinavn..."
            ></b-form-input>
            <b-form-input
              v-if="Modes.edit == Mode"
              v-model="LocalCategory.categoryName"
              type="text"
              required
              placeholder="Kategorinavn..."
            ></b-form-input>
          </b-col>
        </b-row>
      </b-form>

      <b-row v-if="Modes.delete == Mode">
        <b-col lg="12">
          <p
            style="font-size:20px"
          >Er du sikker på at du vil slette {{LocalCategory.categoryName}} fra listen over kategorier?</p>
        </b-col>
      </b-row>

      <template v-slot:modal-footer>
        <b-button class="float-right" @click="closeModal">Annuller</b-button>
        <b-button
          v-if="Modes.add == Mode"
          style="margin-left:10px"
          class="float-right"
          variant="success"
          @click="addCategory"
        >Gem</b-button>

        <b-button
          v-if="Modes.edit == Mode"
          style="margin-left:10px"
          class="float-right"
          variant="success"
          @click="editCategory"
        >Gem</b-button>

        <b-button
          v-if="Modes.delete == Mode"
          style="margin-left:10px"
          class="float-right"
          variant="danger"
          @click="deleteCategory"
        >Ja</b-button>
      </template>
    </b-modal>
</template>

<script>
import {Modes} from 'src/Enums/CategoryModalModes.js'
export default {
    name: 'CategoryEditDelete',
    data(){
        return{
            Modes,
            LocalShowModal: this.ShowModal,
            LocalCategory: {},
            categoryName: ''
        }
    },
    props:{
        Mode: Number,
        SelectedCategory: Object,
        ShowModal: Boolean
    },
    model: {
        prop: 'ShowModal',
        event: 'change'
    },
    watch: {
        LocalShowModal(){
            this.$emit('change', this.LocalShowModal)
        },
        ShowModal(val){
            this.LocalShowModal = val
        },
        SelectedCategory(val){
          Object.assign(this.LocalCategory, val)
        }
    },
    computed: {
        ModalTitle(){
            let title = ''
            switch(this.Mode){
                case this.Modes.add:
                    title = 'Tilføj kategori'
                    break
                case this.Modes.edit:
                    title = 'Rediger kategori'
                    break
                case this.Modes.delete:
                    title = 'Slet kategori'
                    break
            }
            return title
        }
    },
    methods: {
        addCategory() {
      this.$store
        .dispatch("categoryManagement/addCategory", {
          categoryName: this.categoryName
        })
        .then(() => {
          this.categoryName = "";
          this.$emit('update');
          this.LocalShowModal = false;
        })
        .catch(reject => {
          alert(reject);
        });
    },
    editCategory() {
      this.$store
        .dispatch("categoryManagement/editCategory", {
          categoryId: this.LocalCategory.categoryId,
          categoryName: this.LocalCategory.categoryName
        })
        .then(() => {
          this.$emit('update');
          this.LocalShowModal = false;
        })
        .catch(reject => {
          alert(reject);
        });
    },
    deleteCategory() {
      this.$store
        .dispatch("categoryManagement/deleteCategory", {
          categoryId: this.LocalCategory.categoryId
        })
        .then(() => {
          this.$emit('deleted')
          this.closeModal()
        }).catch(reject => {
          alert(reject);
          this.closeModal()
        });
    },
    closeModal() {
      this.categoryName = "";
      this.$emit('update:SelectedCategory', {})
      this.LocalShowModal = false;
    },
    }
}
</script>