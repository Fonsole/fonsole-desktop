<template>
  <div class="workspace-toolbar">
    <toolbar-icon
      name="refresh"
      @click.native="$emit('refresh')"
    ></toolbar-icon>
    <toolbar-icon
      name="refreshAll"
      @click.native="$emit('refreshAll')"
    ></toolbar-icon>
    <toolbar-icon
      name="recreateSession"
      @click.native="$emit('recreateSession')"
    ></toolbar-icon>
    <toolbar-icon
      class="toolbar-icon-exit"
      name="exit"
      @click.native="exit"
    ></toolbar-icon>
  </div>
</template>

<script>
  import ToolbarIcon from './ToolbarIcon';

  export default {
    name: 'WorkspaceToolbar',
    components: {
      ToolbarIcon,
    },
    computed: {
      canAddDevices() {
        const players = this.$store.getters.workspaceGameInfo.players;
        if (!players) return true;
        // TODO
        return true;
      },
    },
    methods: {
      exit() {
        this.$store.commit('setPage', 'workshop');
      },
    },
  };
</script>

<style lang="sass" scoped>
  .workspace-toolbar
    display: flex
    background-color: rgba(0, 0, 0, 0.8)
  .toolbar-icon-exit
    margin-left: auto !important
</style>
