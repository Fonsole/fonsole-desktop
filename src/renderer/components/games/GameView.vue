<template>
  <div id="gameView" class="tabContents">
    <div class="column leftColumn">
      <div class="header">
        <h2 class="gameTitle">SAY ANYTHING!</h2>
        <div class="rating">
          <div class="stars gray">

          </div>
          <div class="stars lit">

          </div>
        </div>
      </div>
      <div class="frame">

      </div>
      <div class="media">

      </div>
    </div>
    <div class="column rightColumn">
      <fbutton width="22" height="6" :type="getButtonState" @click.native="onButtonClick()">
        {{ $localize(this.buttonState) }}
      </fbutton>
    </div>
  </div>
</template>

<script>
  import FButton from '../generic/FButton';
  // import StarRating from '../generic/StarRating';

  export default {
    name: 'GameView',
    components: {
      fbutton: FButton,
    },
    data: () => ({
      buttonState: 'install',
    }),
    computed: {
      getButtonState() {
        return this.buttonState;
      },
    },
    methods: {
      onButtonClick() {
        console.log('asds');
        this.buttonState = 'progress';
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

    .leftColumn
      flex: 0.75 1 auto
      display: flex
      flex-direction: column

      .header
        flex: 0 1 auto
        clear: both
        background-color: rgba(20,20,20,.8)

        .gameTitle
          float: left
          margin-left: 2vh

        .gameTitle:before
          background-image: url('../../assets/gamepad.png')
          background-size: 4.5vh 4.5vh
          background-repeat: no-repeat
          display: inline-block
          width: 5.7vh
          height: 4.85vh
          vertical-align: middle
          content: ''

        .rating
          float: right
          width: 22.5vh
          height: 5.5vh
          margin: 1vh

          .stars
            width: 100%
            height: 100%
            background-image: url('../../assets/star.png')
            background-size: 4.5vh 4.5vh
            background-position: 50% 50%
            background-repeat: repeat-x

          .gray
            filter: grayscale(85%)

          .lit
            margin-bottom: -3.5vh

    .rightColumn
      flex: 0.25 1 auto
      margin-left: 2vh
</style>
