<template>
  <div id="installed" ref="dick">
    <carousel-3d :height="height" :controls-visible="true">
      <slide v-for="(id,index) in installedGames" :index="index">
        <preview :gameID="id"></preview>
      </slide>
    </carousel-3d>
  </div>
</template>
<script>
  import Carousel3d from './Carousel3d';
  import Slide from './Slide';
  import Preview from './Preview';

  export default {
    name: 'installed',
    components: {
      Carousel3d,
      Slide,
      Preview,
    },
    data: () => ({
      height: 0,
      installedGames: [0, 1, 2, 3, 4, 5],
    }),
    created() {
      this.$nextTick(() => {
        this.height = this.$parent.clientHeight;
      });
    },
  };
</script>
<style lang="sass" scoped>
  #installed
    width: 90%
    margin: 0 auto
    display: flex
    flex-direction: row
    flex-wrap: wrap
    justify-content: center
    align-items: center
</style>
