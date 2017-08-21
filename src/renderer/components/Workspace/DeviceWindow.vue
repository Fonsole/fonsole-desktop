<template>
  <vue-draggable-resizable
    class="device-window"
    :x="x"
    :y="y"
    :w="w"
    :h="h"
    :minw="minw"
    :minh="minh"
    :parent="true"
    @activated="activated"
    @deactivated="deactivated"
    @resizing="resizing"
    @resizestop="resizestop"
    @dragging="dragging"
    @dragstop="dragstop"
  >
    {{`DEVICE: ${session}`}}
  </vue-draggable-resizable>
</template>

<script>
  import VueDraggableResizable from 'vue-draggable-resizable';

  export default {
    name: 'DeviceWindow',
    components: {
      VueDraggableResizable,
    },
    props: {
      session: {
        type: String,
        required: true,
      },
      isDesktop: {
        type: Boolean,
        required: true,
      },
    },
    data: () => ({
      x: 0,
      y: 0,
      w: 1,
      h: 1,
      minw: 1,
      minh: 1,
    }),
    beforeMount() {
      const MARGIN = 10;
      // Get browser viewport and divide it by 100, to make it like css unit
      const vw = Math.max(document.documentElement.clientWidth, window.innerWidth || 0) / 100;
      const vh = Math.max(document.documentElement.clientHeight, window.innerHeight || 0) / 100;
      // Set initial resolutions depending on emulated device type
      this.w = this.isDesktop ? 60 * vw : 20 * vw;

      this.h = 55 * vh;
      // Minimal resolutions are 10% of initial
      this.minw = this.w * 0.1;
      this.minh = this.h * 0.1;
      // TODO
      // Try to find clear space
      const otherWindows = this.$parent.$children;
      // Select pre-last child
      const lastChild = otherWindows[otherWindows.length - 1];
      if (lastChild) {
        const desiredX = lastChild.x + lastChild.w + MARGIN;
        if (desiredX + this.w > this.$parent.$el.offsetWidth) {
          this.x = desiredX;
          return;
        }
        const desiredY = lastChild.y + lastChild.h + MARGIN;
        if (desiredY + this.h > this.$parent.$el.offsetHeight) {
          this.y = desiredY;
        }
      }
    },
    methods: {
      activated() {
        this.$emit('activated');
      },
      deactivated() {
        this.$emit('deactivated');
      },
      resizing(left, top, width, height) {
        this.$emit('resizing', left, top, width, height);
      },
      resizestop(left, top, width, height) {
        this.$emit('resizestop', left, top, width, height);
      },
      dragging(left, top) {
        this.$emit('dragging', left, top);
      },
      dragstop(left, top) {
        this.$emit('dragstop', left, top);
      },
    },
  };
</script>

<style lang="sass" scoped>
  .device-window
    background-color: #888
</style>
