<template>
  <div class="workspace-container">
    <div class="column-main">
      <workspace-console></workspace-console>
      <dragbar direction="horizontal"></dragbar>
      <device-window
        class="device-window"
        :desktop="true"
        session="DESKTOP"
      ></device-window>
    </div>
    <dragbar direction="vertical"></dragbar>
    <div class="column-devices">
      <device-window
        class="device-window"
        v-for="device in devices"
        :key="device.session"
        :session="device.session"
      ></device-window>
    </div>
  </div>
</template>

<script>
  import Dragbar from '@/components/Generic/Dragbar';
  import DeviceWindow from './DeviceWindow';
  import WorkspaceConsole from './WorkspaceConsole';

  export default {
    name: 'WorkspaceContainer',
    components: {
      DeviceWindow,
      Dragbar,
      WorkspaceConsole,
    },
    data: () => ({
      devices: [],
    }),
    mounted() {
      this.devices.push({ session: 'CONTROLLER1' });
    },
  };
</script>

<style lang="sass" scoped>
  .workspace-container
    display: flex
    flex: 1 1 auto
    background-color: rgba(0, 0, 0, 0.4)

  .column-main
    display: flex
    flex-flow: column
    width: 60%
    min-width: 20%

    .workspace-console
      height: 40%
      min-height: 10%

    .device-window
      height: 60%
      min-height: 20%

  .column-devices
    display: flex
    min-width: 20%
</style>
