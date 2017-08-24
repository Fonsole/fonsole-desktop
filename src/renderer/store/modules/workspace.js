/**
 * @file A store for development server & game development workspace.
 *       This module will be loaded only on electron version.
 */

import Networking from 'fonsole-networking';
import NetworkingAPI from 'fonsole-networking/client';

const server = new Networking({
  isLocal: true,
  port: 46839,
});
server.listen();

// Using other networking.
// TODO Start a local server there.
const networking = new NetworkingAPI({
  forcedServer: 'http://127.0.0.1:46839/',
});

export default {
  state: {
    roomName: '',
    gamePath: '',
  },
  getters: {
    workspaceGameInfo: (state, getters, rootState, rootGetters) =>
      state.gamePath && rootGetters.workshopGameInfo(state.gamePath),
  },
  mutations: {
    setRoomName: (state, roomName) => {
      state.roomName = roomName;
    },
    setWorkspaceGame(state, gamePath) {
      state.gamePath = gamePath;
    },
  },
  actions: {
    async openRoom(store) {
      const roomStatus = await networking.openRoom();
      // Room name is formed by server index + room name on server.
      store.commit('setRoomName', `${networking.serverIndex}${roomStatus.roomName}`);
    },
    attachNetworkingApi(state, frame) {
      // eslint-disable-next-line no-underscore-dangle
      frame.__NetworkingAPI = networking.export();
    },
  },
};
