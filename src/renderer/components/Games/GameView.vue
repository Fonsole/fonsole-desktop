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
        <h2 class="headerTitle">GAME INFO</h2>
      </div>
      <fbutton width="22" height="6" :type="getButtonState" @click.native="onButtonClick()">
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
      buttonState: 'install',
      rating: 3,
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
</style>
