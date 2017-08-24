<template>
  <div id="settings" class="tabContents">
    <div class="settings-general">
      <h2 class="block-title">{{ $localize('general') }}</h2>
      <div class="divider"></div>
      <div class="settings-list">
        <setting-checkbox-fullscreen
          id="setting-fullscreen"
          setting="fullscreen"
          :title="$localize('fullscreen')"
        ></setting-checkbox-fullscreen>
      </div>
    </div>
    <div class="settings-profile">
      <h2 class="block-title">{{ $localize('profile') }}</h2>
      <div class="divider"></div>
    </div>
  </div>
</template>
<script>
  import SettingCheckbox from './SettingCheckbox';
  import SettingDropdown from './SettingDropdown';
  import SettingCheckboxFullscreen from './SettingCheckboxFullscreen';

  export default {
    name: 'Settings',
    components: {
      SettingCheckbox,
      SettingDropdown,
      SettingCheckboxFullscreen,
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
