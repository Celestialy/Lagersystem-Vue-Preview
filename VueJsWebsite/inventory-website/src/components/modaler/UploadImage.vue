<template>
    <b-modal id="UploadImageModal" size="xl" title="Upload billede" v-model="showUploadImageModal" @cancel="closeModal" hide-header-close>
      <b-form-group>
        <b-row>
          <b-col lg="12" style="margin-bottom:30px">
            <label>Navn</label>
            <b-form-input :state="NoImageName" v-model="uploadImageName" type="text" required placeholder="Navn"></b-form-input>
            <b-form-invalid-feedback :state="NoImageName">Husk at give billedet et navn</b-form-invalid-feedback>
          </b-col>
          <b-col lg="12">
            <div class="file-upload" :class="imageUploadBorder">
              <div v-if="!isUploaded" class="file-upload-text" ref="uploadedImageText">
                <b-img
                  class="fileUploadImg"
                  :src="standardImageURI"
                ></b-img>Vælg et billede...
              </div>
              <b-img v-else style="height:inherit" :src="uploadedImageURL"></b-img>
              <input
                type="file"
                id="uploadedImage"
                ref="uploadedImage"
                v-on:change="handleFileUpload()"
              />
            </div>
          </b-col>
        </b-row>
      </b-form-group>
      <template v-slot:modal-footer="{cancel}">
        <div class="w-100">
          <b-button
            style="margin-left:10px"
            class="float-right"
            variant="primary"
            @click="uploadImage"
          >Ok</b-button>
          <b-button class="float-right" @click="cancel()">Annuller</b-button>
        </div>
      </template>
    </b-modal>
</template>

<script>
export default {
    name: 'UploadImageModal',
    data() {
        return {
            uploadedImageURL: "",
            uploadImageName: "",
            uploadedImage: "",
            isUploaded: false,
            imageUploadBorder: "fileBorder",
            showUploadImageModal: false,
            standardImageURI: process.env.VUE_APP_FILEUPLOAD
        }
    },
    props: {
        //must be sync
        Image: Object
    },
    computed: {
        NoImageName() {
            if (this.uploadImageName != '') {
                return true
            } else {
                return false
            }
        },

    },
    methods: {
        uploadImage() {
            let formData = new FormData();
            formData.append("file", this.uploadedImage);
            formData.append("imageName", this.uploadImageName);

            if (this.uploadImageName == '') {
                return
            }

            this.$store
                .dispatch("images/uploadImage", formData)
                    .then(resolve => {
                        this.$emit('update:Image', resolve)
                        this.$emit('uploadet', resolve)
                        // eslint-disable-next-line no-console
                        console.log("SUCCESS!!");
                        this.closeModal()
                        this.showUploadImageModal = false;
                    })
                    .catch(error => {
                        // eslint-disable-next-line no-console
                        console.log(error);
            });
        },
        handleFileUpload() {
            this.imageUploadBorder = "";
            this.uploadedImage = this.$refs.uploadedImage.files[0];
            this.uploadedImageURL = URL.createObjectURL(this.uploadedImage);

            this.isUploaded = true;
        },
        closeModal(){
            this.uploadImageName = "";
            this.uploadedImageURL = "";
            this.imageUploadBorder = "fileBorder";
            this.isUploaded = false;
        }
    }
}
</script>