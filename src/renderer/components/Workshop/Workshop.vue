<template>
  <div id="workshop" class="tabContents">
    <game-list
      id="game-list"
      @game="setPreviewGame"
      @over="setHoverPreviewGame"
      @out="resetHoverPreviewGame"
    ></game-list>
    <game-preview id="game-preview" :gamePath="selectedGame"></game-preview>
  </div>
</template>

<script>
  import GameList from './GameList';
  import GamePreview from './GamePreview';

  export default {
    name: 'Workshop',
    components: {
      GameList,
      GamePreview,
    },
    data: () => ({
      selectedGame: null,
      lockedSelectedGame: null,
    }),
    methods: {
      setPreviewGame(gamePath) {
        this.lockedSelectedGame = gamePath;
        this.selectedGame = gamePath;
      },
      setHoverPreviewGame(gamePath) {
        this.selectedGame = gamePath;
      },
      resetHoverPreviewGame() {
        this.selectedGame = this.lockedSelectedGame;
      },
    },
  };
</script>

<style lang="sass" scoped>
  #workshop
    display: flex
    width: 100%
    height: 100%
    flex-direction: row
    align-items: stretch

  #game-list
    width: 40vw

  #game-preview
    flex: 1
    margin-left: 12px
</style>
