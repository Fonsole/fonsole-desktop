<template>
  <div id="settings" class="tabContents">
    <div class="block">
      <h2>{{ $localize('general') }}</h2>
      <div class="divider"></div>
      <checkbox class="row"
        id="fullscreenCheckbox"
        :text="$localize('fullscreen')"
        v-model="isFullscreen"
      ></checkbox>
      <dropdown class="row"
        id="resolutionDropdown"
        :text="$localize('resolution')"
        v-model="resolution"
        :closeAfterClick="true">
        <label slot="toggle">{{ `◂${getCurrentResolution()}▸` }}</label>
        <div
          v-for="(resolution,index) in getAvailableResolutions"
          class="s-dropdown-item"
          @click="changeResolution(resolution)"
          :id="resolution"
          :key="resolution">{{resolution}}</div>
      </dropdown>
    </div>
    <div class="block">
      <h2>{{ $localize('profile') }}</h2>
      <div class="divider"></div>
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
    name: 'Settings',
    components: {
      checkbox: Checkbox,
      dropdown: Dropdown,
    },
    data: () => ({
      isFullscreen: electron.remote.getCurrentWindow().isFullScreen(),
      resolution: electron.remote.getCurrentWindow().getSize(),
      allResolutions: ['1280x720', '1280x800', '1280x1024', '1600x900', '1920x1080'],
    }),
    computed: {
      getAvailableResolutions() {
        return this.allResolutions;
      },
    },
    watch: {
      isFullscreen() {
        electron.remote.getCurrentWindow().setFullScreen(this.isFullscreen);
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
  };
</script>

<style lang="sass" scoped>
  h2
    font-size: 4.5vh
    margin: 0px

  #settings
    background-color: rgba(20,20,20,.8)
    text-transform: uppercase
    padding: 20px

    flex-direction: row
    display: flex

    .block:nth-child(1)
      flex: 0.4 1 auto

    .block:nth-child(2)
      flex: 0.6 1 auto
</style>
