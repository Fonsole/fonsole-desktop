<template>
  <div class="game-preview" v-if="gameName !== ''">
    <div class="game-title">
      {{ gameName }}
    </div>
    <div class="key-value-pair game-row-working-directory">
      <div>WORKING DIRECTORY:</div>
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
      class="open-game-workspace"
      height="8"
      width="40"
      type="play"
      :disabled="!gamePath"
      @click="openWorkspace"
    >
      {{ $localize('open') }}
    </fbutton>

    <fbutton
      class="delete-game"
      height="8"
      width="40"
      type="uninstall"
      :disabled="!gamePath"
      @click="openWorkspace"
    >
      {{ $localize('delete') }}
    </fbutton>
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

  .placeholder
    display: flex
    background-color: rgba(0, 0, 0, 0.4)
    align-items: center
    justify-content: center
    flex-direction: column

  .game-preview
    display: flex
    background-color: rgba(0, 0, 0, 0.4)
    flex-direction: column

  .game-title
    align-self: center
    font-size: 3vw
    font-weight: 900
    height: 3.5vw

  .open-game-workspace
    align-self: flex-end
    // Temporary !important use. Should be removed as well as id on reusable component
    margin-top: auto !important
    margin-bottom: 2vh !important

  .key-value-pair
    width: 100%
    display: flex
    justify-content: space-between

    // Both
    > *
      font-size: 2vw

    // Key
    > *:first-child
      margin-left: 12px

    // Value
    > *:last-child
      margin-right: 12px
      color: #dfdfdf
      // A fallback used to make element stick to right side if it goes multiline
      margin-left: auto
</style>
