<template>
  <div id="about" class="tabContents">
    <div class="container" transition="fade" transition-mode="out-in" appear>
      <a class="logoContainer">
        <div class="logo animateGears">
          <div class="logoInner">
            <div class="logoGear small"></div>
            <div class="logoGear large"></div>
          </div>
        </div>
      </a>
      <a class="gradient">Fonsole</a>
      <a>alpha</a>
    </div>
  </div>
</template>

<script>
  export default {
    name: 'About',
    data: () => ({

    }),
  };
</script>

<style lang="sass" scoped>
  @import "~@/assets/styles/global.sass"

  #about
    display: block
    width: 100%
    height: 100%
    background-color: rgba(20,20,20,.8)

    .container
      position: absolute
      top: 50%
      left: 50%
      transform: translate(-50%, -50%)

      display: flex
      flex-direction: column

      font-size: 3vh
      text-align: center

      img
        width: 100%
        height: 100%

      .gradient
        background: -webkit-linear-gradient(135deg, #F38630, #A7DBD8, #FF4E50)
        -webkit-background-clip: text
        -webkit-text-fill-color: transparent
        font-size: 8vh

      .logoContainer
        flex: 0 1 auto

      & > *
        // flex: 0 1 auto


</style>
