<template>
  <div>
    <iframe id="game" ref="game" :src="url"></iframe>
  </div>
</template>
<script>
export default {
  name: 'Game',
  data() {
    return {
      url: '',
    };
  },
  mounted() {
    this.$store.dispatch('attachNetworkingApi', this.$refs.game);
    this.url = '/static/games/test.html';
  },
};
</script>

<style lang="sass" scoped>
  div
    width: 100%
    height: 100%
  #game
    border: none
    position: absolute
    width: 100%
    height: 100%
</style>

