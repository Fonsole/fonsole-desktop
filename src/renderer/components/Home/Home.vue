<template>
  <div id="home" class="tabContents">
    <div class="column">
      <div class="cell">
        <div class="cellHeader">
          <h2 class="cellHeaderTitle">{{ $localize('featured') }}</h2>
        </div>
        <div class="cellContent">
          <h2>say anything!</h2>
        </div>
      </div>
      <div class="cell">
        <div class="cellHeader">
          <h2 class="cellHeaderTitle">{{ $localize('promo') }}</h2>
        </div>
        <div class="cellContent">
          <h2>save 50% off this!</h2>
        </div>
      </div>
    </div>
    <div class="column">
      <div class="cell">
        <div class="cellHeader">
          <h2 class="cellHeaderTitle">{{ $localize('quick') }}</h2>
        </div>
        <div class="cellContent quickPlay">
          <fbutton width="22" height="6" type="install">install</fbutton>
          <fbutton width="22" height="6" type="uninstall">uninstall</fbutton>
        </div>
      </div>
      <div class="cell">
        <div class="cellHeader">
          <h2 class="cellHeaderTitle">{{ $localize('tutorials') }}</h2>
        </div>
        <div class="cellContent">
          <h2>creating your first fonsole game</h2>
        </div>
      </div>
    </div>
    <div class="column restrict">
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
    font-size: 4.5vh
    margin: 1vh

  #home
    display: flex
    width: 100%
    height: 100%
    // background-color: rgba(255,20,20,0.5)

    .column
      flex: 1 1 auto
      display: flex
      flex-direction: column

      &.restrict
        max-width: 25vw

      .cell, .verticalCell
        flex: 1 1 auto
        margin: 2vh
        background-color: rgba(20,20,20,0.8)
        box-shadow: 0 0 1vh rgba(0,0,0,0.5)
        overflow: hidden
        display: flex
        flex-direction: column

        .cellHeader
          flex: 0 1 auto
          display: flex
          flex-direction: row
          justify-content: space-between
          height: 7.5vh
          min-height: 7.5vh
          clear: both
          background-color: rgba(20,20,20,.8)

          .cellHeaderTitle
            font-size: 4.0vh
            flex: 0.5 1 auto
            margin-left: 2vh

        .cellContent
          max-width: 100%
          max-height: 100%
          display: flex

          &.quickPlay
            flex: 1 1 auto
            background: url('~@/assets/preview_quickplay.png') center
            background-size: cover
            background-repeat: no-repeat
            box-shadow: 0 0 5vh rgba(0,0,0,0.9) inset

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
