<template>
  <div id="settings" class="tabContents">
    <div class="block">
      <h2>{{ $localize('general') }}</h2>
      <div class="divider"/>
      <checkbox id="fullscreenCheckbox" v-bind:text="$localize('fullscreen')" v-model="isFullscreen"/>
      <dropdown id="resolutionDropdown" v-bind:text="$localize('resolution')" v-model="resolution" :closeAfterClick="true">
        <button slot="toggle">{{ `◂${getCurrentResolution()}▸` }}</button>
        <div v-for="(resolution,index) in getAvailableResolutions" class="s-dropdown-item" @click="changeResolution(resolution)" :id="resolution">{{resolution}}</div>
      </dropdown>
    </div>
    <div class="block">
      <h2>{{ $localize('profile') }}</h2>
      <div class="divider"/>
    </div>
  </div>
</template>
<script>
  import Checkbox from './Checkbox';
  import Dropdown from './Dropdown';

  let electron;

  if (!process.env.IS_WEB) {
    electron = require('electron');
  }

  export default {
    name: 'settings',
    components: {
      checkbox: Checkbox,
      dropdown: Dropdown,
    },
    computed: {
      getAvailableResolutions() {
        return this.allResolutions;
      },
    },
    methods: {
      stringToResolution(s) {
        return s.split('x').map(Number);
      },
      resolutionToString(s) {
        return `${s[0]}x${s[1]}`;
      },
      getCurrentResolution() {
        return this.resolutionToString(electron.remote.getCurrentWindow().getSize());
      },
      changeResolution(r) {
        const width = this.stringToResolution(r)[0];
        const height = this.stringToResolution(r)[1];

        electron.remote.getCurrentWindow().setSize(width, height, true);

        this.resolution = this.getCurrentResolution();
      },
    },
    data: () => ({
      isFullscreen: electron.remote.getCurrentWindow().isFullScreen(),
      resolution: electron.remote.getCurrentWindow().getSize(),
      allResolutions: ['1280x720', '1280x800', '1280x1024', '1920x1080'],
    }),
    watch: {
      isFullscreen() {
        electron.remote.getCurrentWindow().setFullScreen(this.isFullscreen);
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
