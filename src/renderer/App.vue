<template>
  <div id="app">
    <body>
      <rings></rings>
      <div id="appContent">
        <div id="topbarContainer">
          <topbar></topbar>
        </div>
        <div id="routerContainer">
          <component :is="currentContentIndex" transition="fade" transition-mode="out-in"></component>
        </div>
      </div>
    </body>
  </div>
</template>

<script>
  import Rings from '@/components/rings/Rings';
  import Topbar from '@/components/topbar/Topbar';
  import Game from '@/components/game/Game';
  import Home from '@/components/home/Home';
  import Games from '@/components/games/Games';
  import Shop from '@/components/shop/Shop';
  import Community from '@/components/community/Community';
  import Settings from '@/components/settings/Settings';

  export default {
    name: 'app',
    data: () => ({
    }),
    components: {
      rings: Rings,
      topbar: Topbar,
      game: Game,
      home: Home,
      games: Games,
      shop: Shop,
      community: Community,
      settings: Settings,
    },
    methods: {
    },
    computed: {
      currentContentIndex() {
        return this.$store.state.currentContentIndex;
      },
    },
  };
</script>

<style lang="sass">
  @font-face
    font-family: 'zekton'
    src: url('./assets/zekton.ttf') format('truetype')
    font-weight: normal
    font-style: normal

  $font-stack:    zekton, Helvetica, sans-serif
  $primary-color: white

  body
    margin: 0
    padding: 0
    overflow: hidden
    background-color: #222
    color: $primary-color
    font-family: 'zekton'

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

  .fade-transition
    transition: opacity 0.2s ease

  .fade-enter, .fade-leave
    opacity: 0
</style>
