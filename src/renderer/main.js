import Vue from 'vue';

import App from './App';
import store from './store';
import Localization from './localization';

Vue.config.productionTip = false;
Vue.config.devtools = true;

Vue.use(Localization, store);

const vm = new Vue({
  components: { App },
  el: '#app',
  template: '<App/>',
  store,
});

// Initialize gameLibrary module
vm.$store.dispatch('init');

// Open room
vm.$store.dispatch('openRoom');
