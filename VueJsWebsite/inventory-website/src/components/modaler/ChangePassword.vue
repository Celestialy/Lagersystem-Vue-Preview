<template>
    <b-modal id="ChangePassword"
       no-close-on-backdrop
       title="Reset password."
       @cancel="clearChangePasswordModal"
       @close="clearChangePasswordModal"

       v-model="showmodal"
       >
         <b-row>
             <b-col lg="12" class="my-1">
                 <b-form-group
                    label="Nuværende kodeord:"
                    label-align-sm="6"
                    label-size="sm"
                    class="mb-0"
                 >
                 <b-input-group size="sm">
              <b-form-input
                name="CurrentPassword"
                v-model="oldPassword"
                type="password"
                required
                placeholder="Nuværende kodeord"
              ></b-form-input>
              <b-form-invalid-feedback :state="validCurrentPassword">Nuværende adgangskode er forkert</b-form-invalid-feedback>
                 </b-input-group>
                 </b-form-group>
            </b-col>
           <b-col lg="12" class="my-1">
                 <b-form-group
                    label="Nyt kodeord:"
                    label-align-sm="6"
                    label-size="sm"
                    class="mb-0"
                 >
                 <b-input-group size="sm">
              <b-form-input
                name="newPassword"
                v-model="newPassword"
                type="password"
                required
                placeholder="Nyt kodeord"
                :state="validPassword"
              ></b-form-input>
              <b-form-invalid-feedback :state="validPassword">Kodeordet skal have et stort bogstav og et tal eller special tegn</b-form-invalid-feedback>
                 </b-input-group>
                 </b-form-group>
            </b-col>
            <b-col lg="12" class="my-1">
                 <b-form-group
                    label="Bekræft nyt kodeord:"
                    label-align-sm="6"
                    label-size="sm"
                    class="mb-0"
                 >
                 <b-input-group size="sm">
              <b-form-input
                name="ConfirmNewPassword"
                v-model="ConfirmNewPassword"
                type="password"
                required
                placeholder="Bekræft nyt kodeord"
                :state="PasswordsMatch"
              ></b-form-input>
              <b-form-invalid-feedback :state="PasswordsMatch">Kodeordet matcher ikke</b-form-invalid-feedback>
                 </b-input-group>
                 </b-form-group>
            </b-col>
         </b-row>
       <template  v-slot:modal-footer="{cancel}">
           <label>Hvis du har glemt dit kodeord bedes du spørge din instruktør om at nulstille det.</label>
           <b-button
              class="float-right"
              style="margin-left: 5px"
              variant="success"
              @click="SubmitChangePassword()"
            >OK</b-button>
            <b-button class="float-right" variant="danger" @click="cancel()">Cancel</b-button>
       </template>
      </b-modal>
</template>

<script>
export default {
    name: 'ChangePasswordModal',
    data () {
        return {
            oldPassword: '',
            newPassword: '',
            ConfirmNewPassword: '',
            validCurrentPassword: true,
            showmodal: false
        }
    },
    computed: {
        validPassword(){
            var re = new RegExp("^((?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])|(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[^a-zA-Z0-9])|(?=.*?[A-Z])(?=.*?[0-9])(?=.*?[^a-zA-Z0-9])|(?=.*?[a-z])(?=.*?[0-9])(?=.*?[^a-zA-Z0-9])).{8,}$")
            return re.test(this.newPassword)
        },
        PasswordsMatch(){
            return this.newPassword == this.ConfirmNewPassword
        }
    },
    methods: {
        SubmitChangePassword() {
            this.$store.dispatch('auth/changePassword', {
                OldPassword: this.oldPassword,
                NewPassword: this.newPassword,
                ConfirmNewPassword: this.ConfirmNewPassword
            }).then(() => {
                this.clearChangePasswordModal()
                this.showmodal = false
            }).catch(error => {
                if(error.response.data == 'nuværende kode er forkert')
                {
                    this.validCurrentPassword = false;
                }

            })
        },
        clearChangePasswordModal(){
            this.oldPassword = ''
            this.newPassword = ''
            this.ConfirmNewPassword = ''
            this.validCurrentPassword = true
        },
    }
}
</script>