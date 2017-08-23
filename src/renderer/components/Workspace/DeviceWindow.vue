<template>
  <div :class="['device-window', {desktop}]">
    {{ `DEVICE: ${session}` }}
    <webview class="webview" ref="webview">
    </webview>
  </div>
</template>

<script>
  export default {
    name: 'DeviceWindow',
    props: {
      session: {
        type: String,
        required: true,
      },
      desktop: {
        type: Boolean,
        default: false,
      },
    },
  };
</script>

<style lang="sass" scoped>
  .device-window
    background-color: #888
    display: flex
    flex-flow: column

  .webview
    flex: 1
    background-color: red
</style>
