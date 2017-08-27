<template>
  <div class="cell">
    <div class="cellHeader">
      <h2 class="cellHeaderTitle">{{ header }}</h2>
      <template v-if="showSelector">
        <ul>
          <li v-for="n in slidesCount" :key="n">
            <input type="radio" v-model="currentSlide" :id="cellName + n" :value="n">
            <label :for="cellName + n"></label>
            <div class="check">
              <div class="inside">
              </div>
            </div>
          </li>
        </ul>
      </template>
    </div>
    <div class="slot">
      <slot>
        <cell-slide v-for="(slide,n) in slides" :key="n"
        :caption="$localize(slide.caption)" :backgroundURL="slide.backgroundURL"
        :style="{ opacity: currentSlide == n + 1 ? '1.0' : '0.0' }">
        </cell-slide>
      </slot>
    </div>
  </div>
</template>

<script>
  import CellSlide from '@/components/Generic/CellSlide';

  export default {
    name: 'Cell',
    components: {
      'cell-slide': CellSlide,
    },
    props: {
      name: {
        type: String,
        default: 'cell_name',
      },
      header: {
        type: String,
        default: 'Header!',
      },
      slides: {
        type: Array,
        default: [{ caption: 'save_50', backgroundURL: 'testgame3.jpg' }],
      },
    },
    data: () => ({
      currentSlide: 1,
    }),
    computed: {
      showSelector() {
        return this.slides.length > 1;
      },
      slidesCount() {
        return this.slides.length;
      },
      cellName() {
        return this.name;
      },
    },
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
      position: relative
</style>
