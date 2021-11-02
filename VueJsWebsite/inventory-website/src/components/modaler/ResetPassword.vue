<template>
      <b-modal id="ResetPasswordModal"
       no-close-on-backdrop
       title="Reset password."
       @ok="SubmitResetPassword"
       @cancel="clearResetPasswordModal"
       @close="clearResetPasswordModal"
       >
         <b-row>
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
      </b-modal>
</template>
<script>
export default {
    name: 'resetPassword',
    data() {
        return {
            
            newPassword: '',
            ConfirmNewPassword: ''
        }
    },
    props: {
        user: Object
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
        SubmitResetPassword() {
        this.$store.dispatch('UserManagement/resetPassword', {
          UserId: this.user.userId,
          NewPassword: this.newPassword,
          ConfirmNewPassword: this.ConfirmNewPassword
        }).then(() => {
          this.clearResetPasswordModal()
        })
    },
    clearResetPasswordModal(){
      this.newPassword = ''
      this.ConfirmNewPassword = ''
    },
    }
}
</script>