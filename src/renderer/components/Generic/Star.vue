<template>
  <div>
    <img src="../../assets/star.png" @mousemove="mouseMoving" @click="selected()"
    :style="{width: this.size + 'vh', height: this.size + 'vh', overflow: 'visible'}"
    :class="`${ this.rtl ?
      this.currentRating <= this.starId : this.currentRating >= this.starId }`">

    </img>
  </div>
</template>

<script>
  export default {
    props: {
      fill: {
        type: Number,
        default: 0,
      },
      size: {
        type: [Number, String],
        default: 50,
      },
      currentRating: {
        type: Number,
        default: 0,
      },
      starId: {
        type: Number,
        required: true,
      },
      padding: {
        type: Number,
        default: 0,
      },
      rtl: {
        type: Boolean,
        default: false,
      },
      lit: {
        type: Boolean,
        default: false,
      },
    },
    data() {
      return {

      };
    },
    computed: {
      getGradId() {
        return `url(#${this.grad})`;
      },
      getFill() {
        return (this.rtl) ? `${100 - this.fill}%` : `${this.fill}%`;
      },
    },
    created() {
    },
    methods: {
      mouseMoving($event) {
        this.$emit('star-mouse-move', {
          event: $event,
          position: this.getPosition($event),
          id: this.starId,
        });
      },
      getPosition() {
        const starWidth = (92 / 100) * this.size;
        const position = Math.round((100 / starWidth));
        return Math.min(position, 100);
      },
      selected($event) {
        this.$emit('star-selected', {
          id: this.starId,
          position: this.getPosition($event),
        });
      },
    },
  };
</script>

<style lang="sass" scoped>
  div
    margin: 0
    padding: 0

  img
    user-drag: none
    user-select: none
    transition: all 0.25s ease

  // img:hover
  //   filter: grayscale(65%)

  .false
    filter: grayscale(85%)

  .true
    filter: grayscale(0%)
</style>
