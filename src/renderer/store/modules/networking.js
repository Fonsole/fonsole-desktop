import NetworkingAPI from 'fonsole-networking/client';

const networking = new NetworkingAPI();

export default {
  state: {
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
  actions: {
    async openRoom(store) {
      const roomStatus = await networking.openRoom();
      store.commit('setRoomName', roomStatus.roomName);
    },
  },
};
