<template>
     <b-modal
        cancel-title="Annuller"
        @ok="submitEdittedUser"
        id="editUserModal"
        no-close-on-backdrop
        size="xl"
        title="Rediger lager"
        v-model="showModal"
      >
        <b-form>
          <b-row>
            <b-col lg="4" style="margin-bottom:15px">
              <label>Brugernavn:</label>
              <b-form-input
                name="Username"
                v-model="SelectedUser.username"
                type="text"
                required
                placeholder="Brugernavn"
              ></b-form-input>
            </b-col>
            <b-col lg="4">
              <label>Fornavn:</label>
              <b-form-input
                v-model="SelectedUser.firstName"
                type="text"
                required
                placeholder="Fornavn"
              ></b-form-input>
            </b-col>
            <b-col lg="4">
              <label>Efternavn:</label>
              <b-form-input
                v-model="SelectedUser.lastName"
                type="text"
                required
                placeholder="Efternavn"
                
              ></b-form-input>
            </b-col>
            <b-col lg="4">
              <label>Email:</label>
              <b-form-input v-model="SelectedUser.email" type="email" required placeholder="Email"></b-form-input>
            </b-col>
            <b-col lg="4">
              <label>LagerManager:</label>
              <b-form-checkbox
                v-model="SelectedUser.roles"
                value="InventoryManager"
                unchecked-value
              >- LagerManager</b-form-checkbox>
            </b-col>
          </b-row>
        </b-form>
        <template v-slot:modal-footer="{ok,cancel}">
          <div class="w-100">
            <b-button v-b-modal.ResetPasswordModal class="float-left" variant="primary">Reset Password</b-button>
            <b-button
              class="float-right"
              style="margin-left: 5px"
              variant="success"
              @click="ok()"
            >OK</b-button>
            <b-button class="float-right" variant="danger" @click="cancel()">Cancel</b-button>
          </div>
        </template>
      </b-modal>
</template>

<script>
export default {
    name: 'EditUser',
    data() {
        return {
            
        }
    },
    props: {
        SelectedUser: Object,
        showModal: {
            type: Boolean,
            default: false
        }
    },
    methods: {
        submitEdittedUser() {
            this.$store.dispatch('UserManagement/editUser', this.SelectedUser).then(() => {
                this.$store.dispatch('UserManagement/updateRoles', this.SelectedUser)
                .then(() => {
                    this.loadData()
                // eslint-disable-next-line no-unused-vars
                }).catch(error => {

                })
            })
        },
    }
}
</script>