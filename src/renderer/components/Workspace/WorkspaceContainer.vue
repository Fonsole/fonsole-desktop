<template>
  <grid-layout
    class="workspace-container"
    :layout="layout"
  >
    <grid-item
      class="window"
      v-for="item in layout"
      :key="item.i"
      :x="item.x"
      :y="item.y"
      :w="item.w"
      :h="item.h"
      :i="item.i"
    >
      {{`DEVICE: ${item.i}`}}
    </grid-item>
  </grid-layout>
</template>

<script>
  import { GridLayout, GridItem } from 'vue-grid-layout/dist/vue-grid-layout.min';

  export default {
    name: 'WorkspaceContainer',
    components: {
      GridLayout,
      GridItem,
    },
    data: () => ({
      layout: [
        { x: 0, y: 0, w: 6, h: 2.5, i: 'DESKTOP' },
      ],
    }),
    mounted() {
      this.layout.push({ x: 0, y: 0, w: 3, h: 3, i: 'CONTROLLER1' });
    },
  };
</script>

<style lang="sass" scoped>
  .workspace-container
    flex: 1
    background-color: rgba(0, 0, 0, 0.4)

  .window
    background-color: #888
</style>
