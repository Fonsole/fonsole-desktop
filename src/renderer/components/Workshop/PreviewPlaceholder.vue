<template>
  <div class="preview-placeholder">
    <a class="logoContainer">
      <div class="logo animateGears">
        <div class="logoInner">
          <div class="logoGear small"></div>
          <div class="logoGear large"></div>
        </div>
      </div>
    </a>
    <h2>{{ $localize('select') }}</h2>
  </div>
</template>

<script>
export default {
  name: 'PreviewPlaceholder',
};
</script>

<style lang="sass" scoped>
  @import "~@/assets/styles/global.sass"

  .preview-placeholder
    align-items: center
    justify-content: center
    flex-direction: column
</style>
