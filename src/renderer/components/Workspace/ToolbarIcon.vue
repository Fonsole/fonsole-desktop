<template>
  <div class="toolbar-icon">
    <img :src="src" :alt="name" draggable="false">
  </div>
</template>

<script>
  export default {
    name: 'ToolbarIcon',
    props: {
      name: {
        type: String,
        required: true,
      },
    },
    computed: {
      src() {
        try {
          // eslint-disable-next-line import/no-dynamic-require
          return require(`@/assets/workshop_toolbar_${this.name}.png`);
        } catch (err) {
          return null;
        }
      },
    },
  };
</script>

<style lang="sass" scoped>
  .toolbar-icon
    width: 2.5vw
    height: 2.5vw
    margin: 0.5vw

    > img
      width: 100%
      height: 100%
</style>
