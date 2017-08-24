<template>
  <div class="game-list">
    <div class="header">
      <h2 class="headerTitle">{{ $localize('local_games') }}</h2>
    </div>
    <div class="games">
      <game-list-entry
        :previewData="WORKSHOP_PREVIEW_TYPE.CREATE"
        :selected="lockedPreviewData === WORKSHOP_PREVIEW_TYPE.CREATE"
        @click.native="$emit('update', WORKSHOP_PREVIEW_TYPE.CREATE)"
        @mouseover.native="$emit('over', WORKSHOP_PREVIEW_TYPE.CREATE)"
        @mouseout.native="$emit('out')"
      ></game-list-entry>
      <game-list-entry
        v-for="gamePath in workshopGamePaths"
        :previewData="gamePath"
        :selected="lockedPreviewData === gamePath"
        :key="gamePath"
        @click.native="$emit('update', gamePath)"
        @mouseover.native="$emit('over', gamePath)"
        @mouseout.native="$emit('out')"
      ></game-list-entry>
    </div>
    <div class="create-new-game">
      <div class="create-game-info">
        <ul>
          <li @click="openLink('https://developers.fonsole.com/api/')">DOCS</li>
          <li @click="openLink('https://developers.fonsole.com/community/')">COMMUNITY</li>
          <li @click="openLink('https://stackoverflow.com/questions/tagged/fonsole')">STACK OVERFLOW</li>
        </ul>
      </div>
    </div>
  </div>
</template>

<script>
// @ifdef ELECTRON
  import { shell } from 'electron';
// @endif
  import { mapGetters } from 'vuex';
  import { WORKSHOP_PREVIEW_TYPE } from '=/enums';
  import FButton from '@/components/Generic/FButton';
  import GameListEntry from './GameListEntry';

  export default {
    name: 'GameList',
    components: {
      GameListEntry,
      fbutton: FButton,
    },
    props: {
      lockedPreviewData: {
        type: [String, Number],
        required: true,
      },
    },
    data: () => ({ WORKSHOP_PREVIEW_TYPE }),
    computed: mapGetters(['workshopGamePaths']),
    methods: {
      openLink(url) {
// @ifdef ELECTRON
        shell.openExternal(url);
// @endif
// @ifdef WEB
        window.open(url, '_blank');
// @endif
      },
    },
  };
</script>

<style lang="sass" scoped>
  .game-list
    display: flex
    flex-direction: column

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

  .games
    display: flex
    flex-direction: column
    background-color: rgba(40, 40, 40, 0.4)
    flex: 1
    1 auto
    overflow: scroll
    overflow-x: hidden
    &::-webkit-scrollbar-track
      // -webkit-box-shadow: inset 0 0 6px rgba(0,0,0,0.3)
      background-color: rgba(0,0,0,0.1)

    &::-webkit-scrollbar
      width: 1vw
      padding: 0
      margin: 0
      background-color: rgba(0,0,0,0.0)

    &::-webkit-scrollbar-thumb
      -webkit-box-shadow: inset 0 0 6px rgba(0,0,0,.3)
      background-color: rgba(78, 122, 135, 0.85)

  .game-list-entry
    margin: 0

    &:last-child
      margin-bottom: 0

  .create-new-game
    display: flex
    width: 100%

  .create-game-info
    display: flex
    background-color: rgba(0, 0, 0, 0.8)
    min-height: 10vh
    margin: auto 0
    flex-direction: column
    flex: 1 1 auto
    position: relative

    ul
      list-style-type: none
      margin: 0
      padding: 0
      display: flex
      flex-direction: row
      justify-content: space-evenly
      flex: 1 1 auto

      li
        flex: 1 1 0
        float: left
        vertical-align: middle
        position: relative
        display: flex
        align-items: center
        justify-content: center
        flex-direction: column
        margin-bottom: 0.7vh
        transition: transform 0.05s, box-shadow 0.05s
        color: #f7e8e8
        font-size: 3vh
        text-align: center
        cursor: pointer

        &:nth-child(1)
          background-color: #660000
          box-shadow: 0px 0.7vh 0px #3D0000

        &:nth-child(2)
          background-color: #052C7F
          box-shadow: 0px 0.7vh 0px #031A4C

        &:nth-child(3)
          background-color: #C3661C
          box-shadow: 0px 0.7vh 0px #61330E

        &:hover
          // box-shadow: none
          // margin-bottom: 0

          box-shadow: 0 0 0 #111
          background: radial-gradient(ellipse at center, #232530 0%,#232530 100%)
          transform: rotateX(15deg) translateY(4px)

  .create-game-button
    flex: 1
    margin: 0
</style>
