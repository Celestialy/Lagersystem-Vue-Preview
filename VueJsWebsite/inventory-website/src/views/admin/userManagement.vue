<template>
  <b-container fluid>
    <b-row>
      <b-col lg="2" class="my-1">
        <b-button-group class="mt-1">
          <b-button :disabled="isFetching" v-on:click="loadData">Opdater tabel</b-button>
          <b-button v-show="isFetching" v-on:click="cancel">Cancel</b-button>
        </b-button-group>
      </b-col>
      <b-col lg="4" style="padding-top:15px">
        <b-form-checkbox
          v-model="checked"
          name="check-button"
          switch
        >Vis brugere der er i en afdeling</b-form-checkbox>
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
            <b-col lg="4">
              <b-button variant="warning" @click="editUser(data.item)">Rediger</b-button>
            </b-col>
            <b-col lg="6">
              <b-button v-if="!checked" variant="danger" @click="deleteUser(data.item)">Slet</b-button>
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
      @cancel="cancel"
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
          <b-col lg="4" v-if="!checked">
            <label>Afdeling</label>
            <b-form-select
              v-model="departmentDropdown"
              :options="Departments"
              value-field="departmentId"
              text-field="departmentName"
            ></b-form-select>
          </b-col>
          <b-col lg="4" v-if="checked">
            <label>Manager:</label>
            <b-form-checkbox
              v-model="SelectedUser.roles"
              value="Manager"
              unchecked-value
            >- Manager</b-form-checkbox>
          </b-col>
        </b-row>
      </b-form>
      <template v-slot:modal-footer="{ok,cancel}">
        <div class="w-100">
          <b-button v-b-modal.ResetPasswordModal class="float-left" variant="primary">Reset Password</b-button>
          <b-button class="float-right" style="margin-left: 5px" variant="success" @click="ok()">OK</b-button>
          <b-button class="float-right" variant="danger" @click="cancel()">Annuller</b-button>
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
      showEditModal: false,
      departmentDropdown: null,
      checked: false
    };
  },
  computed: {
    Users() {
      if (!this.checked) {
        return this.$store.getters["UserManagement/getUsers"];
      } else {
        return this.$store.getters["UserManagement/getUsersInDepartment"];
      }
    },
    UserAmount() {
      if (!this.checked) {
        return this.$store.getters["UserManagement/getUsers"].length;
      } else {
        return this.$store.getters["UserManagement/getUsersInDepartment"]
          .length;
      }
    },

    Departments() {
      var departments = this.$store.getters[
        "departmentManagement/getDepartments"
      ];
      departments.unshift({
        departmentId: null,
        departmentName: "Vælg afdeling"
      });

      return departments;
    }
  },
  methods: {
    submitEdittedUser() {
      this.$store
        .dispatch("UserManagement/editUser", this.SelectedUser)
        .then(() => {
          if (this.checked) {
            this.$store.dispatch("UserManagement/updateRoles", this.SelectedUser).then(() => {
              this.getUsersInDepartment();
            });
          } else {            
            if (this.departmentDropdown > 0) {
              this.$store.dispatch("UserManagement/addUserToDepartment", {
                userId: this.SelectedUser.userId,
              departmentId: this.departmentDropdown
            }).then(() => {
              this.getUsersInDepartment();
            });
          }
          this.loadData();
          }
        });
    },
    deleteUser(user) {
      var result = confirm(
        "er du sikker på du vil fjerne: " +
          user.firstName +
          " " +
          user.lastName +
          " fra din afdeling"
      );
      if (result) {
        this.$store
          .dispatch("UserManagement/DeleteUser", {
            params: {
              UserId: user.userId
            }
          })
          .then(() => {
            this.totalRows--;
          });
      }
    },
    editUser(user) {
      Object.assign(this.SelectedUser, user);
      this.showEditModal = true;

      if (this.checked) {
        this.departmentDropdown = 1;
      } else {
        this.departmentDropdown = null;
      }
    },
    addUserToDepartment(userid) {
      this.$store
        .dispatch(
          "UserManagement/addUserToDepartment",
          userid,
          // eslint-disable-next-line no-undef
          departmentDropdown
        )
        .then(() => {
          this.totalRows--;
        });
    },
    getDepartments() {
      this.$store
        .dispatch("departmentManagement/GetDepartments")
        .then(() => {
          // eslint-disable-next-line no-console
          console.log(
            "Got some data, now lets show something in this component"
          );
          this.failedFetches = 0;
        })
        .catch(() => {
          // eslint-disable-next-line no-console
          console.log("no departments found");
          this.totalRows = 0;
          this.failedFetches++;
          if (this.failedFetches <= 5) {
            this.timer = setTimeout(() => {
              this.getDepartments();
            }, 5000);
          } else {
            this.failedFetches = 0;
            this.notFoundMessage = "Der blev ikke fundet nogen afdelinger...";
          }
        });
    },
    loadData() {
      this.isFetching = true;
      this.$store
        .dispatch("UserManagement/GetUsersWithoutDepartment")
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
    getUsersInDepartment() {
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
              this.getUsersInDepartment();
            }, 5000);
          } else {
            this.isFetching = false;
            this.failedFetches = 0;
          }
        });
    },
    cancel() {
      clearTimeout(this.timer);
      // eslint-disable-next-line no-console
      console.log("Test");
      this.departmentDropdown = null;
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
    this.getUsersInDepartment();
    this.getDepartments();
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