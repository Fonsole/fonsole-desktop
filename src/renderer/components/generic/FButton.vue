<template>
  <div id="fbutton" :style="{width: this.width + 'vh', height: this.height + 'vh'}">
    <div ref="particlesBG" id="particlesBG">
      <!-- <div v-for="n in 10" :key="n" class="particle">
      </div> -->
    </div>
    <div id="button" :style="{fontSize: this.height/2 + 'vh'}">
      <slot>
      </slot>
    </div>
  </div>
</template>

<script>
  export default {
    name: 'Fbutton',
    props: {
      width: {
        type: [Number, String],
        default: 360,
      },
      height: {
        type: [Number, String],
        default: 270,
      },
    },
    data: () => ({
      colours: ['#69D2E7', '#A7DBD8', '#E0E4CC', '#F38630', '#FA6900', '#FF4E50', '#F9D423'],
    }),
    mounted() {

    },
  };
</script>

<style lang="sass" scoped>
  #fbutton
    position: relative

    #particlesBG, #button
      position: absolute
      top: 50%
      left: 50%
      transform: translate(-50%, -50%)
    #particlesBG
      background: repeating-linear-gradient(-45deg,
      #F9D423 0%,
      #F9D423 10%, #c7c12f 10%,
      #c7c12f 30%, #f26247 30%,
      #f26247 50%, #ec2045 50%,
      #ec2045 70%, #A7DBD8 70%,
      #A7DBD8 90%, #F9D423 90%)
      background-size: 60vh 100%
      -webkit-animation: 'moving-gradient' 8s infinite linear
      width: 100%
      height: 100%

  @-webkit-keyframes 'moving-gradient'
    0%
      background-position-x: 0vh
    100%
      background-position-x: 60vh
</style>
