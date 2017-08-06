import Vue from 'vue';

import App from './App';
import store from './store';
import Localization from './localization';

Vue.config.productionTip = false;
Vue.config.devtools = true;

Vue.use(Localization, store);

// eslint-disable-next-line no-new
const vm = new Vue({
  components: { App },
  el: '#app',
  template: '<App/>',
  store,
});

// Initialize gameLibrary module
vm.$store.dispatch('init');
