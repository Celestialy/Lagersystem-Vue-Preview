
<script>
export default {
    
    created(){
    this.$store.dispatch('auth/destroyToken').then(() => {
            this.$router.push('/')
          })
    } 
}
</script>