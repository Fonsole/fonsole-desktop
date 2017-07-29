<template>
  <div id="topbar">
    <ul>
      <li><img src="../../assets/menu_logo.png"></img></li>
      <li v-for="(buttonName,index) in buttons">
        <a @click="onButtonClicked" :id="buttonName">{{ $localize(buttonName) }}</a>
      </li>
    </ul>
  </div>
</template>

<script>
  export default {
    name: 'topbar',
    data() {
      return {
        buttons: ['home', 'games', 'shop', 'community', 'settings'],
      };
    },
    methods: {
      onButtonClicked(event) {
        if (event.target.id === 'Games') {
          this.$router.push('/game/test');
        }
      },
    },
  };
</script>

<style lang="sass">
  @font-face
    font-family: 'zekton'
    src: url('../../assets/zekton.ttf') format('truetype')
    font-weight: normal
    font-style: normal

  ul
    list-style-type: none
    margin: 0
    padding: 0

    li
      float: left

      a, img
        font-family: 'zekton'
        display: block
        color: white
        opacity: 0.8
        text-align: center
        padding: 14px 16px
        text-decoration: none
        text-transform: uppercase
        font-size: 20px
        transition: transform 0.1s linear 0.0s
        cursor: pointer

      img
        max-height: 24px

      a:hover, img:hover
        transform: scale(1.1)


  #topbar
    margin: 0px
    width: 100%
    // height: 70px
    background-color: rgba(20,20,20,.8)
    position: absolute
</style>
