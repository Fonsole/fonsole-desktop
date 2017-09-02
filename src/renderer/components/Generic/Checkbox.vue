<template>
  <div id="checkbox">
    <label :style="{color: checked ? 'white' : ''}">{{ title }}</label>
    <input type="checkbox" value="0" :checked="checked" @change="change">
  </div>
</template>
<script>
  export default {
    name: 'Checkbox',
    props: {
      title: {
        type: String,
        default: '',
      },
      value: {
        type: Boolean,
        default: false,
      },
    },
    data: () => ({
      checked: this.value,
    }),
    methods: {
      change() {
        this.checked = !this.checked;
        this.$emit('input', this.checked);
      },
    },
  };
</script>
<style lang="sass" scoped>
  #checkbox
    width: 100%

  label
    font-size: 2.0vh
    transition: color 0.2s ease-in

  input
    float: right
    width: 1.8vh
    height: 1.8vh
</style>
