<template>
  <div id="checkbox">
    <label>{{ text }}</label>
    <input type="checkbox" v-model="checkboxValue">
  </div>
</template>
<script>
export default {
  name: 'checkbox',
  props: ['text', 'value'],
  data: () => ({
    checkboxValue: false,
  }),
  watch: {
    checkboxValue() {
      this.$emit('input', this.checkboxValue);
    },
  },
};
</script>
<style lang="sass" scoped>
  #checkbox
    width: 100%

  input
    float: right
    margin-right: 20px
</style>
