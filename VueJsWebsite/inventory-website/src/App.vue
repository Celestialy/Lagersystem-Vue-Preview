<template>
  <div id="app">
    <div id="navigation-container" >
      <!--div style="overflow: hidden; height: 100%; width: 100%; bottom: 0; top: 0; position: absolute;">
      <video style="min-width: 100%;  min-height: 100%; z-index: 2;  width: auto; height: auto; position: absolute; top: 50%; left: 50%; transform: translate(-50%,-50%); " loop autoplay preload="auto">
        <source src="http://rickrollomatic.com/rickroll.mp4" type="video/mp4">
      </video>
      </div>
       <iframe style="position: fixed; z-index:999999; overflow:hidden; display: block; border: none;  height: 100vh; width: 100vw;" src="https://www.youtube.com/embed/dQw4w9WgXcQ?autoplay=1" frameborder="0" allowfullscreen></iframe-->
      <navbar />
      
    </div>
    <div>
        <router-view/>
    </div>
  </div>
</template>

<script>
import navbar from 'src/components/Navigation'

export default {
  name: 'App',
  components: {
    navbar
  }
}
</script>

<style lang="scss">
@import './style/style.scss';
</style>
