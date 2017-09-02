<template>
  <div id="checkbox">
    <label>{{ title }}</label>
    <input type="checkbox" v-model="value">
  </div>
</template>
<script>
  import Setting from '@/components/Generic/Setting';

  export default {
    name: 'SettingCheckbox',
    mixins: [Setting],
    props: {
      title: {
        type: String,
        default: '',
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
    width: 1.8vh
    height: 1.8vh
</style>
