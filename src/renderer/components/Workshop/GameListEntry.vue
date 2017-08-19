<template>
  <div class="game-list-entry">
    {{ gameName }}
  </div>
</template>

<script>
  export default {
    name: 'GameListEntry',
    props: {
      gamePath: {
        type: String,
        required: true,
      },
    },
    computed: {
      gameName() {
        return (this.$store.getters.workshopGameInfo(this.gamePath) || {}).name;
      },
    },
  };
</script>

<style lang="sass" scoped>
  .game-list-entry
    box-sizing: border-box
    width: 100%
    background-color: rgba(30, 30, 30, 0.6)
    transition: transform 0.1s linear
    font-size: 6vh
    padding: 5px

    &:hover
      transform: translateY(-0.2rem) translateX(-0.04rem)

    &:active
      transform: translateY(0.2rem) translateX(0.04rem)
</style>
