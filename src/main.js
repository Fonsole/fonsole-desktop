// The Vue build version to load with the `import` command
// (runtime-only or standalone) has been set in webpack.base.conf with an alias.
import NetworkingAPI from 'fonsole-networking/client';
import Vue from 'vue';
import Vuex from 'vuex';

import App from './App';
import Localization from './localization';

Vue.use(Vuex);
Vue.config.productionTip = false;

const networking = new NetworkingAPI();
const store = new Vuex.Store({
  state: {
    currentContentIndex: 'home',
    roomName: '',
  },
  mutations: {
    setRoomName: (state, roomName) => {
      state.roomName = roomName;
    },
    attachNetworkingApi: (state, frame) => {
      // eslint-disable-next-line no-underscore-dangle
      frame.__NetworkingAPI = networking.export();
    },
  },
});
networking.openRoom()
  .then((status) => {
    store.commit('setRoomName', status.roomName);
  })
  .catch((message) => {
    throw new Error(message);
  });

Vue.use(Localization, store);

// eslint-disable-next-line no-new
new Vue({
  el: '#app',
  store,
  template: '<App/>',
  components: { App },
});
