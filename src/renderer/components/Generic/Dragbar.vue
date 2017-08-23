<template>
  <div
    :class="['dragbar', direction]"
    @mousedown="mousedown"
  ></div>
</template>

<script>
  import _ from 'lodash';

  export default {
    name: 'Dragbar',
    props: {
      direction: {
        type: String,
        validator: value => value === 'vertical' || value === 'horizontal',
      },
    },
    mounted() {
      // Globally add listener to mouseup, so it will be triggered even when mouse
      // button was released on another panel
      document.addEventListener('mouseup', this.mouseup);

      // Check that we are between 2 other elements, that can be resized
      const previous = this.$el.previousElementSibling;
      const next = this.$el.nextElementSibling;
      if (!previous || !next) throw new Error('Dragbar should be placed between 2 siblings');
    },
    methods: {
      mousedown() {
        document.addEventListener('mousemove', this.mousemove);
      },
      cssUnitToPx(value) {
        const numValue = parseFloat(value);
        if (value.endsWith('px')) return numValue;
        if (value.endsWith('vw')) return numValue * (window.innerWidth / 100);
        if (value.endsWith('vh')) return numValue * (window.innerHeight / 100);
        if (value.endsWith('%')) {
          const cwh = this.direction === 'vertical' ? 'clientWidth' : 'clientHeight';
          return numValue * (this.$parent.$el[cwh] / 100);
        }
        return 0;
      },
      mousemove(event) {
        // Declare variables that will be used as keys
        // for properties which should depend on direction
        const xy = this.direction === 'vertical' ? 'x' : 'y';
        const wh = this.direction === 'vertical' ? 'width' : 'height';
        const cwh = this.direction === 'vertical' ? 'clientWidth' : 'clientHeight';
        const mwh = this.direction === 'vertical' ? 'minWidth' : 'minHeight';
        const lt = this.direction === 'vertical' ? 'left' : 'top';

        // Calculate coordinate within parent, instead of document
        const actualCoord = event[xy] - this.$parent.$el.getBoundingClientRect()[lt];

        // Get parent size, so we can do percentage calculations
        const parentSize = this.$parent.$el[cwh];

        // Use previous and next sibling elements as resizable components
        const previous = this.$el.previousElementSibling;
        const next = this.$el.nextElementSibling;

        // Get minimal resoultions for resized elements.
        const minPrevious = this.cssUnitToPx(getComputedStyle(previous)[mwh]);
        const minNext = this.cssUnitToPx(getComputedStyle(next)[mwh]);
        if (!previous || !next) throw new Error('Dragbar should be placed between 2 siblings');

        // Clamp coord to handle min-* style
        const clampedCoord = _.clamp(actualCoord, minPrevious, parentSize - minNext);

        // Convert px coord to percentage, so resize can work better
        const resolutionPercent = (clampedCoord / parentSize) * 100;

        // Apply percentage resolutions on elements
        previous.style[wh] = `${resolutionPercent}%`;
        next.style[wh] = `${100 - resolutionPercent}%`;
      },
      mouseup() {
        // Remove mousemove listener from document after mouse button was released
        document.removeEventListener('mousemove', this.mousemove);
      },
    },
  };
</script>

<style lang="sass" scoped>
  .dragbar
    background-color: #111

  .vertical
    width: 5px
    cursor: col-resize

  .horizontal
    height: 5px
    cursor: row-resize
</style>
