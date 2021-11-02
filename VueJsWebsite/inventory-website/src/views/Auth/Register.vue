<template>
  <b-container style="margin-top: 2%; width:40%;">
    <b-row>
      <b-col lg="6">
        <b-form-group label="Fornavn:">
          <b-form-input required v-model="newUser.firstname" type="text" placeholder="Fornavn"></b-form-input>
        </b-form-group>
      </b-col>
      <b-col lg="6">
        <b-form-group label="Efternavn:">
          <b-form-input required v-model="newUser.lastname" type="text" placeholder="Efternavn"></b-form-input>
        </b-form-group>
      </b-col>
      <b-col lg="6">
        <b-form-group label="Brugernavn:">
          <b-form-input
            required
            v-model="Username"
            type="text"
            placeholder="Brugernavn"
            @keydown.space.prevent
          ></b-form-input>
        </b-form-group>
      </b-col>
      <b-col lg="6">
        <b-form-group label="Email:">
          <b-form-input required v-model="newUser.email" type="email" placeholder="Email"></b-form-input>
        </b-form-group>
      </b-col>
      <b-col lg="6">
        <b-form-group label="Kodeord:">
          <b-form-input
            required
            v-model="newUser.password"
            type="password"
            placeholder="Kodeord"
            :state="validPassword"
          ></b-form-input>
          <b-form-invalid-feedback
            :state="validPassword"
          >Kodeordet skal have et stort bogstav og et tal eller special tegn</b-form-invalid-feedback>
        </b-form-group>
      </b-col>
      <b-col lg="6">
        <b-form-group label="Bekræft kodeord:">
          <b-form-input
            required
            v-model="newUser.PasswordConfirmation"
            type="password"
            placeholder="Bekræft kodeord"
            :state="PasswordsMatch"
          ></b-form-input>
          <b-form-invalid-feedback :state="PasswordsMatch">Kodeordet matcher ikke</b-form-invalid-feedback>
        </b-form-group>
      </b-col>
    </b-row>
    <b-button :disabled="IsFetching" @click="register()" variant="success">Register</b-button>
  </b-container>
</template>

<script>
export default {
  name: "Register",
  data() {
    return {
      newUser: {
        firstname: "",
        lastname: "",
        username: "",
        email: "",
        password: "",
        PasswordConfirmation: ""
      },
      IsFetching: false
    };
  },
  computed: {
    validPassword() {
      var re = new RegExp(
        "^((?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])|(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[^a-zA-Z0-9])|(?=.*?[A-Z])(?=.*?[0-9])(?=.*?[^a-zA-Z0-9])|(?=.*?[a-z])(?=.*?[0-9])(?=.*?[^a-zA-Z0-9])).{8,}$"
      );
      return re.test(this.newUser.password);
    },
    PasswordsMatch() {
      return this.newUser.password == this.newUser.PasswordConfirmation;
    },
    Username: {
      get: function() {
        return this.newUser.username;
      },
      set: function(value) {
        this.newUser.username  = value.replace(" ", "");
      }
    }
  },
  methods: {
    register() {
      this.IsFetching = true;
      this.$store
        .dispatch("auth/Register", this.newUser)
        .then(() => {
          this.IsFetching = false;
          this.$router.push("/");
        })
        .catch(() => {
          this.IsFetching = false;
        });
    }
  }
};
</script>