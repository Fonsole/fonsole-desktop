<template>
  <div class="setting-dropdown">
    <label>{{ text }}</label>
    <multiselect
      class="multiselect"
      v-model="value"
      :options="options"
      :showLabels="false"
      :searchable="false"
      :allowEmpty="false"
    >
      <template slot="option" scope="props">
        {{ props.option }}
      </template>
    </multiselect>
  </div>
</template>

<script>
import Multiselect from 'vue-multiselect';
import Setting from '@/components/Generic/Setting';

export default {
  name: 'SettingDropdown',
  components: {
    Multiselect,
  },
  mixins: [Setting],
  props: {
    text: {
      type: String,
      default: '',
    },
    options: {
      type: Array,
      required: true,
    },
  },
};
</script>

<style src="vue-multiselect/dist/vue-multiselect.min.css"></style>
<style lang="sass" scoped>
  .setting-dropdown
    display: flex
    flex-flow: row

  .multiselect
    width: 40%
    margin-left: auto

    .multiselect-item
      color: white
</style>
