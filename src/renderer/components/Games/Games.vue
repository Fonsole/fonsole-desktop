<template>
  <div id="games" class="tabContents">
    <ul id="categories">
      <li>
        <input type="radio" v-model="showLibrary" value="1" id="radio2"></input>
        <label for="radio2">{{ $localize('library') }}</label>
      </li>
      <li>
        <label style="opacity: 0.5">|</label>
      </li>
      <li>
        <input type="radio" v-model="showLibrary" value="0" id="radio1" checked="true"></input>
        <label for="radio1">{{ $localize('installed') }}</label>
      </li>
    </ul>
    <!-- <div class="divider"></div> -->
    <div class="contents">
      <component
        :is="showLibrary == 1 ? 'library' : 'installed'"
        transition="fade"
        transition-mode="out-in"
      ></component>
    </div>
  </div>
</div></template>

<script>
  import Installed from './Installed';
  import Library from './Library';

  export default {
    name: 'Games',
    components: {
      installed: Installed,
      library: Library,
    },
    data: () => ({
      showLibrary: 1,
    }),
  };
</script>

<style lang="sass" scoped>
  h1, h2
    margin: 0px

  label
    font-size: 25px
    text-align: center

  #games
    background-color: rgba(20,20,20,.8)
    // padding: 10px
    display: flex
    flex-direction: column
    align-items: left
    justify-content: center

    .divider
      width: 50%
      margin-left: 10px

    ul
      padding: 0px
      margin: 0px
      list-style: none
      li
        display: inline-block
        margin: 15px

        input
          appearance: none
          display: none
        input + label
          transition: opacity 0.2s linear 0.0s
        input:not(:checked) + label
          opacity: 0.5

    .contents
      display: flex
      flex: 1 1 auto
</style>
