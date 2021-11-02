<template>
  <b-container fluid>
    <b-row>
      <b-col lg="6" class="my-1">
        <b-button-group class="mt-1">
          <b-button :disabled="isFetching" v-on:click="loadData">Opdater tabel</b-button>
          <b-button v-show="isFetching" v-on:click="cancel">Cancel</b-button>
        </b-button-group>
      </b-col>
      <b-col lg="6" class="my-1">
        <b-input-group size="sm">
          <b-form-input
            v-model="search"
            type="search"
            id="filterInput"
            placeholder="Skriv for at søge..."
          ></b-form-input>
        </b-input-group>
      </b-col>
    </b-row>

    <div class="tablecontainer">
      <b-table
        id="departmentTable"
        outlined
        :busy="isFetching"
        :items="Users"
        :fields="fields"
        :filter="search"
        show-empty
        :per-page="perPage"
        :current-page="currentPage"
        primary-key="username"
        @filtered="onFilter"
        :sort-by.sync="sortBy"
      >
        <template v-slot:head(button)>
          <b-button @click="sortBy = ''; search = ''">Nulstil filter</b-button>
        </template>

        <template v-slot:table-busy>
          <div class="text-center text-danger my-2">
            <b-spinner class="align-middle"></b-spinner>
            <strong>Loading...</strong>
          </div>
        </template>
        <template v-slot:empty="scope">
          <h4>Vi kunne ikke finde nogen brugere der ikke er i en afdeling...</h4>
          <b-button v-on:click="loadData">Refresh table</b-button>
        </template>
        <template v-slot:cell(button)="data">
          <b-row>
            <b-col lg="6">
              <b-button variant="warning" @click="editUser(data.item)">Rediger</b-button>
            </b-col>
            <b-col lg="6">
              <b-button
                variant="danger"
                :disabled="data.item.roles.includes('Manager') || data.item.roles.includes('Admin')"
                :title="data.item.roles.includes('Manager') || data.item.roles.includes('Admin')? 'Du har ikke rettighedder til at fjerne denne bruger': ''"
                @click="DeleteUser(data.item)"
              >Fjern</b-button>
            </b-col>
          </b-row>
        </template>
      </b-table>
    </div>
    <b-pagination
      v-if="totalRows > 10"
      align="center"
      v-model="currentPage"
      :total-rows="totalRows"
      :per-page="perPage"
      aria-controls="departmentTable"
    />
    <b-modal
      cancel-title="Annuller"
      @ok="submitEdittedUser"
      id="editUserModal"
      no-close-on-backdrop
      size="xl"
      title="Rediger lager"
      v-model="showEditModal"
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
          <b-button class="float-right" style="margin-left: 5px" variant="success" @click="ok()">OK</b-button>
          <b-button class="float-right" variant="danger" @click="cancel()">Cancel</b-button>
        </div>
      </template>
    </b-modal>
    <ResetPassword :user="SelectedUser" />
  </b-container>
</template>

<script>
import smoothReflow from "vue-smooth-reflow";

export default {
  name: "DepartmentManager",
  mixins: [smoothReflow],
  data() {
    return {
      timer: null,
      fields: [
        { key: "username", label: "Brugernavn", sortable: true },
        { key: "firstName", label: "Fornavn", sortable: true },
        { key: "lastName", label: "Efternavn", sortable: true },
        "email",
        { key: "button", label: "" }
      ],
      isFetching: true,
      failedFetches: 0,
      search: "",
      currentPage: 1,
      totalRows: 0,
      perPage: 10,
      sortBy: "",
      SelectedUser: {},
      showEditModal: false
    };
  },
  computed: {
    Users() {
      return this.$store.getters["UserManagement/getUsersInDepartment"];
    },
    UserAmount() {
      return this.$store.getters["UserManagement/getUsersInDepartment"].length;
    }
  },
  methods: {
    submitEdittedUser() {
      this.$store
        .dispatch("UserManagement/editUser", this.SelectedUser)
        .then(() => {
          this.$store
            .dispatch("UserManagement/updateRoles", this.SelectedUser)
            .then(() => {
              this.loadData();
            });
        });
    },
    DeleteUser(user) {
      var result = confirm(
        "er du sikker på du vil fjerne: " +
          user.firstName +
          " " +
          user.lastName +
          " fra din afdeling"
      );
      if (result) {
        this.$store
          .dispatch("UserManagement/RemoveUserFromDepartment", user.userId)
          .then(() => {
            this.totalRows--;
          });
      }
    },
    editUser(user) {
      Object.assign(this.SelectedUser, user);
      this.showEditModal = true;
    },
    addUserToDepartment(userid) {
      this.$store
        .dispatch("UserManagement/addUserToDepartment", userid)
        .then(() => {
          this.totalRows--;
        });
    },
    loadData() {
      this.isFetching = true;
      this.$store
        .dispatch("UserManagement/GetUsersFromDepartment")
        .then(() => {
          // eslint-disable-next-line no-console
          console.log(
            "Got some data, now lets show something in this component"
          );
          this.totalRows = this.UserAmount;
          this.isFetching = false;
          this.failedFetches = 0;
        })
        .catch(() => {
          // eslint-disable-next-line no-console
          console.log("no users");
          this.totalRows = 0;
          this.failedFetches++;
          if (this.failedFetches <= 5) {
            this.timer = setTimeout(() => {
              this.loadData();
            }, 5000);
          } else {
            this.isFetching = false;
            this.failedFetches = 0;
          }
        });
    },
    cancel() {
      clearTimeout(this.timer);
      this.failedFetches = 0;
      this.isFetching = false;
    },
    onFilter(filteredItems) {
      this.totalRows = filteredItems.length;
      this.currentPage = 1;
    }
  },
  destroyed() {
    clearTimeout(this.timer);
  },
  created() {
    this.loadData();
  },
  mounted() {
    this.$smoothReflow({
      el: "#departmentTable"
    });
    this.$smoothReflow({
      el: ".tablecontainer"
    });
  }
};
</script>