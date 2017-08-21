<template>
  <div class="workspace-container">
    <device-window
      v-for="(device, deviceIndex) in devices"
      :isDesktop="deviceIndex === 0"
      :key="device.session"
      :session="device.session"
    ></device-window>
  </div>
</template>

<script>
  import DeviceWindow from './DeviceWindow';

  export default {
    name: 'WorkspaceContainer',
    components: {
      DeviceWindow,
    },
    data: () => ({
      devices: [
        { session: 'DESKTOP' },
      ],
    }),
    mounted() {
      this.devices.push({ session: 'CONTROLLER1' });
    },
  };
</script>

<style lang="sass" scoped>
  .workspace-container
    flex: 1 1 auto
    position: relative
    background-color: rgba(0, 0, 0, 0.4)
</style>
