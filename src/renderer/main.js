import Vue from 'vue';
import AsyncComputed from 'vue-async-computed';

import App from './App';
import store from './store';
import Localization from './localization';

Vue.config.productionTip = false;
Vue.config.devtools = true;

Vue.use(AsyncComputed);
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

// @ifdef DEBUG
window.vm = vm;
// @endif
