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
    injectPlatform: (state, frame) => {
      console.log(networking.gameEmitToPlayer);
      frame.contentWindow.gameEmitToPlayer = networking.gameEmitToPlayer.bind(networking);
      frame.contentWindow.gameEmitToEveryone = networking.gameEmitToEveryone.bind(networking);
      frame.contentWindow.gameOn = networking.gameOn.bind(networking);
      frame.contentWindow.gameOnce = networking.gameOnce.bind(networking);
      frame.contentWindow.platformType = networking.platform;
    },
  },
});
networking.once('room-joined', (roomName) => {
  alert(roomName);
  store.commit('setRoomName', roomName);
});

Vue.use(Localization, store);

/* eslint-disable no-new */
new Vue({
  el: '#app',
  store,
  template: '<App/>',
  components: { App },
});
