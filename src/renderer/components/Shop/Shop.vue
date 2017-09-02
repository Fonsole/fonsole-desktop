<template>
  <div id="shop" class="tabContents">
    <div class="header">
      <h2 class="headerTitle">{{ $localize('mtx') }}</h2>
      <div class="coinsIcon">

      </div>
      <h2 class="headerTitle coins">{{ coins }}</h2>
    </div>
    <div class="tabs">
      <ul id="categories">
        <li>
          <input type="radio" v-model="showGames" value="1" id="radio2"></input>
          <label for="radio2">{{ $localize('games') }}</label>
        </li>
        <li>
          <label style="opacity: 0.5">/</label>
        </li>
        <li>
          <input type="radio" v-model="showGames" value="0" id="radio1" checked="true"></input>
          <label for="radio1">{{ $localize('cosmetics') }}</label>
        </li>
      </ul>
      <h2 class="headerTitle purchaseCoins">{{ $localize('purchase_coins') }}</h2>
    </div>
    <div class="contents">

    </div>
  </div>
</template>

<script>
  import FButton from '@/components/Generic/FButton';
  import StarRating from '@/components/Generic/StarRating';

  export default {
    name: 'Shop',
    components: {
      fbutton: FButton,
      StarRating,
    },
    data: () => ({
      coins: 250,
      showGames: 1,
    }),
    methods: {
    },
  };
</script>

<style lang="sass" scoped>
  #shop
    display: flex
    flex-direction: column
    width: 100%
    height: 100%
    background-color: rgba(20,20,20,.8)
    position: relative

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

      &.purchaseCoins
        font-size: 2vh
        background-color: #E3B727
        box-shadow: 0px 0.5vh 0px #715B14
        border-radius: 1vh
        margin-left: auto
        margin-right: 1vw
        padding: 0.2vw
        padding-left: 1vw
        padding-right: 1vw
        transition: transform 0.15s, box-shadow 0.15s

        &:hover:active
          box-shadow: 0 1px 0 #715B14
          transition: transform 0.05s, box-shadow 0.05s
          transform: rotateX(15deg) translateY(4px)

      &.coins
        margin: auto 0
        margin-right: 0.7vw
        margin-left: 0.7vw

    .coinsIcon
      margin-left: auto
      background-image: url('../../assets/coin.png')
      background-position: 50% 50%
      background-size: 4.5vh 4.5vh
      background-repeat: no-repeat
      display: inline-block
      width: 4.5vh
      height: 100%
      vertical-align: middle
      content: ''

    .tabs
      flex: 0 1 auto
      display: flex
      flex-direction: row
      height: 5vh
      min-height: 5vh
      clear: both
      background-color: rgba(10,10,10,.4)

      ul
        padding: 0px
        margin: 0px
        list-style: none
        li
          display: inline-block
          margin: 1vh
          margin-left: 0.1vh
          font-size: 2.5vh

          input
            appearance: none
            display: none
          input + label
            transition: opacity 0.2s linear 0.0s
          input:not(:checked) + label
            opacity: 0.5

        &:first-child
          margin-left: 2vh

    .contents
      flex: 1 1 auto
</style>
