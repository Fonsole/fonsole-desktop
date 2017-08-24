<template>
  <div id="settings" class="tabContents">
    <div class="settings-general">
      <h2 class="block-title">{{ $localize('general') }}</h2>
      <div class="divider"></div>
      <div class="settings-list">
        <!--<setting-checkbox
          id="fullscreen-checkbox"
          :title="$localize('fullscreen')"
          v-model="isFullscreen"
        ></setting-checkbox>
        <setting-dropdown
          id="resolution-dropdown"
          text="TYPE:"
          :title="$localize('resolution')"
          :options="allResolutions"
          setting="resolution"
          @change="changeResolution"
        ></setting-dropdown>-->
        <setting-dropdown
          id="resolution-dropdown"
          text="RESOLUTION:"
          :title="$localize('resolution')"
          :options="allResolutions"
          setting="resolution"
          @change="changeResolution"
        ></setting-dropdown>
      </div>
    </div>
    <div class="settings-profile">
      <h2 class="block-title">{{ $localize('profile') }}</h2>
      <div class="divider"></div>
    </div>
  </div>
</template>
<script>
  import { stringToResolution } from '=/util';
  // @ifdef ELECTRON
  import { remote } from 'electron';
  // @endif
  import SettingCheckbox from './SettingCheckbox';
  import SettingDropdown from './SettingDropdown';

  const win = remote.getCurrentWindow();

  export default {
    name: 'Settings',
    components: {
      SettingCheckbox,
      SettingDropdown,
    },
    data: () => ({
      isFullscreen: win.isFullScreen(),
      resolution: win.getSize(),
      allResolutions: ['1280x720', '1280x800', '1280x1024', '1600x900', '1920x1080'],
    }),
    watch: {
      isFullscreen() {
        win.setFullScreen(this.isFullscreen);
      },
    },
    methods: {
      changeResolution(newValue) {
        const resolution = stringToResolution(newValue);
        const width = resolution[0];
        const height = resolution[1];
        win.setSize(width, height, true);
      },
    },
  };
</script>

<style lang="sass" scoped>
  #settings
    background-color: rgba(20, 20, 20, 0.8)
    text-transform: uppercase
    padding: 20px

    flex-flow: row
    display: flex

    .settings-general
      flex: 0.4

    .settings-profile
      flex: 0.6

  .block-title
    font-size: 4.5vh
    margin: 0px

  .settings-list
    display: flex
    flex-flow: column
</style>
