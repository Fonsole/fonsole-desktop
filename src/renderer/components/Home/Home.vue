<template>
  <div id="home" class="tabContents">
    <div class="column left marginRight">
      <div class="cell marginBottom">
        <div class="cellHeader">
          <h2 class="cellHeaderTitle">{{ $localize('featured') }}</h2>
        </div>
        <div class="cellContent featured">

        </div>
        <div class="cellCaption">
          <h2>say anything!</h2>
        </div>
      </div>
      <div class="cell marginTop">
        <div class="cellHeader">
          <h2 class="cellHeaderTitle">{{ $localize('promo') }}</h2>
          <div v-for="n in 3" :key="n" class="slideSelector">

          </div>
        </div>
        <div class="cellContent shop">

        </div>
        <div class="cellCaption">
          <h2>save 50% off this!</h2>
        </div>
      </div>
    </div>
    <div class="column middle marginRight marginLeft">
      <div class="cell marginBottom">
        <div class="cellHeader">
          <h2 class="cellHeaderTitle">{{ $localize('quick') }}</h2>
        </div>
        <div class="cellContent quickPlay">
          <div class="quickPlayButton lastPlayed clickable">
            <div class="cellCaption">
              <h2>
                {{ $localize('last_played') }}
              </h2>
            </div>
          </div>
          <div class="quickPlayButton random clickable">
            <div class="cellCaption">
              <h2>
                {{ $localize('random') }}
              </h2>
            </div>
          </div>
        </div>
      </div>
      <div class="cell marginTop">
        <div class="cellHeader">
          <h2 class="cellHeaderTitle">{{ $localize('tutorials') }}</h2>
        </div>
        <div class="cellContent tutorials">

        </div>
        <div class="cellCaption">
          <h2>creating your first fonsole game pt. 1</h2>
        </div>
      </div>
    </div>
    <div class="column right marginLeft">
      <div class="verticalCell">
        <div class="cellHeader">
          <h2 class="cellHeaderTitle">{{ $localize('news') }}</h2>
        </div>
        <div class="newsContainer">
          <div v-for="(entry,n) in news" :key="n" class="entry">
            <h2>{{ entry.title }}</h2>
            <div class="date">{{ entry.date }}</div>
            <div class="text">
              {{ entry.text }}
            </div>
            <div class="newsDivider"></div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
  import FButton from '@/components/Generic/FButton';

  export default {
    name: 'Home',
    components: {
      fbutton: FButton,
    },
    data: () => ({
      news: [{
        title: 'we rollback\'d something',
        text: 'Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.',
        date: '15.08.2017',
      }, {
        title: 'we did something',
        text: 'Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.',
        date: '10.08.2017',
      }, {
        title: 'we did something',
        text: 'Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.',
        date: '10.08.2017',
      }, {
        title: 'we did something',
        text: 'Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.',
        date: '10.08.2017',
      }],
    }),
  };
</script>

<style lang="sass" scoped>
  h2
    font-size: 2.5vw
    margin: 1vh

  .marginLeft
    margin-left: 1vh

  .marginRight
    margin-right: 1vh

  .marginTop
    margin-top: 1vh

  .marginBottom
    margin-bottom: 1vh

  #home
    display: flex
    width: 100%
    height: 100%
    // background-color: rgba(255,20,20,0.5)
    box-shadow: none
    justify-content: space-between

    .clickable
      transition: all 0.25s ease
      &:hover
        opacity: 0.6

      &:active
        filter: brightness(50%)

    .cellCaption
      position: absolute
      bottom: 0
      width: 100%
      background: linear-gradient(to top, rgba(0,0,0,0.8), rgba(0,0,0,0))
      margin: 0

    .column
      flex: 1 1 auto
      display: flex
      flex-direction: column

      &.left
        min-width: 30vw

      &.middle
        min-width: 30vw

      &.right
        max-width: 25vw

      .cell, .verticalCell
        flex: 1 1 auto
        background-color: rgba(20,20,20,0.8)
        box-shadow: 0 0 1vh rgba(0,0,0,0.5)
        overflow: hidden
        display: flex
        flex-direction: column
        position: relative

        .cellHeader
          flex: 0 1 auto
          display: flex
          flex-direction: row
          height: 7.5vh
          min-height: 7.5vh
          clear: both
          background-color: rgba(20,20,20,.8)

          .cellHeaderTitle
            font-size: 4.0vh
            margin-left: 2vh

          .slideSelector
            width: 2vh
            height: 2vh
            background: #c7c746
            border-radius: 1vh
            margin-top: 2.9vh
            margin-left: 1vh

        .cellContent
          max-width: 100%
          max-height: 100%
          display: flex
          flex: 1 1 auto
          box-shadow: 0 0 5vh rgba(0,0,0,0.9) inset
          position: relative
          background-position: center
          background-size: cover
          background-repeat: no-repeat

          &.tutorials
            background-image: url('~@/assets/testgame4.jpg')
          &.shop
            background-image: url('~@/assets/testgame3.jpg')
          &.featured
            background-image: url('~@/assets/testgame2.jpg')
          &.quickPlay
            .quickPlayButton
              background-color: rgba(0,0,0,.3)
              flex: 1 1 auto
              position: relative
              box-shadow: 0 0 5vh rgba(0,0,0,0.9) inset
              position: relative

              &.random
                flex: 0.75 0 auto
                background-image: url('~@/assets/preview_quickplay.png')
                background-position: center
                background-size: cover
                background-repeat: no-repeat
                text-align: center

              &.lastPlayed
                background-image: url('~@/assets/testgame.jpg')
                background-position: center
                background-size: cover
                background-repeat: no-repeat

      .verticalCell
        display: flex
        flex-direction: column

        .newsContainer
          flex: 0 1 auto
          overflow: scroll
          overflow-x: hidden

          &::-webkit-scrollbar-track
            // -webkit-box-shadow: inset 0 0 6px rgba(0,0,0,0.3)
            background-color: rgba(0,0,0,0.1)

          &::-webkit-scrollbar
            width: 12px
            background-color: rgba(0,0,0,0.0)

          &::-webkit-scrollbar-thumb
            -webkit-box-shadow: inset 0 0 6px rgba(0,0,0,.3)
            background-color: rgba(78, 122, 135, 0.85)

          .entry
            .newsDivider
              $dividerColor: rgba(78, 122, 135, 0.85)
              $dividerColorTransparent: rgba(78, 122, 135, 0)

              margin-top: 1vh
              // margin-bottom: 1vw
              // margin-left: 1vw
              width: 100%
              height: 0.10vh
              background: linear-gradient(to right,
              $dividerColorTransparent 0%,
              $dividerColorTransparent 10%,
              $dividerColor 30%,
              $dividerColor 70%,
              $dividerColorTransparent 90%,
              $dividerColorTransparent 100%)

            h2, div
              margin: 1vh

            h2
              font-size: 2.5vh
              color: #73ABBC

            .date
              margin-top: 0
              margin-bottom: 0

            .text
              font-size: 1.5vh
              opacity: 0.8
</style>
