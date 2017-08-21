<template>
  <div id="workshop" class="tabContents">
    <game-list
      id="game-list"
      :lockedPreviewData="lockedData"
      @update="setPreviewData"
      @over="setHoverPreviewData"
      @out="resetHoverPreviewData"
    ></game-list>
    <component
      id="preview-container"
      :is="previewComponent"
      :gamePath="currentData"
    ></component>
  </div>
</template>

<script>
  import { WORKSHOP_PREVIEW_TYPE } from '=/enums';
  import GameList from './GameList';
  import PreviewPlaceholder from './PreviewPlaceholder';
  import PreviewCreateGame from './PreviewCreateGame';
  import PreviewGame from './PreviewGame';

  export default {
    name: 'Workshop',
    components: {
      GameList,
      PreviewGame,
      PreviewPlaceholder,
      PreviewCreateGame,
    },
    data: () => ({
      currentData: WORKSHOP_PREVIEW_TYPE.PLACEHOLDER,
      lockedData: WORKSHOP_PREVIEW_TYPE.PLACEHOLDER,
    }),
    computed: {
      previewComponent() {
        switch (this.currentData) {
          case WORKSHOP_PREVIEW_TYPE.PLACEHOLDER:
            return 'PreviewPlaceholder';
          case WORKSHOP_PREVIEW_TYPE.CREATE:
            return 'PreviewCreateGame';
          default:
            return 'PreviewGame';
        }
      },
    },
    methods: {
      setPreviewData(gamePath) {
        this.lockedData = gamePath;
        this.currentData = gamePath;
      },
      setHoverPreviewData(gamePath) {
        this.currentData = gamePath;
      },
      resetHoverPreviewData() {
        this.currentData = this.lockedData;
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

  #preview-container
    flex: 1
    margin-left: 12px
    display: flex
    background-color: rgba(0, 0, 0, 0.6)
</style>
