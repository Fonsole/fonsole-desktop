<template>
  <div class="game-preview" v-if="gameName !== ''">
    <div class="left">
      <div class="nameAndDescription">
        <h2>{{ $localize('game_name') }}</h2>
        <div class="divider"></div>
        <textarea maxlength="32" rows="1"
        :placeholder="$localize('game_name_placeholder')" class="name">
        </textarea>
        <h2>{{ $localize('game_description') }}</h2>
        <div class="divider"></div>
        <textarea rows="8" :placeholder="$localize('game_description_placeholder')"
        class="description">
        </textarea>
      </div>
    </div>
    <div class="right">
      <div class="previewImage">
        <img src="~@/assets/testgame2.jpg">
      </div>
      <div class="key-value-pair game-row-working-directory">
        <div>DIRECTORY:</div>
        <div>{{ gamePath }}</div>
      </div>
      <div class="key-value-pair game-row-change-date">
        <div>CHANGE DATE:</div>
        <div>{{ gameName && new Date() }}</div>
      </div>
      <div class="key-value-pair game-row-creation-date">
        <div>CREATION DATE:</div>
        <div>{{ gameName && new Date() }}</div>
      </div>
      <fbutton
        class="button open-game-workspace"
        height="8"
        width="auto"
        type="play"
        :disabled="!gamePath"
        @click="openWorkspace"
      >
        {{ $localize('open') }}
      </fbutton>
      <fbutton
        class="button publish-game"
        height="8"
        width="auto"
        type="publish"
        :disabled="!gamePath"
        @click="openWorkspace"
      >
        {{ $localize('publish') }}
      </fbutton>
      <fbutton
        class="button delete-game"
        height="8"
        width="auto"
        type="delete"
        :disabled="!gamePath"
        @click="openWorkspace"
      >
        {{ $localize('delete') }}
      </fbutton>
    </div>
  </div>
  <div class="placeholder" v-else-if="gameName === ''">
    <a class="logoContainer">
      <div class="logo animateGears">
        <div class="logoInner">
          <div class="logoGear small"></div>
          <div class="logoGear large"></div>
        </div>
      </div>
    </a>
    <h2>{{ $localize('select') }}</h2>
  </div>
</template>

<script>
  import FButton from '@/components/Generic/FButton';

  export default {
    name: 'GamePreview',
    components: {
      fbutton: FButton,
    },
    props: {
      gamePath: String,
    },
    computed: {
      gameInfo() {
        return (this.gamePath && this.$store.getters.workshopGameInfo(this.gamePath)) || {};
      },
      gameName() {
        return this.gameInfo.name || '';
      },
    },
    methods: {
      openWorkspace() {
        this.$store.commit('setWorkspaceGame', this.gamePath);
        this.$store.commit('setPage', 'workspace');
      },
    },
  };
</script>

<style lang="sass" scoped>
  @import "~@/assets/styles/global.sass"

  h2
    font-size: 4.5vh
    margin: 0

  .placeholder
    display: flex
    background-color: rgba(0, 0, 0, 0.6)
    align-items: center
    justify-content: center
    flex-direction: column

  .game-preview
    display: flex
    background-color: rgba(0, 0, 0, 0.6)
    flex-direction: row

    .left
      flex: 1 1 0
      display: flex
      flex-direction: column
      margin: 2vh

      .nameAndDescription
        flex: 1 1 auto

        textarea
          resize: none
          width: 100%
          background: none
          outline: none
          font-family: zekton
          text-transform: uppercase
          font-size: 3.5vh
          border: none
          padding: 1px
          margin-bottom: 1vh
          color: white
          overflow: hidden

          &.description
            font-size: 2.5vh
            text-transform: none

          &::placeholder
            color: #6B90B5

          &:focus
            background-color: rgba(0, 0, 0, 0.4)
            border: solid 1px rgba(0, 0, 0, 0.4)
            border-radius: 1vh
            outline: none
            padding: 0

            &::placeholder
              // opacity: 0.0
    .right
      flex: 1 1 0
      display: flex
      flex-direction: column
      padding: 0.75vw
      max-width: 22.5vw
      background-color: rgba(0, 0, 0, 0.6)

      .previewImage
        flex: 0 1 auto
        margin-bottom: -0.25vh

        img
          width: 100%
          box-shadow: 0 0 0 1px #3c3c3c

  .game-title
    align-self: center
    font-size: 3vw
    font-weight: 900
    height: 3.5vw

  .button
    align-self: flex-end
    margin-bottom: 2vh !important

  .open-game-workspace
    margin-top: auto !important

  .key-value-pair
    width: 100%
    display: flex
    justify-content: space-between
    background-color: rgba(0, 0, 0, 0.6)
    box-shadow: 0 0 0 1px #161616

    // Both
    > *
      font-size: 1vw

    // Key
    > *:first-child
      margin-left: 0.5vw

    // Value
    > *:last-child
      margin-right: 0.5vw
      color: #dfdfdf
      margin-left: auto
</style>
