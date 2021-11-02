<template>
  <div class="d-flex"> 
    <div>
      <sidebar/>
    </div>
    <div style="display: contents;">
        <router-view> </router-view>
    </div>
  </div>
</template>

<script>
import sidebar from 'components/admin/adminNav'
export default {
  name: 'adminIndex',
  components: {
    sidebar
  }
}
</script>