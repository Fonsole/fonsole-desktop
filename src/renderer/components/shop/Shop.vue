<template>
  <div id="shop" class="tabContents">
    <fbutton width="22" height="6" type="install">install</fbutton>
    <fbutton width="22" height="6" type="progress">installing</fbutton>
    <fbutton width="22" height="6" type="progress-dark">removing</fbutton>
    <star-rating v-model="rating"></star-rating>
  </div>
</template>

<script>
  import FButton from '../generic/FButton';
  import StarRating from '../generic/StarRating';

  export default {
    name: '',
    components: {
      fbutton: FButton,
      'star-rating': StarRating,
    },
    data: () => ({
      rating: 3,
    }),
    methods: {
    },
  };
</script>

<style lang="sass" scoped>
  #shop
    display: block
    width: 100%
    height: 100%
    background-color: rgba(20,20,20,.8)

    padding: 15px
</style>
