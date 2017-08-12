<template>
  <div id="library" v-if="currentCategory === ''">
    <div v-for="(category,n) in categories"
    :key="n" :id="category"
    :class="['category', category]" @click="onCategoryClicked">
      <div class="caption">
        <div class="header">
          <h2>
            {{ $localize(category) }}
          </h2>
        </div>
        <p>
          {{ $localize(category + '_Descr') }}
        </p>
      </div>
    </div>
  </div>
  <div class="gameList" v-else-if="currentCategory === 'featured'">
    <preview v-for="(game,n) in 6" :key="n" :gameID="game" class="game"
    style="height: 45%;"></preview>
  </div>
  <div class="gameList" v-else-if="currentCategory === 'new'">
    <preview v-for="(game,n) in 6" :key="n" :gameID="game" class="game"
    style="height: 45%;"></preview>
  </div>
  <div class="gameList" v-else-if="currentCategory === 'best'">
    <preview v-for="(game,n) in 6" :key="n" :gameID="game" class="game"
    style="height: 45%;"></preview>
  </div>
</template>

<script>
  import Preview from './Preview';

  export default {
    name: 'Library',
    components: {
      preview: Preview,
    },
    data: () => ({
      categories: ['featured', 'new', 'best'],
      currentCategory: '',
    }),
    methods: {
      onCategoryClicked(event) {
        this.currentCategory = event.target.id;
      },
    },
  };
</script>

<style lang="sass" scoped>
  #library
    width: 100%
    margin: 0 auto
    display: flex
    flex-direction: row
    flex-wrap: nowrap
    justify-content: center
    align-items: center
    padding: 5vh
    perspective: 200vh

    .category
      position: relative
      height: 100%
      flex: 0.3 1 auto
      overflow: hidden
      margin: 2vh
      box-shadow: 0 0 1vh rgba(0,0,0,0.5)
      transition: all 0.4s linear
      background-size: 100% 100%

      &.new
        background: url('../../assets/category_new.jpg') center

      &.featured
        background: url('../../assets/category_featured.jpg') center

      &.best
        background: url('../../assets/category_best.jpg') center

      &:hover
        transform: rotateY(5deg)
        box-shadow: 0 0 2vh rgba(75,200,75,0.5)
        .caption
          max-height: 22vh

      .caption
        position: absolute
        bottom: 0
        width: 100%
        background: rgba(0,0,0,0.9)
        margin: 0
        max-height: 9.5vh
        transition: all 0.4s linear 0s

        .header
          height: 9.5vh
          h2
            font-size: 5vh
            padding: 2vh
            margin: 0

        p
          margin-top: 0
          margin-left: 2vh

  .gameList
    width: 100%
    margin: 0 auto
    display: flex
    flex-direction: row
    flex-wrap: wrap
    justify-content: center
    align-items: center
    padding: 5vh
    padding-top: 0

    .game
      width: 30%
      margin: 2vh
      position: relative
      box-shadow: 0 0 1vh rgba(0,0,0,0.5)
      transition: all 0.3s ease

      &:hover
        transform: translateY(-1vh)
        box-shadow: 0 0 2vh rgba(75,200,75,0.5)

      &:active
        transform: translateY(0vh)
        opacity: 0.8
</style>
