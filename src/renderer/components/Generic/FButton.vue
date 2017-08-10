<template>
  <div id="fbutton" :style="{width: this.width + 'vh', height: this.height + 'vh'}"
  :class="this.type">
    <div id="background">
    </div>
    <div id="button" :style="{fontSize: this.height/2 + 'vh'}" @click="handleClick">
      <slot>
      </slot>
    </div>
  </div>
</template>

<script>
  export default {
    name: 'FButton',
    props: {
      width: {
        type: [Number, String],
        default: 360,
      },
      height: {
        type: [Number, String],
        default: 270,
      },
      type: {
        type: [String],
        default: 'default',
      },
    },
    data: () => ({
      colours: ['#69D2E7', '#A7DBD8', '#E0E4CC', '#F38630', '#FA6900', '#FF4E50', '#F9D423'],
    }),
    methods: {
      handleClick(e) {
        this.$emit('click', e);
      },
    },
  };
</script>

<style lang="sass" scoped>
  $primary-background-width: 23.5vh

  #fbutton
    margin: 10px

    & > div
      position: absolute
      top: 50%
      left: 50%
      transform: translate(-50%, -50%)

    &.install
      box-shadow: 0px 0px 0px #232530
      background: radial-gradient(ellipse at center, #232530 0%,#232530 100%)
      transition: transform 0.15s, box-shadow 0.15s
      transform: rotateX(0deg) translateY(0)
      -webkit-font-smoothing: antialiased

      &:hover
        box-shadow: 0px 5px 0px #232530
        transform: rotateX(30deg) translateY(0)

      &:hover:active
        box-shadow: 0 1px 0 #423847
        background: radial-gradient(ellipse at center, #232530 0%,#232530 100%)
        transition: transform 0.05s, box-shadow 0.05s
        transform: rotateX(30deg) translateY(4px)

      > #background
        background: linear-gradient(-45deg,
        #5e6aa7 10%, #8e96c1 10%,
        #8e96c1 30%, #5e6aa7 30%,
        #5e6aa7 50%, #8e96c1 50%,
        #8e96c1 70%, #5e6aa7 70%,
        #5e6aa7 90%, #8e96c1 90%)
        background-size: $primary-background-width 100%
        animation: 'progress-forward' 12s infinite linear
        width: 100%
        height: 100%

    &.progress
      background: radial-gradient(ellipse at center, #D60D2E 0%,#C20020 100%)
      transition: transform 0.15s, box-shadow 0.15s
      -webkit-font-smoothing: antialiased
      box-shadow: 0px 5px 0px #750014
      transform: rotateX(30deg) translateY(0)

      > #background
        background: linear-gradient(-45deg,
        #A7DBD8 10%, #c7c12f 10%,
        #c7c12f 30%, #f26247 30%,
        #f26247 50%, #ec2045 50%,
        #ec2045 70%, #A7DBD8 70%,
        #A7DBD8 90%, #c7c12f 90%)
        background-size: $primary-background-width 100%
        animation: 'progress-forward' 3s infinite linear
        width: 100%
        height: 100%

    &.progress-dark
      background: radial-gradient(ellipse at center, #222 0%,#222 100%)
      transition: transform 0.15s, box-shadow 0.15s
      -webkit-font-smoothing: antialiased
      box-shadow: 0px 5px 0px #111
      transform: rotateX(30deg) translateY(0)

      > #background
        background: linear-gradient(-45deg,
        #222 10%, #333 10%,
        #333 30%, #222 30%,
        #222 50%, #333 50%,
        #333 70%, #222 70%,
        #222 90%, #333 90%)
        background-size: $primary-background-width 100%
        animation: 'progress-backward' 3s infinite linear
        width: 100%
        height: 100%

  .wobble-horizontal
    display: inline-block
    // box-shadow: 0 0 1px rgba(0, 0, 0, 0)
    // transform: perspective(1px) translateZ(0)

    & > *:hover
      animation: 'wobble-horizontal' 1s 1 ease-in-out

  @keyframes 'progress-forward'
    0%
      background-position-x: 0vh
    100%
      background-position-x: $primary-background-width

  @keyframes 'progress-backward'
    0%
      background-position-x: $primary-background-width
    100%
      background-position-x: 0vh

  @keyframes 'wobble-horizontal'
    16.65%
      transform: translateX(8px)
    33.3%
      transform: translateX(-6px)
    49.95%
      transform: translateX(4px)
    66.6%
      transform: translateX(-2px)
    83.25%
      transform: translateX(1px)
    100%
      transform: translateX(0)
</style>
