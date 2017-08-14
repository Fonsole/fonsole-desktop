<template>
  <div id="home" class="tabContents">
    <div class="column">
      <div class="cell">
        <div class="cellHeader">
          <h2 class="cellHeaderTitle">{{ $localize('featured') }}</h2>
        </div>
      </div>
      <div class="cell">
        <div class="cellHeader">
          <h2 class="cellHeaderTitle">{{ $localize('promo') }}</h2>
        </div>
      </div>
    </div>
    <div class="column">
      <div class="cell">
        <div class="cellHeader">
          <h2 class="cellHeaderTitle">{{ $localize('quick') }}</h2>
        </div>
      </div>
      <div class="cell">
        <div class="cellHeader">
          <h2 class="cellHeaderTitle">{{ $localize('tutorials') }}</h2>
        </div>
      </div>
    </div>
    <div class="column">
      <div class="verticalCell">
        <div class="cellHeader">
          <h2 class="cellHeaderTitle">{{ $localize('news') }}</h2>
        </div>
        <div class="newsContainer">

        </div>
      </div>
    </div>
  </div>
</template>

<script>
  export default {
    name: 'Home',
    data: () => ({

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

      .cell, .verticalCell
        flex: 1 1 auto
        margin: 2vh
        background-color: rgba(20,20,20,0.8)
        box-shadow: 0 0 1vh rgba(0,0,0,0.5)

        .cellHeader
          flex: 0 1 auto
          display: flex
          flex-direction: row
          justify-content: space-between
          height: 7.5vh
          clear: both
          background-color: rgba(20,20,20,.8)

          .cellHeaderTitle
            font-size: 4.0vh
            flex: 0.5 1 auto
            margin-left: 2vh

      .verticalCell
        display: flex
        flex-direction: column
        .newsContainer
          flex: 1 1 auto
</style>
