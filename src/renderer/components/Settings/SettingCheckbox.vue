<template>
  <div id="checkbox">
    <label>{{ title }}</label>
    <input type="checkbox" v-model="checkboxValue">
  </div>
</template>
<script>
export default {
  name: 'Checkbox',
  props: ['title', 'value'],
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

  label
    font-size: 2.0vh

  input
    float: right
    margin-right: 20px
    width: 1.8vh
    height: 1.8vh
</style>
