<template>
  <div class="preview-create-game">
    <h2 class="header">CREATE NEW GAME</h2>
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
    <div v-if="isLinkableGame">
      Link game
    </div>
    <div v-else>
      Generator options
    </div>
  </div>
</template>

<script>
  import { remote } from 'electron';
  import path from 'path';

  const fs = remote.require('fs-extra');

  export default {
    name: 'PreviewCreateGame',
    data: () => ({
      gamePath: '',
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
    },
  };
</script>

<style lang="sass" scoped>
  .preview-create-game
    display: flex
    flex-flow: column

  .header
    align-self: center
    font-size: 9vh
    margin: 1vh 0

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

  .game-path-input-line
    display: flex
    flex-flow: row
    $height: 24px * 1.3
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
      border: 2px solid $border-color-clear
      border-right: none
      border-radius: 5px 0 0 5px
      outline: none
      background: rgba(#000, 0.2)
      color: white

    > button
      width: $height
      height: 100%
      background: rgba(#000, 0.2)
      border: 2px solid $border-color-clear
      border-left: none
      border-radius: 0 5px 5px 0
      transition: background 0.2s ease-in
      outline: none
      color: white

      &:hover
        background: rgba(#666, 0.2)

    &.is-error > *
      border-color: $border-color-error

    &.is-link > *
      border-color: $border-color-link

    &.is-ok > *
      border-color: $border-color-ok

</style>
