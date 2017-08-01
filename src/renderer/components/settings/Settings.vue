<template>
  <div id="settings" class="tabContents">
    <div class="block">
      <h2>{{ $localize('general') }}</h2>
      <div class="divider"/>
      <checkbox id="fullscreenCheckbox" v-bind:text="$localize('fullscreen')" v-model="isFullscreen"/>
    </div>
    <div class="block">
      <h2>{{ $localize('profile') }}</h2>
      <div class="divider"/>
    </div>
  </div>
</template>
<script>
  import Checkbox from './Checkbox';

  const { remote } = require('electron');

  export default {
    name: 'settings',
    components: {
      checkbox: Checkbox,
    },
    data: () => ({
      isFullscreen: remote.getCurrentWindow().isFullScreen(),
    }),
    watch: {
      isFullscreen() {
        remote.getCurrentWindow().setFullScreen(this.isFullscreen);
      },
    },
  };
</script>

<style lang="sass" scoped>
  $dividerColor: rgba(255,255,255,0.65)
  $dividerColorTransparent: rgba(255,255,255,0)

  h1, h2
    margin: 0px

  #settings
    background-color: rgba(20,20,20,.8)
    text-transform: uppercase
    padding: 20px

    .block
      min-width: 400px
      float: left

  .divider
    margin-top: 6px
    margin-bottom: 6px
    margin-left: 1px
    width: 100%
    height: 1px
    background: linear-gradient(left, $dividerColor 0%,$dividerColor 5%,$dividerColor 50%,$dividerColorTransparent 95%,$dividerColorTransparent 100%)
</style>
