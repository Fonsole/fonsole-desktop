<template>
  <div id="checkbox">
    <label>{{ title }}</label>
    <input type="checkbox" v-model="value">
  </div>
</template>
<script>
// @ifdef ELECTRON
  import { remote } from 'electron';
// @endif
// @ifdef WEB
  import screenfull from 'screenfull';
// @endif
  import SettingCheckbox from './SettingCheckbox';

// @ifdef ELECTRON
  const win = remote.getCurrentWindow();
// @endif

  export default {
    name: 'SettingCheckboxFullscreen',
    extends: SettingCheckbox,
    props: {
      title: {
        type: String,
        default: '',
      },
    },
// @ifdef WEB
    data: () => ({
      isFullscreen: false,
    }),
    computed: {
      value: {
        get() {
          return this.isFullscreen;
        },
        set(value) {
          this.onFullscreenChange(value);
          return true;
        },
      },
    },
// @endif
    mounted() {
// @ifdef WEB
      screenfull.on('change', () => {
        this.isFullscreen = screenfull.isFullscreen;
      });
// @endif
      this.$on('change', this.onFullscreenChange);
    },
    methods: {
      onFullscreenChange(value) {
// @ifdef ELECTRON
        win.setFullScreen(value);
// @endif
// @ifdef WEB
        if (value) {
          screenfull.request();
        } else {
          screenfull.exit();
        }
// @endif
      },
    },
  };
</script>
<style lang="sass" scoped>
  #checkbox
    width: 100%

  label
    font-size: 2.0vh

  input
    float: right
    margin-right: 20px
    width: 1.8vh
    height: 1.8vh
</style>
