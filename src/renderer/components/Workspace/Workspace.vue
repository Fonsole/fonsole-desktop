<template>
  <div class="workspace">
    <workspace-toolbar></workspace-toolbar>
    <workspace-container></workspace-container>
  </div>
</template>

<script>
  import WorkspaceToolbar from './WorkspaceToolbar';
  import WorkspaceContainer from './WorkspaceContainer';

  export default {
    name: 'GameWorkspace',
    components: {
      WorkspaceToolbar,
      WorkspaceContainer,
    },
    beforeMount() {
      this.$store.commit('setTopbarVisible', false);
    },
    destroyed() {
      this.$store.commit('setTopbarVisible', true);
    },
  };
</script>

<style lang="sass" scoped>
  .workspace
    width: 100%
    height: 100vh
    display: flex
    flex-direction: column
    align-items: stretch
</style>
