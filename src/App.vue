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
  import Topbar from './components/topbar/Topbar';
  import Rings from './components/rings/Rings';

  export default {
    name: 'app',
    data() {
      return {
      };
    },
    components: {
      Rings,
      Topbar,
    },
    methods: {
      random(m, n) {
        const mi = parseInt(m, 10);
        const ni = parseInt(n, 10);
        return Math.floor(Math.random() * ((ni - mi) + 1)) + mi;
      },
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
