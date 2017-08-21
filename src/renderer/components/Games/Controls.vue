<template>
  <div class="carousel-3d-controls">
    <a href="#"
      @click.prevent="this.$parent.goPrev()"
      :class="['prev', { disabled: !this.$parent.isPrevPossible }]"
      :style="buttonStyle">
      <span v-html="prevHtml"></span>
    </a>
    <a href="#"
      @click.prevent="this.$parent.goNext()"
      :class="['next', { disabled: !this.$parent.isNextPossible }]"
      :style="buttonStyle">
      <span v-html="nextHtml"></span>
    </a>
  </div>
</template>

<script>
  export default {
    name: 'Controls',
    props: {
      /**
       * Width in pixels of the navigation buttons
       */
      width: {
        type: [String, Number],
        default: 50,
      },

      /**
       * Height in pixels of the navigation buttons
       */
      height: {
        type: [String, Number],
        default: 60,
      },

      /**
       * Text content of the navigation prev button
       */
      prevHtml: {
        type: String,
        default: '&lsaquo;',
      },

      /**
       * Text content of the navigation next button
       */
      nextHtml: {
        type: String,
        default: '&rsaquo;',
      },
    },
    computed: {
      buttonStyle() {
        return {
          width: this.width,
          height: this.height,
          lineHeight: this.height,
        };
      },
    },
  };
</script>

<style lang="sass" scoped>
  .carousel-3d-controls
    position: absolute
    top: 50%
    height: 0
    margin-top: -30px
    left: 0
    width: 100%
    z-index: 9099

  .next
    width: 60px
    position: absolute
    z-index: 9999
    font-size: 60px
    height: 60px
    line-height: 60px
    color: white
    user-select: none
    text-decoration: none
    top: 0
    right: 10px
    text-align: right
    &:hover
      cursor: pointer
      opacity: 0.7

  .prev
    width: 60px
    position: absolute
    z-index: 9999
    font-size: 60px
    height: 60px
    line-height: 60px
    color: white
    user-select: none
    text-decoration: none
    top: 0
    left: 10px
    text-align: left
    &:hover
      cursor: pointer
      opacity: 0.7

  .disabled
    opacity: 0.2
    cursor: default
    &:hover
      cursor: default
      opacity: 0.2
</style>
