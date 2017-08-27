<template>
  <div class="cell">
    <div class="cellHeader">
      <h2 class="cellHeaderTitle">{{ header }}</h2>
      <ul>
        <li v-for="n in 3" :key="n">
          <input type="radio" v-model="currentSlide" :id="'selector' + n" :value="n">
          <label :for="'selector' + n"></label>
          <div class="check">
            <div class="inside">
            </div>
          </div>
        </li>
      </ul>
    </div>
    <div class="slot">
      <slot>

      </slot>
    </div>
  </div>
</template>

<script>
  export default {
    name: 'Cell',
    props: {
      header: {
        type: String,
        default: 'Header!',
      },
    },
    data: () => ({
      currentSlide: 1,
    }),
  };
</script>

<style lang="sass" scoped>
  h2
    font-size: 2.5vw
    margin: 1vh

  .cell
    flex: 1 1 auto
    background-color: rgba(20,20,20,0.8)
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

      ul
        list-style: none
        list-style-type: none
        margin: 0
        padding: 0
        overflow: auto
        flex: 1 1 auto
        display: flex

        li
          color: #AAAAAA
          display: inline
          position: relative
          float: left
          width: 3.5vh
          // height: 100%

          label
            position: absolute
            width: 3.5vh
            height: 7.5vh
            z-index: 5

          &:hover .check
            border: 0.25vh solid #c7c746

          input[type=radio]
            position: absolute
            visibility: hidden
            max-width: 0
            max-height: 0

          input[type=radio]:checked ~ .check
            height: 2.5vh
            width: 2.5vh
            background: #c7c746
            border-width: 0
            border-radius: 1.25vh

          .check
            display: block
            border: 0.25vh solid #c7c746
            border-radius: 100%
            height: 2vh
            width: 2vh
            transition: all .25s linear
            position: absolute
            top: 50%
            left: 50%
            transform: translate(-50%, -50%)

      .cellHeaderTitle
        font-size: 4.0vh
        margin-left: 2vh

    .slot
      flex: 1 1 auto
      display: flex
</style>
