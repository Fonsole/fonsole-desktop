<template>
  <div id="shop" class="tabContents">
    <fbutton width="27" height="6">FButton</fbutton>
  </div>
</template>

<script>
  import FButton from '../generic/FButton';

  export default {
    name: '',
    components: {
      fbutton: FButton,
    },
    data: () => ({

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
