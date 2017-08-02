<template>
  <div id="topbar">
    <ul>
      <li><img src="../../assets/menu_logo.png"></img></li>
      <li v-for="(buttonName,index) in buttons">
        <a @click="onButtonClicked" :id="buttonName">{{ $localize(buttonName) }}</a>
      </li>
      <li id="roomName">
        ROOM CODE: {{ roomName }}
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
        this.$store.state.currentContentIndex = event.target.id;
      },
    },
    computed: {
      roomName() {
        return this.$store.state.roomName;
      },
    },
  };
</script>

<style lang="sass" scoped>
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

  #roomName
    float: right
    font-weight: 900
    font-family: 'zekton'
    text-align: center
    padding: 14px 16px
    text-decoration: none
    text-transform: uppercase
    font-size: 20px
    transition: transform 0.1s linear 0.0s
    color: lime
</style>
