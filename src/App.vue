<template>
  <div id="app">
    <body>
      <rings></rings>
      <div id="router-container">
        <router-view></router-view>
      </div>
      <topbar></topbar>
    </body>
  </div>
</template>

<script>
  import Rings from './components/rings/Rings';
  import Topbar from './components/topbar/Topbar';

  export default {
    name: 'app',
    data: () => ({
    }),
    components: {
      rings: Rings,
      topbar: Topbar,
    },
    methods: {
    },
    computed: {
    },
  };
</script>

<style lang="sass">
  $font-stack:    Helvetica, sans-serif;
  $primary-color: #333;

  body
    width: 100%
    height: 100%
    margin: 0
    padding: 0
    overflow: hidden
    background-color: #222
  #router-container
    width: 100%
    height: 100%
    position: absolute
    top: 53px
</style>
