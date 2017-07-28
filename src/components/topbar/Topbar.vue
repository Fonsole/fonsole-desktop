<template>
  <div id="topbar">
    <ul>
      <li><img src="../../assets/menu_logo.png"></img></li>
      <li v-for="(buttonName,index) in buttons">
        <a @click="onButtonClicked" :id="buttonName">{{ buttonName }}</a>
      </li>
    </ul>
  </div>
</template>

<script>
  export default {
    name: 'topbar',
    data() {
      return {
        buttons: ['Home', 'Games', 'Shop', 'Community', 'Settings'],
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
        cursor: pointer

      img
        max-height: 25px


  #topbar
    margin: 0px
    width: 100%
    // height: 70px
    background-color: rgba(20,20,20,.8)
    position: absolute
</style>
