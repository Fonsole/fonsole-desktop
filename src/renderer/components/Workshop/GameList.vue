<template>
  <div class="game-list">
    <div class="header">
      <h2 class="headerTitle game">{{ $localize('local_games') }}</h2>
    </div>
    <div class="games">
      <game-list-entry
        v-for="gamePath in workshopGamePaths"
        :gamePath="gamePath"
        :key="gamePath"
        @click.native="$emit('game', gamePath)"
        @mouseover.native="$emit('over', gamePath)"
        @mouseout.native="$emit('out')"
      ></game-list-entry>
    </div>
    <div class="create-new-game">
      <div class="create-game-info">
        <a tabindex="-1" @click.prevent="openLink" href="https://developers.fonsole.com/api/">DOCS</a>
        <a tabindex="-1" @click.prevent="openLink" href="https://developers.fonsole.com/community/">COMMUNITY</a>
        <a tabindex="-1" @click.prevent="openLink" href="https://stackoverflow.com/questions/tagged/fonsole">STACK OVERFLOW</a>
      </div>
      <fbutton type="play" class="create-game-button" height="7" @click="createGame">
        {{ $localize('create_game') }}
      </fbutton>
    </div>
  </div>
</template>

<script>
  // @ifdef ELECTRON
  import { shell } from 'electron';
  // @endif
  import { mapGetters } from 'vuex';
  import FButton from '@/components/Generic/FButton';
  import GameListEntry from './GameListEntry';

  export default {
    name: 'GameList',
    components: {
      GameListEntry,
      fbutton: FButton,
    },
    computed: mapGetters(['workshopGamePaths']),
    methods: {
      openLink(event) {
        // event.target contains reference to event emitter, so we can grab href here.
        const url = event.target.href;
        // @ifdef ELECTRON
        shell.openExternal(url);
        // @endif
        // @ifdef WEB
        window.open(url, '_blank');
        // @endif
      },
      createGame() {
        this.$store.commit('setPage', 'createGame');
      },
    },
  };
</script>

<style lang="sass" scoped>
  .game-list
    display: flex
    flex-direction: column

  .header
    background-color: rgba(20, 20, 20, 0.8)

    .headerTitle
      margin: 0.5rem 2vw

  .games
    display: flex
    flex-direction: column
    background-color: rgba(40, 40, 40, 0.4)
    height: 90%

  .game-list-entry
    margin: 10px 0

    &:last-child
      margin-bottom: 0

  .create-new-game
    margin-top: 4px
    display: flex
    width: 100%

  .create-game-info
    display: flex
    padding: 4px
    background-color: rgba(0, 0, 0, 0.8)
    height: 90%
    margin: auto 0
    flex-direction: column

    a
      color: #bcbcbc
      outline: none

  .create-game-button
    flex: 1
    margin: auto 0.2rem
</style>
