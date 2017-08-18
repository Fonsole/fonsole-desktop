<template>
  <div id="gameView" class="tabContents">
    <div class="column leftColumn">
      <div class="header">
        <h2 class="headerTitle game">SAY ANYTHING!</h2>
        <star-rating v-model="rating" starSize="4.5" class="rating"></star-rating>
      </div>
      <div class="media">
        <div class="currentFrame">
          <img src="../../assets/preview_background.png">
          <div class="screenshotCaption">
            <h2>best gaem this is not a drill</h2>
          </div>
        </div>
        <div class="screenshots">
          <div v-for="n in 4" :key="n"
          :class="[{'current': currentMedia == n }, 'screenshot']" @click="currentMedia = n">
            <img src="../../assets/preview_background.png">
          </div>
        </div>
      </div>
      <div class="description">
        <div class="text">
          Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do
          eiusmod tempor incididunt ut labore et dolore magna aliqua.
          Ut enim ad minim veniam, quis nostrud exercitation ullamco
          laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure
          dolor in reprehenderit in voluptate velit esse cillum dolore eu
          fugiat nulla pariatur. Excepteur sint occaecat cupidatat non
          proident, sunt in culpa qui officia deserunt mollit anim id est laborum.
        </div>
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
      StarRating,
    },
    data: () => ({
      gameState: 'notInstalled',
      currentMedia: 1,
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
      flex: 0.7 1 auto
      display: flex
      flex-direction: column

      .media
        flex: 0.8 1 auto
        display: flex
        justify-content: space-between
        flex-direction: row

        .currentFrame
          flex: 0.7 1 auto
          overflow: hidden
          margin: 2vh
          box-shadow: 0 0 10px rgba(0,0,0,0.5)
          position: relative

          img
            width: 100%
            height: 100%

          .screenshotCaption
            position: absolute
            bottom: 0
            width: 100%
            background: linear-gradient(to top, rgba(0,0,0,0.8), rgba(0,0,0,0))
            margin: 0

        .screenshots
          flex: 0.3 1 auto
          display: flex
          flex-direction: column
          margin: 2vh
          margin-left: 0

          .screenshot
            margin-bottom: 1.5vh
            width: 25vh
            height: 14vh
            box-shadow: 0 0 10px rgba(0,0,0,0.5)
            transition: all 0.25s ease
            box-sizing: border-box

            img
              width: 100%
              height: 100%
              box-shadow: 0 0 10vh rgba(0,0,0,0.9) inset
              filter: hue-rotate(90deg)

            &.current
              border: 1px dashed white

            &:hover
              opacity: 0.8

      .description
        flex: 0.2 1 auto

        .text
          margin: 2vh
          margin-top: 0

    .rightColumn
      flex: 0.3 1 auto
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
        margin-left: 2vh
        margin-right: 2vh
</style>
