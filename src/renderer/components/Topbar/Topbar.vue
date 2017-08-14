<template>
  <div id="topbar">
    <ul>
      <li>
        <div class="">
          <img src="~@/assets/menu_logo.png" @click="onButtonClicked" id="about"></img>
        </div>
      </li>
      <li v-for="(buttonName,n) in buttons" :key="n">
        <div class="">
          <a @click="onButtonClicked" :id="buttonName">{{ $localize(buttonName) }}</a>
        </div>
        <div :class="['pointer', {'lit':buttonName == currentPage}]">
          <div class="circle">

          </div>
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
      currentPage() {
        return this.$store.state.gui.page;
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
      position: relative

      &:hover
        .pointer
          opacity: 0.9

      .pointer
        width: 1vh
        height: 1vh
        position: absolute
        bottom: 5%
        left: 47%
        transition: all 0.65s ease
        opacity: 0.0

        .circle
          margin: 0 auto
          width: 0.6vh
          height: 0.6vh
          background: #2da0e3
          border-radius: 0.3vh
          margin: auto
          transition: box-shadow 0.3s ease

        &.lit
          opacity: 0.9
          .circle
            box-shadow: 0 0 0.6vh rgba(255,255,255,0.5)

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
