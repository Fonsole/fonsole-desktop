<template>
  <div class="preview-create-game">
    <div class="header">
      <h2 class="headerTitle">{{ $localize('create_new_game') }}</h2>
    </div>
    <h2>{{ $localize('game_name') }}</h2>
    <div class="name">
      <textarea maxlength="32" rows="1"
      :placeholder="$localize('game_name_placeholder')" class="name">
      </textarea>
    </div>
    <h2>{{ $localize('working_directory') }}</h2>
    <div class="game-path">
      <div :class="['game-path-input-line', pathBorderClass]">
        <input
          type="text"
          :value="gamePath"
          @input="gamePath = normalizePath($event.target.value)"
          tabindex="-1"
        >
        <button @click="selectDirectory" tabindex="-1">...</button>
      </div>
      <div
        :style="{opacity: errorReason ? 1 : 0}"
        class="error-reason"
      >{{ $localize(errorReason) }}</div>
    </div>
    <div
      v-if="isLinkableGame"
      class="link-container"
    >
      <div class="link-text">
        Specified path already contains valid Fonsole game.
        You can add this game to your list.
      </div>
      <fbutton
        type="play"
        width="80"
        height="10"
        @click="linkGame"
      >LINK GAME</fbutton>
    </div>
    <div v-else>
      <h2>{{ $localize('generator_options') }}</h2>
      <div class="generator-options">
        <checkbox
          id="setting-placeholders"
          class="checkbox"
          :title="$localize('placeholders')"
        ></checkbox>
        <checkbox
          id="setting-assets"
          class="checkbox"
          :title="$localize('assets')"
        ></checkbox>
        <checkbox
          id="setting-controller"
          class="checkbox"
          :title="$localize('controller')"
        ></checkbox>
        <checkbox
          id="setting-framework"
          class="checkbox"
          v-model="showFrameworks"
          :title="$localize('framework')"
        ></checkbox>
        <transition name="fade">
          <div v-show="showFrameworks" class="generator-options-frameworks">
            <checkbox
              id="setting-controller"
              class="checkbox"
              title="crafty.js"
            ></checkbox>
            <checkbox
              id="setting-framework"
              class="checkbox"
              title="phaser"
            ></checkbox>
          </div>
        </transition>
      </div>
    </div>
    <fbutton
      class="create-game"
      height="10"
      width="auto"
      type="publish"
      :disabled="!gamePath"
      @click="createGame"
    >
      {{ $localize('create') }}
    </fbutton>
  </div>
</template>

<script>
  import { remote } from 'electron';
  import path from 'path';
  import FButton from '@/components/Generic/FButton';
  import Checkbox from '@/components/Generic/Checkbox';

  const fs = remote.require('fs-extra');

  export default {
    name: 'PreviewCreateGame',
    components: {
      fbutton: FButton,
      checkbox: Checkbox,
    },
    data: () => ({
      gamePath: '',
      showFrameworks: false,
    }),
    computed: {
      pathBorderClass() {
        if (!this.gamePath) return 'is-clear';
        if (this.errorReason) return 'is-error';
        if (this.isLinkableGame) return 'is-link';
        return 'is-ok';
      },
      isLinkableGame() {
        // Path should be valid
        if (!this.errorReason) {
          // Directory should exist
          if (fs.existsSync(this.gamePath)) {
            const gameIndexPath = path.join(this.gamePath, '.fonsole.yml');
            // Directory should contain .fonsole.yml file
            if (fs.existsSync(gameIndexPath)) {
              return true;
            }
          }
        }
        // If some of conditions fails return false
        return false;
      },
    },
    asyncComputed: {
      errorReason() {
        // A hack to keep correct scope
        // This bug is caused by something in Babel chain, so we should remove it once fixed
        return (async () => {
          // We don't need to show error for empty path
          if (this.gamePath === '') return '';

          // Path should be absolute
          if (!path.isAbsolute(this.gamePath) ||
            path.normalize(this.gamePath) !== this.gamePath) return 'absolute';

          // If directory not exists we don't need to check it's files
          if (!fs.existsSync(this.gamePath)) return null;

          // Readdir throws error when we try to read file, so provide other error before it
          if ((await fs.lstat(this.gamePath)).isFile()) return 'file_exists';

          try {
            // Synchronously read all files from directory
            const files = await fs.readdir(this.gamePath);
            // Directory is empty, treat it as non-existing directory
            if (files.length === 0) return null;
            // Directory contains game index file, so it should be considered as a valid game.
            if (files.includes('.fonsole.yml')) return null;
            return 'directory_not_empty';
          } catch (err) {
            console.error(err);
            return err.message;
          }
        })();
      },
    },
    methods: {
      selectDirectory() {
        const paths = remote.dialog.showOpenDialog(remote.getCurrentWindow(), {
          title: this.$localize('open_game_directory'),
          properties: [
            'openDirectory',
            'createDirectory',
            'promptToCreate',
          ],
        });
        // If nothing was selected, dialog returns empty array, so ignore this case
        if (paths.length === 1) this.gamePath = this.normalizePath(paths[0]);
      },
      normalizePath(gamePath) {
        // Get path separator, used on platform other than current
        const otherSep = path.sep === path.posix.sep ? path.win32.sep : path.posix.sep;
        // Replace wrong path separators
        // We also can reach same effect by using path.normalize,
        // but it also resolves '..' and '.' segments, which we don't need
        return gamePath.replace(otherSep, path.sep);
      },
      linkGame() {
        this.$store.dispatch('addGame', this.gamePath);
      },
      createGame() {

      },
    },
  };
</script>

<style lang="sass" scoped>
  h2
    margin-left: 2vw
    margin-bottom: 1vh
    font-size: 3vh

  .checkbox
    margin-left: 3vw
    margin-right: 2vw
    margin-bottom: 0.25vw

    color: #B7B7B7

  .generator-options-frameworks
    margin-left: 2vw

  textarea
    resize: none
    background: none
    outline: none
    font-family: zekton
    text-transform: uppercase
    font-size: 2.5vh
    border: none
    color: white
    overflow: hidden
    margin: 1vh 2vw
    display: flex
    flex-flow: column

    background-color: rgba(0, 0, 0, 0.4)
    border: solid 1px rgba(0, 0, 0, 0.4)
    border-radius: 0
    outline: none
    padding: 0

    &::placeholder
      color: #6B90B5
      padding-left: 3px

    &::content
      padding-left: 3px

  .preview-create-game
    display: flex
    flex-flow: column

  .header
    flex: 0 1 auto
    display: flex
    flex-direction: row
    height: 7.5vh
    min-height: 7.5vh
    clear: both
    background-color: rgba(20,20,20,.8)

    .headerTitle
      font-size: 4.0vh
      margin: auto 0
      margin-left: 2vh

  .game-path
    margin: 12px 2vw
    display: flex
    flex-flow: column

    .error-reason
      height: 20px
      margin: 5px
      align-self: flex-end
      font-size: 20px
      color: #ff5555
      font-weight: bold
      display: none

  .game-path-input-line
    display: flex
    flex-flow: row
    $height: 3.2vh
    height: $height

    $border-color-error: red
    $border-color-clear: #000
    $border-color-link: yellow
    $border-color-ok: lime

    > input
      height: 100%
      padding: 2px
      flex: 1
      font-size: $height / 1.3
      box-sizing: border-box
      outline: none
      color: white
      font-family: zekton

      background-color: rgba(0, 0, 0, 0.4)
      border: solid 1px rgba(0, 0, 0, 0.4)
      border-radius: 0
      outline: none
      padding: 0

    > button
      width: $height
      height: 100%
      transition: background 0.2s ease-in
      outline: none
      color: white
      background-color: rgba(0, 0, 0, 0.4)
      border: solid 1px rgba(0, 0, 0, 0.4)
      border-radius: 0
      outline: none
      padding: 0
      border-left: none

      &:hover
        background: rgba(#666, 0.2)

    &.is-error > *
      border-color: $border-color-error

    &.is-link > *
      border-color: $border-color-link

    &.is-ok > *
      border-color: $border-color-ok

  .link-container
    display: flex
    flex-flow: column
    align-items: center

  .link-text
    font-size: 2vw
    text-align: center
    white-space: pre-line
    margin-bottom: 2vh

  .generator-options
    display: flex
    flex-flow: column

  .create-game
    margin-top: auto
    margin-bottom: 0.5vh

  .fade-enter-active, .fade-leave-active
    transition: all .5s

  .fade-enter, .fade-leave-to
    height: 0
    opacity: 0
</style>
