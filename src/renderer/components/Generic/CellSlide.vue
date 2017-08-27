<template>
  <div class="cellSlide">
    <div class="cellContent"
    :style="{ backgroundImage: 'url(' + backgroundImage + ')' }">

    </div>
    <div class="cellCaption">
      <h2>{{ caption }}</h2>
    </div>
  </div>
</template>

<script>
  export default {
    name: 'CellSlide',
    props: {
      caption: {
        type: String,
        default: 'Caption!',
      },
      backgroundURL: {
        type: String,
        default: 'testgame3.jpg',
      },
    },
    data: () => ({
    }),
    computed: {
      backgroundImage() {
        // eslint-disable-next-line import/no-dynamic-require
        return require(`@/assets/${this.backgroundURL}`);
      },
    },
  };
</script>

<style lang="sass" scoped>
  h2
    font-size: 2.5vw
    margin: 1vh

  .cellSlide
    // flex: 1 1 auto
    background-color: rgba(20,20,20,0.8)
    overflow: hidden
    display: flex
    flex-direction: column
    position: absolute
    height: 100%
    width: 100%
    transition: all 0.3s ease

    .cellCaption
      position: absolute
      bottom: 0
      width: 100%
      background: linear-gradient(to top, rgba(0,0,0,0.8), rgba(0,0,0,0))
      margin: 0

    .cellContent
      max-width: 100%
      max-height: 100%
      display: flex
      flex: 1 1 auto
      box-shadow: 0 0 5vh rgba(0,0,0,0.9) inset
      position: relative
      background-position: center
      background-size: cover
      background-repeat: no-repeat
</style>
