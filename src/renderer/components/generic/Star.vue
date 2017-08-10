<template>
  <div>
    <img src="~@/assets/star.png" :height="getSize" :width="getSize" @mousemove="mouseMoving"
    @click="selected()" style="overflow:visible;" :class="`${ this.lit }`">

    </img>
  </div>
</template>

<script type="text/javascript">
export default {
  props: {
    fill: {
      type: Number,
      default: 0,
    },
    size: {
      type: Number,
      default: 50,
    },
    starId: {
      type: Number,
      required: true,
    },
    activeColor: {
      type: String,
      required: true,
    },
    inactiveColor: {
      type: String,
      required: true,
    },
    borderColor: {
      type: String,
      default: '#000',
    },
    borderWidth: {
      type: Number,
      default: 0,
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
      starPoints: [19.8, 2.2, 6.6, 43.56, 39.6, 17.16, 0, 17.16, 33, 43.56],
      grad: '',
    };
  },
  computed: {
    getGradId() {
      return `url(#${this.grad})`;
    },
    getSize() {
      return parseInt(this.size, 10) + parseInt(this.borderWidth * 3, 10) + this.padding;
    },
    getFill() {
      return (this.rtl) ? `${100 - this.fill}%` : `${this.fill}%`;
    },
  },
  created() {
    this.grad = Math.random().toString(36).substring(7);
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
            // calculate position in percentage.
      const starWidth = (92 / 100) * this.size;
      const position = Math.round((100 / starWidth));
      return Math.min(position, 100);
    },
    selected($event) {
      console.log(this.lit);
      this.$emit('star-selected', {
        id: this.starId,
        position: this.getPosition($event),
      });
    },
  },
};
</script>

<style lang="sass" scoped>
  img
    user-drag: none
    user-select: none
    transition: all 0.25s ease

  .false
    filter: grayscale(85%)

  .true
    filter: grayscale(0%)
</style>
