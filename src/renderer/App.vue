<template>
  <div id="app">
    <body>
      <rings></rings>
      <div id="appContent">
        <div id="topbarContainer">
          <topbar></topbar>
        </div>
        <div id="routerContainer">
          <component
            :is="page"
            transition="fade"
            transition-mode="out-in"
          ></component>
        </div>
      </div>
    </body>
  </div>
</template>

<script>
  import Rings from '@/components/rings/Rings';
  import Topbar from '@/components/topbar/Topbar';
  import About from '@/components/about/About';
  import Game from '@/components/game/Game';
  import Home from '@/components/home/Home';
  import Games from '@/components/games/Games';
  import Shop from '@/components/shop/Shop';
  import Community from '@/components/community/Community';
  import Settings from '@/components/settings/Settings';

  export default {
    name: 'App',
    components: {
      rings: Rings,
      topbar: Topbar,
      about: About,
      game: Game,
      home: Home,
      games: Games,
      shop: Shop,
      community: Community,
      settings: Settings,
    },
    computed: {
      page() {
        return this.$store.state.gui.page;
      },
    },
  };
/* eslint-disable max-len */
</script>

<style lang="sass">
  @font-face
    font-family: 'zekton'
    src: url('./assets/zekton.ttf') format('truetype')
    font-weight: normal
    font-style: normal

  $font-stack:    zekton, Helvetica, sans-serif
  $primary-color: white

  $dividerColor: rgba(255,255,255,0.65)
  $dividerColorTransparent: rgba(255,255,255,0)

  body
    margin: 0
    padding: 0
    overflow: hidden
    background-color: #222
    color: $primary-color
    font-family: 'zekton'
    text-transform: uppercase

  label, h1, h2, div
    cursor: default
    user-select: none
    font-weight: 100

  #appContent
    height: 100%
    width: 100%
    position: absolute
    display: flex
    flex-direction: column

  #topbarContainer
    flex: 0 1 auto
    background-color: rgba(20,20,20,.8)

  #routerContainer
    flex: 1 1 auto
    margin: 15px
    display: flex
    flex-direction: column

    .tabContents
      flex: 1 1 auto
      margin: 0px

  .divider
    margin-top: 6px
    margin-bottom: 6px
    margin-left: 1px
    width: 100%
    height: 2px
    background: linear-gradient(left, $dividerColor 0%, $dividerColor 5%, $dividerColor 50%, $dividerColorTransparent 95%, $dividerColorTransparent 100%)

  .fade-transition
    transition: opacity 0.2s ease

  .fade-enter, .fade-leave
    opacity: 0
</style>
