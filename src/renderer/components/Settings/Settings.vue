<template>
  <div id="settings" class="tabContents">
    <div class="settings-general">
      <h2 class="block-title">{{ $localize('general') }}</h2>
      <div class="divider"></div>
      <div class="settings-list">
        <setting-checkbox
          id="setting-fullscreen"
          setting="fullscreen"
          :title="$localize('fullscreen')"
          @change="fullscreen"
        ></setting-checkbox>
      </div>
    </div>
    <div class="settings-profile">
      <h2 class="block-title">{{ $localize('profile') }}</h2>
      <div class="divider"></div>
    </div>
  </div>
</template>
<script>
  // @ifdef ELECTRON
  import { remote } from 'electron';
  // @endif
  import SettingCheckbox from './SettingCheckbox';
  import SettingDropdown from './SettingDropdown';

  // @ifdef ELECTRON
  const win = remote.getCurrentWindow();
  // @endif

  export default {
    name: 'Settings',
    components: {
      SettingCheckbox,
      SettingDropdown,
    },
    methods: {
      fullscreen(value) {
        console.log(value);
        // @ifdef ELECTRON
        win.setFullScreen(value);
        // @endif
        // @ifdef WEB
        if (value) {
          document.requestFullscreen();
        } else {
          document.cancelFullscreen();
        }
        // @endif
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
