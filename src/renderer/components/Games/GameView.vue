<template>
  <div id="gameView" class="tabContents">
    <div class="column leftColumn">
      <div class="header">
        <h2 class="headerTitle game">SAY ANYTHING!</h2>
        <star-rating v-model="rating" starSize="4.5" class="rating"></star-rating>
      </div>
      <div class="frame">

      </div>
      <div class="media">

      </div>
    </div>
    <div class="column rightColumn">
      <div class="header">
        <h2 class="headerTitle">INFO</h2>
      </div>
      <div class="infoIcon">
        <img src="../../assets/players.png">
        <h2 class="playerCount">3-6</h2>
      </div>
      <div class="infoIcon">
        <img src="../../assets/time.png">
        <h2 class="time">15-20′</h2>
      </div>
      <div class="infoIcon">
        <img src="../../assets/wifi.png">
        <h2 class="wifi">300mb</h2>
      </div>
      <fbutton class="button" width="30" height="8" :disabled="this.gameState !== 'installed'"
      type="play" @click.native="onPlayButtonClick()">
        {{ $localize('play') }}
      </fbutton>
      <fbutton class="button" width="30" height="6"
      :type="buttonState" @click.native="onStatusButtonClick()">
        {{ $localize(this.buttonState) }}
      </fbutton>
    </div>
  </div>
</template>

<script>
  import FButton from '../Generic/FButton';
  import StarRating from '../Generic/StarRating';

  export default {
    name: 'GameView',
    components: {
      fbutton: FButton,
      'star-rating': StarRating,
    },
    data: () => ({
      gameState: 'notInstalled',
      rating: 3,
    }),
    computed: {
      buttonState() {
        switch (this.gameState) {
          case 'notInstalled':
            return 'install';
          case 'installing':
            return 'progress';
          case 'uninstalling':
            return 'progress-dark';
          case 'installed':
            return 'uninstall';
          default:
            return 'install';
        }
      },
      isInstalled() {
        return this.gameState === 'installed';
      },
    },
    methods: {
      onPlayButtonClick() {
        console.log('play');
      },
      onStatusButtonClick() {
        switch (this.gameState) {
          case 'notInstalled':
            this.gameState = 'installing';
            window.setTimeout(() => {
              this.gameState = 'installed';
            }, 1000);
            break;
          case 'installing':
            break;
          case 'uninstalling':
            break;
          case 'installed':
            this.gameState = 'uninstalling';
            window.setTimeout(() => {
              this.gameState = 'notInstalled';
            }, 1000);
            break;
          default:
        }
      },
    },
  };
</script>

<style lang="sass" scoped>
  h2
    font-size: 4.5vh
    margin: 1vh

  #gameView
    display: flex
    flex-direction: row
    width: 100%
    height: 100%
    // background-image: url('../../assets/preview_background.png')
    // background-position: center center
    // background-repeat: no-repeat
    // box-shadow: inset 0px 0px 20vh 3vh rgba(0, 0, 0, 0.8)

    .column
      background-color: rgba(20,20,20,.8)

    .header
      flex: 0 1 auto
      display: flex
      flex-direction: row
      justify-content: space-between
      height: 8.0vh
      clear: both
      background-color: rgba(20,20,20,.8)

      .headerTitle
        flex: 0.5 1 auto
        margin-left: 2vh

      .headerTitle.game:before
        background-image: url('../../assets/gamepad.png')
        background-size: 4.5vh 4.5vh
        background-repeat: no-repeat
        display: inline-block
        width: 5.7vh
        height: 4.85vh
        vertical-align: middle
        content: ''

      .rating
        margin-right: 2vh

    .leftColumn
      flex: 0.75 1 auto
      display: flex
      flex-direction: column

    .rightColumn
      flex: 0.25 1 auto
      margin-left: 2vh

      .infoIcon
        margin-top: 5vh
        text-align: center
        img
          height: 8.5vh
          width: 8.5vh

        h2
          margin: 0
          font-size: 5vh

      .button
        margin: 2vh auto
</style>
