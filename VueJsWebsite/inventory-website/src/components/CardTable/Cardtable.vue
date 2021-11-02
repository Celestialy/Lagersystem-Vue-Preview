<template>
    <b-row>
          <b-card v-for="item in Items"
            :key="item.id"
            :img-src="item.imageUri"
            img-alt="Card image"
            img-top

            style="width: 20%; margin-bottom: 5px; "
            class="float-left"
          >
          <b-card-text>Mærke: {{item.brand}} </b-card-text>
          <b-card-text>Model: {{item.model}}</b-card-text>
          <b-card-text>brugt: {{item.totalAmount - item.amountLeft}}</b-card-text>
          <b-card-text>Tilbage: {{item.amountLeft}}</b-card-text>
          <b-card-text>ialt: {{item.totalAmount}}</b-card-text>
          </b-card>
          </b-row>
</template>

<script>
export default {
    name: 'CardTable',
    props: {
        Items: []
    }
}
</script>