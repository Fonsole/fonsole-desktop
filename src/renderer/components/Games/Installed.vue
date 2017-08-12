<template>
  <div id="installed" ref="dick">
    <carousel-3d height="50" width="80" :display="50" :controls-visible="true" :loop="false">
      <slide v-for="(id,index) in installedGames" :index="index" :key="id" :gameID="id">
      </slide>
    </carousel-3d>
  </div>
</template>
<script>
  import Carousel3d from './Carousel3d';
  import Slide from './Slide';
  import Preview from './Preview';

  export default {
    name: 'Installed',
    components: {
      Carousel3d,
      Slide,
      Preview,
    },
    data: () => ({
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
    width: 100%
    margin: 0 auto
    display: flex
    flex-direction: row
    flex-wrap: wrap
    justify-content: center
    align-items: center
</style>
