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
(async () => {
  const roomStatus = await networking.openRoom();
  store.commit('setRoomName', roomStatus.roomName);
})();

Vue.use(Localization, store);

// eslint-disable-next-line no-new
new Vue({
  components: { App },
  el: '#app',
  template: '<App/>',
  store,
});
