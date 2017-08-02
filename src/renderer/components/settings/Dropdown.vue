<!-- <label>{{ text }}</label> -->
<template>
  <div id="dropdown">
    <label>{{ text }}</label>
    <div class="s-dropdown" @mouseup="onMouseUp">
      <div class="s-dropdown-toggle" @click="onToggle" @focus="onFocus" @blur="onBlur">
        <slot name="toggle"></slot>
      </div>
      <div class="s-dropdown-menu" v-show="active" @mousedown.stop>
        <slot></slot>
      </div>
    </div>
  </div>
</template>

<script>

export default {
  props: {
    closeAfterClick: {
      type: Boolean,
      default: false,
    },
    toggleOnOnly: {
      type: Boolean,
      default: false,
    },
    focusOpen: {
      type: Boolean,
      default: false,
    },
    text: {
      type: String,
      default: '',
    },
  },
  data() {
    return {
      active: false,
    };
  },
  watch: {
    active(active, formerActive) {
      if (active === formerActive) return;
      if (active) {
        document.addEventListener('mousedown', this.onClose, false);
      } else {
        document.removeEventListener('mousedown', this.onClose, false);
      }
    },
  },
  methods: {
    onToggle() {
      this.active = !this.active;
    },
    onOpen() {
      this.active = true;
    },
    onClose() {
      this.active = false;
    },
    onFocus() {
      if (this.focusOpen) this.onOpen();
    },
    onBlur() {
      const { activeElement } = document;
      if (activeElement !== document.body && !this.$el.contains(activeElement)) this.onClose();
    },
    onMouseUp() {
      if (this.closeAfterClick) this.onClose();
    },
  },
};
</script>

<style lang="sass" scoped>
  #dropdown
    width: 100%

  button
    position: relative
    padding: 0px
    margin: 0px
    font-family: 'zekton'
    border: none
    background: none
    color: white
    text-align: right
    font-size: 14px

  button:focus
    outline: 0

  button:hover
    opacity: 0.7

  .s-dropdown
    position: relative
    display: inline-block
    float: right
    margin-right: 20px

    &-item
      padding: 4px
      white-space: nowrap
      text-align: center
      cursor: pointer
      &:hover
        opacity: 0.7

    &-toggle
      cursor: pointer

    &-menu
      position: relative
      background: rgba(20,20,20,.4)
      z-index: 10
      right: 0
      top: 100%
      margin-top: 5px
</style>
