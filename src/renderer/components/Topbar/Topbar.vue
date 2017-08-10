<template>
  <div id="topbar">
    <ul>
      <li>
        <div class="">
          <img src="~@/assets/menu_logo.png" @click="onButtonClicked" id="about"></img>
        </div>
      </li>
      <li v-for="buttonName in buttons" :key="buttonName">
        <div class="">
          <a @click="onButtonClicked" :id="buttonName">{{ $localize(buttonName) }}</a>
        </div>
      </li>
      <li id="roomName">
        <div class="">
          <a @click="onButtonClicked" id="room">{{ $localize('room', { roomName: roomName }) }}</a>
        </div>
      </li>
    </ul>
  </div>
</template>

<script>
  export default {
    name: 'Topbar',
    data: () => ({
      buttons: ['home', 'games', 'shop', 'community', 'settings'],
    }),
    computed: {
      roomName() {
        return this.$store.state.networking.roomName || '...';
      },
    },
    methods: {
      onButtonClicked(event) {
        this.$store.commit('setPage', event.target.id);
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
      vertical-align: middle

      & div
        height: 6vh
        display: table-cell
        vertical-align : middle

      a, img
        font-family: 'zekton'
        display: block
        color: white
        opacity: 0.8
        text-align: center
        padding: 0px 14px
        text-decoration: none
        text-transform: uppercase
        font-size: 2.5vh
        transition: transform 0.1s linear 0.0s
        cursor: pointer

      img
        width: 2.5vh

      a:hover, img:hover
        transform: scale(1.1)

  #roomName
    float: right
</style>
