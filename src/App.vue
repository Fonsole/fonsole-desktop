<template>
  <div id="app">
    <body>
      <rings></rings>
      <topbar></topbar>
    </body>
  </div>
</template>

<script>
  // const fabric = require('fabric').fabric;
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
    computed: {
    },
  };
</script>

<style lang="scss">
  $font-stack:    Helvetica, sans-serif;
  $primary-color: #333;

  body {
    width: 100%;
    height: 100%;
    margin: 0;
    padding: 0;
    overflow: hidden;
    background-color: #222;
  }
</style>
