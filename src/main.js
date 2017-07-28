// The Vue build version to load with the `import` command
// (runtime-only or standalone) has been set in webpack.base.conf with an alias.
import Vue from 'vue';
import Vuex from 'vuex';
import App from './App';
import router from './router';
import { PPlatform } from './network/pplatformclient';

Vue.use(Vuex);
Vue.config.productionTip = false;

const gPlatform = new PPlatform();
const store = new Vuex.Store({
  state: {
    currentGame: 'menu',
  },
  mutations: {
    injectPlatform: (state, frame) => {
      gPlatform.injectAPI(frame.contentWindow);
    },
    createRoom: (/* state, roomName */) => {
      gPlatform.join();
    },
  },
});

/* eslint-disable no-new */
new Vue({
  el: '#app',
  router,
  store,
  template: '<App/>',
  components: { App },
});
