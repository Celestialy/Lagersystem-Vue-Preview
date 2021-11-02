<template>
  <div class="home">
    <b-container style="margin-top: 1%">
        <h1>LagerSystem</h1>
        <div>
          <b-card no-body>
          <b-tabs v-model="currentTab" card>
            <b-tab title="Home"> </b-tab>
            <b-tab v-if="IsLoadingDone.MostUsedItemsCompleted" title="Mest Brugte">
          <card-table :Items="Statistics.MostUsedItems"/>
          
          </b-tab>
          <b-tab title="Mangel" v-if="IsLoadingDone.LowStockCompleted">
            <card-table :Items="Statistics.LowStock"/>
          </b-tab>
          </b-tabs>
          </b-card>
        </div>
    </b-container>
  </div>
</template>

<script>
// @ is an alias to /src
import cardTable from 'src/components/CardTable/Cardtable.vue'
export default {
  name: "home",
  data() {
    return {
      currentTab: 0
    };
  },
  computed: {
    Statistics() {
      return this.$store.getters["stats/GetEveryStatistic"];
    },
    IsLoadingDone() {
      return this.$store.getters["stats/IsLoadingCompleted"];
    },
    
  },
  components: {
    cardTable
  },
  created() {
    this.$store.dispatch("stats/GetStatistics");
  }
};
</script>