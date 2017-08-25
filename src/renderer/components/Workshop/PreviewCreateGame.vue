<template>
  <div class="preview-create-game">
    <h2 class="header">CREATE NEW GAME</h2>
    <div class="game-path">
      <div :class="['game-path-input-line', pathBorderClass]">
        <input type="text" v-model="gamePath" tabindex="-1">
        <button @click="selectDirectory" tabindex="-1">...</button>
      </div>
      <div v-show="errorReason" class="error-reason">{{ $localize(errorReason) }}</div>
    </div>
  </div>
</template>

<script>
  import { remote } from 'electron';
  import path from 'path';

  export default {
    name: 'PreviewCreateGame',
    data: () => ({
      gamePath: '',
    }),
    computed: {
      pathBorderClass() {
        if (!this.gamePath) return 'is-clear';
        if (this.errorReason) return 'is-error';
        if (this.isValidGameDirectory) return 'is-link';
        return 'is-ok';
      },
      errorReason() {
        if (this.gamePath === '') return '';
        if (!path.isAbsolute(this.gamePath)) return 'absoulte';
        return null;
      },
      isValidGameDirectory() {
        // Path should be valid
        if (this.errorReason) return true;
        return false;
      },
    },
    watch: {
      gamePath() {
        if (this.errorReason === 'null') {
          // todo
        }
      },
    },
    methods: {
      selectDirectory() {
        this.gamePath = remote.dialog.showOpenDialog(remote.getCurrentWindow(), {
          title: 'asd',
          properties: [
            'openDirectory',
            'createDirectory',
            'promptToCreate',
          ],
        })[0];
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
