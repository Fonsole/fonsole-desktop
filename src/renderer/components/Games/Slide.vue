<template>
  <div class="carousel-3d-slide" :style="slideStyle" :class="{ 'current': isCurrent }" @click="goTo()">
    <preview :gameID="`${ this.gameID }`"></preview>
  </div>
</template>

<script>
  import Preview from './Preview';

  export default {
    name: 'Slide',
    components: {
      preview: Preview,
    },
    props: {
      index: {
        type: Number,
      },
      gameID: {
        type: Number,
      },
    },
    data() {
      return {
        parent: this.$parent,
        styles: {},
        zIndex: 999,
      };
    },
    computed: {
      isCurrent() {
        return this.index === this.parent.currentIndex;
      },
      slideStyle() {
        let styles = {};

        if (!this.isCurrent) {
          const rIndex = this.getSideIndex(this.parent.rightIndices);
          const lIndex = this.getSideIndex(this.parent.leftIndices);          

          if (rIndex >= 0 || lIndex >= 0) {
            styles = rIndex >= 0 ? this.calculatePosition(rIndex, true, this.zIndex, this.parent.rightIndices.length) : 
                                   this.calculatePosition(lIndex, false, this.zIndex, this.parent.leftIndices.length);
            let idxInDeck = Math.max(rIndex, lIndex)
            styles.opacity = 1.0 / (1.25 + idxInDeck * 0.75);
            styles.visibility = 'visible';
          }
          else {
            styles.opacity = 0;
            styles.visibility = 'hidden';
          }
        }

        return Object.assign(styles, {
          'border-width': `${this.parent.border}px`,
          width: `${this.parent.slideWidth}vh`,
          height: `${this.parent.slideHeight}vh`,
          transition: ` transform ${this.parent.animationSpeed}ms, ` +
                      ` opacity ${this.parent.animationSpeed}ms `,
        });
      },
    },
    methods: {
      getSideIndex(array) {
        let index = -1;

        array.forEach((pos, i) => {
          if (this.matchIndex(pos)) {
            index = i;
          }
        });

        return index;
      },
      matchIndex(index) {
        return (index >= 0) ? this.index === index : (this.parent.total + index) === this.index;
      },
      calculatePosition(i, positive, zIndex, deckSize) {
        const offsetInDeck = 10;
        const z = -75;
        const yAngle = 70;
        const leftRemain = parseInt((this.parent.width * 0.8 + i * offsetInDeck), 10);
        const transform = (positive)
                  ? `translateX(${leftRemain}vh) translateZ(${z}vh) ` +
              `rotateY(-${yAngle}deg)`
                  : `translateX(-${leftRemain}vh) translateZ(${z}vh) ` +
              `rotateY(${yAngle}deg)`;
        const top = this.parent.space === 'auto' ? 0 : parseInt((i + 1) * (this.parent.space));

        return {
          transform,
          top,
          zIndex: zIndex - (Math.abs(i) + 1),
        };
      },
      goTo() {
        if (this.isCurrent) {
          this.$store.commit('setPage', 'gameView');
        } else {
          if (this.parent.clickable === true) {
            this.parent.goFar(this.index);
          }
        }
      },
    },
  };
</script>

<style lang="sass" scoped>
.carousel-3d-slide
  position: absolute
  opacity: 0
  visibility: hidden
  overflow: hidden
  top: 0
  border-radius: 1px
  border-color: rgba(0, 0, 0, 0.4)
  border-style: solid
  background-size: cover
  background-color: #ccc
  display: block
  margin: 0
  box-sizing: border-box
  text-align: left
  img
    width: 100%

  &.current
    opacity: 1 !important
    visibility: visible !important
    transform: none !important
    z-index: 999
</style>
