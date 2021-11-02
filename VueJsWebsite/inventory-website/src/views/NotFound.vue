<template>
<div class="NotFound">
  <b-container>
        <b-card style="margin-top: 2%" height="400" color="transparent">
          <div class="display-3 mt-5">Page not found.</div>
          <div class="grey--text lighten-5">
            The page you are trying to get to never existed in this reality,
            or has migrated to a parallel universe.
          </div>
          <div class="paragraph-text mt-2">
            <br />
            <br />Try going back to home page and repeating your action.
          </div>
        </b-card>
  </b-container>
</div>
</template>

<script>
export default {
  created() {
  }
}
</script>

<style>
.paragraph-text {
  font-size: 18px;
}
</style>